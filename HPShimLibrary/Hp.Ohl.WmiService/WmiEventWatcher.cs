using System.Management;
using Hp.Ohl.WmiService.Models;

namespace Hp.Ohl.WmiService
{
    public static class WmiEventWatcher
    {
        public static event HpBiosEventHandler HpBiosEventArrived;
        private static ManagementEventWatcher _hpBiosEventWatcher;
        private static readonly object HPBiosEventWatcherLock = new();

        public static void StartHpBiosEventWatcher()
        {
            if (_hpBiosEventWatcher == null)
                lock (HPBiosEventWatcherLock)
                    _hpBiosEventWatcher ??= new ManagementEventWatcher(@"root\wmi", "SELECT * FROM hpqBEvnt");
            _hpBiosEventWatcher.Start();
            _hpBiosEventWatcher.EventArrived += OnHpBiosEventArrived;
            _hpBiosEventWatcher.Stopped += OnHpBiosEventStopped;
        }

        public static void StopHpBiosEventWatcher()
        {
            if (_hpBiosEventWatcher != null)
                _hpBiosEventWatcher.Stop();
        }

        public static void OnHpBiosEventArrived(object sender, EventArrivedEventArgs e)
        {
            var raiseEvent = HpBiosEventArrived;
            if (raiseEvent == null) return;

            // Get WMI event details
            // reference: HPSystemEventUtilityHost.exe:HPSystemEventUtilityHost.HPSystemEventUtilityHost.WmiEventCallback

            var args = new HpBiosEventArgs
            {
                eventId = (uint) e.NewEvent.Properties["EventID"].Value,
                eventData = (uint) e.NewEvent.Properties["EventData"].Value,
                timeCreated = (ulong) e.NewEvent.Properties["TIME_CREATED"].Value,
            };

            HpBiosDataOut dataOut;
            switch (args.eventId)
            {
                case 4:
                    dataOut = HpBiosIntHelper.InvokeBiosCommand(1, 12, 4);
                    args.eventPayload = dataOut.Data switch
                    {
                        _ when dataOut.Data[0] == 165 && dataOut.Data[1] == 33 => new OmenKeyPressedPayload(),
                        _ when dataOut.Data[0] == 164 && dataOut.Data[1] == 33 => new WinKeyLockPayload
                        {
                            IsWinKeyEnabled = dataOut.Data[2] == 1
                        },
                        _ when dataOut.Data[0] == 169 && dataOut.Data[1] == 33 => new TouchPadTogglePayload
                        {
                            IsTouchPadEnabled = dataOut.Data[2] == 1
                        },
                        // MicMute, PrivacyScreen, PrivacyCamera, AppSwitchKey long pressed, SysInfo Key, etc.
                        _ => new NotHandledPayload {OriginalBytes = dataOut.Data},
                    };
                    break;
                case 3:
                    dataOut = HpBiosIntHelper.InvokeBiosCommand(1, 15, 4);
                    args.eventPayload = dataOut.Data switch
                    {
                        _ when dataOut.Data[0] == 2 => new IncompatiblePowerAdapterPayload(),
                        _ when dataOut.Data[0] == 3 => new LowPowerAdapterPayload(),
                        _ => new PowerAdapterPayload(),
                    };
                    break;
                // etc.
            }

            raiseEvent(sender, args);
        }

        public static void OnHpBiosEventStopped(object sender, StoppedEventArgs e)
        {
            _hpBiosEventWatcher.EventArrived -= OnHpBiosEventArrived;
        }
    }
}