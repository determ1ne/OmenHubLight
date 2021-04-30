# OMEN Hub Light

OMEN Hub Light is an opensource and lightweight substitute for a series programs from HP. It is based on the reverse engineering, and trying to keep a similar architecture to HP's programs for keeping update with HP. 

## Why I need it?

Tuning your computer fast when others are still launching OMEN Gaming Hub, without annoying HP product advertisements.

The goal is to replace or help you removing the following HP Apps:

- OMEN Gaming Hub (Previously "OMEN Command Center")
- HP System Event Utility
- HP Application Enabling Services (Software component driver)
  - HP Analytics services
  - HP App Helper HSA Service
  - HP Diagnostics HSA Service
  - HP Network HSA Service
  - HP System Info HSA Service
- Omen Software and Services (Software component driver)
  - HP Omen HSA Service

Also, you can avoid the "HP System Event Utility" pouring trashes to your `C:\system.sav`, and other components with various stacks of telemetry components.

## How to use it?

Since OMEN Gaming Hub is a huge project, I only implemented a small part of it which my notebook (OMEN 15-dc0xxx, i7-8750H with NVIDIA GTX 1060) needed.  Currently, the application support the following feature:

- Four Zone Keyboard Light
- Fan Control (For the old models with only three options)

But if you want, you can easily port what you need, because the fundamental chores are available in a similar form to what HP programed in their applications. Happy hacking!

**REQUIRES .NET 5**

Build and run **OmenHubLight** is enough. OmenDriverPatcher enables you to use LPC service from HP driver, whose client is provided in `NativeRpcClient.dll` from OMEN Gaming Hub or HP System Event Utility app package.