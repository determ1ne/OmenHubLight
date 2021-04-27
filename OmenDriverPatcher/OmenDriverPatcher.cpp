#include <Windows.h>
#include <TlHelp32.h>
#include <fstream>
#include <iostream>
#include <map>
#include <sstream>
#include <string>
#include <vector>

struct patch_info
{
    uintptr_t address;
    std::vector<BYTE> orig_code;
    std::vector<BYTE> patch_code;

    patch_info(uintptr_t _address, std::vector<BYTE>&& _orig_code, std::vector<BYTE>&& _patch_code)
        : address(_address)
        , orig_code(std::move(_orig_code))
        , patch_code(std::move(_patch_code))
    {}
};

struct process_info
{
    HANDLE handle;
    DWORD proc_id;
    std::vector<patch_info> PATCH_LIST;
};

std::map<std::wstring, process_info>* PROCESS_LIST;

void PrintError(const LPCSTR desc)
{
    WCHAR error_buffer[256];

    const DWORD dwError = GetLastError();

    FormatMessage(
        FORMAT_MESSAGE_FROM_SYSTEM,
        NULL,
        dwError,
        LANG_USER_DEFAULT,
        error_buffer,
        256,
        NULL
    );

    wprintf_s(L"[Error] (0x%08x) %hs: %ls\n[Error] Exiting", dwError, desc, error_buffer);
}

DECLSPEC_NORETURN
void PrintErrorAndExit(const LPCSTR desc)
{
    PrintError(desc);
    puts("[Critical] Exiting");
    ExitProcess(GetLastError());
}

BYTE* GetModuleBaseAddr(DWORD procId, LPCWSTR modName)
{
    BYTE* base_addr = NULL;

    HANDLE handle = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE | TH32CS_SNAPMODULE, procId);

    if (handle == INVALID_HANDLE_VALUE)
    {
        PrintError("CreateToolhelp32Snapshot");
        return NULL;
    }

    MODULEENTRY32 entry;

    entry.dwSize = sizeof(entry);
    if (Module32First(handle, &entry))
    {
        do
        {
            if (!_wcsicmp(entry.szModule, modName))
            {
                base_addr = entry.modBaseAddr;
                break;
            }
        } while (Module32Next(handle, &entry));
    }

    if (GetLastError() == ERROR_NO_MORE_FILES)
    {
        PrintError("Module32[First|Next]");
    }

    CloseHandle(handle);
    return base_addr;
}

void EnterDebugMode()
{
    HANDLE token;

    if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES, &token))
    {
        PrintErrorAndExit("OpenProcessToken");
    }

    LUID luid;

    if (!LookupPrivilegeValue(NULL, SE_DEBUG_NAME, &luid))
    {
        CloseHandle(token);
        PrintErrorAndExit("LookupPrivilegeValue");
    }

    TOKEN_PRIVILEGES privs;

    privs.PrivilegeCount = 1;
    privs.Privileges[0].Luid = luid;
    privs.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

    if (!AdjustTokenPrivileges(token, FALSE, &privs, sizeof(privs), NULL, NULL))
    {
        CloseHandle(token);
        PrintErrorAndExit("AdjustTokenPrivileges");
    }
}

void GetProcessHandlers()
{
    PROCESSENTRY32 entry;
    entry.dwSize = sizeof(PROCESSENTRY32);
    HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);
    HANDLE hProcess = NULL;
    size_t count = 0;
    size_t total = PROCESS_LIST->size();

    if (Process32First(hSnapshot, &entry))
    {
        do
        {
            for (auto& [key, info] : *PROCESS_LIST)
            {
                if (const auto process_name = key.c_str();
                    info.handle == NULL && _wcsicmp(entry.szExeFile, process_name) == 0)
                {
                    hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, entry.th32ProcessID);
                    if (hProcess)
                    {
                        info.handle = hProcess;
                        info.proc_id = entry.th32ProcessID;
                    }

                    if (++count == total)
                    {
                        // Found all processes
                        CloseHandle(hSnapshot);
                        return;
                    }
                }
            }
        } while (Process32Next(hSnapshot, &entry));
    }

    // Only found some processes
    CloseHandle(hSnapshot);
}

void CloseProcessHandlers()
{
    for (const auto& [_, info] : *PROCESS_LIST)
    {
#pragma warning(disable:6001)
        if (info.handle) CloseHandle(info.handle);
#pragma warning(default:6001)
    }
}

void PatchProcesses()
{
    for (const auto& [proc_name, info] : *PROCESS_LIST)
    {
        if (info.handle == NULL)
        {
            wprintf(L"[WARN] \"%ls\" was not found, skipping\n", proc_name.c_str());
            continue;
        }

        // get base address
        auto base_addr = GetModuleBaseAddr(info.proc_id, proc_name.c_str());
        if (base_addr == NULL)
        {
            wprintf(L"[ERROR] \"%ls\" base address not found, skipping\n", proc_name.c_str());
            continue;
        }

        for (const auto& patch : info.PATCH_LIST)
        {
            auto patch_addr = base_addr + patch.address;
            BYTE* read_buffer = new BYTE[patch.orig_code.size()];
            if (ReadProcessMemory(info.handle, patch_addr, read_buffer, patch.orig_code.size(), NULL) == false)
            {
                wprintf(L"[ERROR] \"%ls\" failed to read process memory %p\n", proc_name.c_str(), patch_addr);
                continue;
            }

            if (!memcmp(&patch.orig_code[0], read_buffer, patch.orig_code.size()))
            {
                DWORD dw_old_protect;
                wprintf(L"[INFO] \"%ls\" patching memory %p\n", proc_name.c_str(), patch_addr);
                VirtualProtectEx(info.handle, patch_addr, patch.patch_code.size(), PAGE_EXECUTE_READWRITE, &dw_old_protect);
                WriteProcessMemory(info.handle, patch_addr, &patch.patch_code[0], patch.patch_code.size(), NULL);
                VirtualProtectEx(info.handle, patch_addr, patch.patch_code.size(), dw_old_protect, &dw_old_protect);
            }
            else
            {
                wprintf(L"[ERROR] \"%ls\" memory content mismatch\n", proc_name.c_str());
                continue;
            }
        }
    }
}

constexpr BYTE CharToHex(char c)
{
    if (c >= '0' && c <= '9') return c - '0';
    else if (c >= 'A' && c <= 'F') return c - 'A' + 10;
    else if (c >= 'a' && c <= 'f') return c - 'a' + 10;
    else return -1;
}

void StringToHex(std::string& str, std::vector<BYTE>& hex)
{
    size_t len = str.length() / 2;
    hex.resize(len);
    for (size_t i = 0; i < len; ++i)
    {
        hex[i] = (CharToHex(str[2 * i]) << 4) + CharToHex(str[2 * i + 1]);
    }
}

int main()
{
    if (std::ifstream patches_file("patches.txt", std::ios::in); patches_file.is_open())
    {
        int patch_count = 0;
        PROCESS_LIST = new std::map<std::wstring, process_info>;
        std::string line;

        while (std::getline(patches_file, line))
        {
            std::istringstream line_parser(line);
            std::string proc_name;
            if (!std::getline(line_parser, proc_name, ',')) continue;

            std::wstring wproc_name(proc_name.begin(), proc_name.end());
            if (PROCESS_LIST->find(wproc_name) == PROCESS_LIST->end())
            {
                PROCESS_LIST->insert({ wproc_name, process_info{NULL, 0, std::vector<patch_info>()} });
            }

            std::string buffer_str;
            std::getline(line_parser, buffer_str, ',');
            uintptr_t offset = std::stoi(buffer_str, nullptr, 16);
            std::getline(line_parser, buffer_str, ',');
            std::vector<BYTE> orig_code;
            StringToHex(buffer_str, orig_code);
            std::getline(line_parser, buffer_str);
            std::vector<BYTE> patch_code;
            StringToHex(buffer_str, patch_code);

            PROCESS_LIST->at(wproc_name).PATCH_LIST.emplace_back(offset, std::move(orig_code), std::move(patch_code));
            ++patch_count;
        }

        printf("[INFO] Patch Count: %d\n", patch_count);

        puts("[INFO] Getting SeDebugPrivilege");
        EnterDebugMode();

        puts("[INFO] Getting Process Handlers");
        GetProcessHandlers();

        puts("[INFO] Patching Processes");
        PatchProcesses();

        puts("[INFO] Closing Process Handlers");
        CloseProcessHandlers();
    }
    else
    {
        puts("[ERROR] Cannot open patches.txt\n[ERROR] Exiting");
        ExitProcess(-1);
    }

    return 0;
}

