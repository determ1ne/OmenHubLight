using System;
using Hp.Ohl.WmiService;
using Hp.Ohl.WmiService.Models;

namespace OmenHubLight.EventWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Action<string, string> wl = (a, b) => Console.WriteLine($"{a}: {b}");

            WmiEventWatcher.HpBiosEventArrived += (sender, eventArgs) =>
            {
                Console.WriteLine("\n========");
                wl("EventId", eventArgs.eventId.ToString());
                wl("EventData", eventArgs.eventData.ToString());
                wl("TimeCreated", eventArgs.timeCreated.ToString());
                switch (eventArgs.eventPayload)
                {
                    case OmenKeyPressedPayload:
                        Console.WriteLine("OMEN key pressed");
                        break;
                    case TouchPadTogglePayload x:
                        wl("TouchPad lock pressed, TouchPad is", x.IsTouchPadEnabled ? "Enabled" : "Disabled");
                        break;
                    case WinKeyLockPayload x:
                        wl("WinKey lock pressed, WinKey is", x.IsWinKeyEnabled ? "Enabled" : "Locked");
                        break;
                    case LowPowerAdapterPayload:
                        Console.WriteLine("Power Adapter has low power supply");
                        break;
                    case IncompatiblePowerAdapterPayload:
                        Console.WriteLine("Incompatible power adapter");
                        break;
                    case PowerAdapterPayload:
                        Console.WriteLine("Power adapter plugged in");
                        break;
                }

                Console.WriteLine("========\n");
            };

            WmiEventWatcher.StartHpBiosEventWatcher();

            Console.Read();

            WmiEventWatcher.StopHpBiosEventWatcher();
        }
    }
}