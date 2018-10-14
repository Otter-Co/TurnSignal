using System;
using System.Collections.Generic;
using Valve.VR;

namespace OVRLay
{
    public static class OVR
    {
        private static readonly uint EVENT_SIZE =
            (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.VREvent_t));

        public static CVRSystem VR_Sys { get; private set; }
        public static CVRApplications Applications { get => OpenVR.Applications; }
        public static CVRCompositor Compositor { get => OpenVR.Compositor; }
        public static CVRChaperone Chaperone { get => OpenVR.Chaperone; }
        public static CVRChaperoneSetup ChaperoneSetup { get => OpenVR.ChaperoneSetup; }
        public static CVROverlay Overlay { get => OpenVR.Overlay; }
        public static CVRSettings Settings { get => OpenVR.Settings; }

        public static EVRInitError LastStartupError { get; private set; } = EVRInitError.None;
        public static bool StartedUp { get; private set; } = false;

        public static bool Startup(
            EVRApplicationType appType = EVRApplicationType.VRApplication_Background
        )
        {
            if (StartedUp)
                return true;

            EVRInitError startupError = EVRInitError.None;
            VR_Sys = OpenVR.Init(ref startupError, appType);

            LastStartupError = startupError;

            return StartedUp = (startupError == EVRInitError.None);
        }

        public static void Shutdown()
        {
            if (StartedUp)
            {
                // OpenVR.Shutdown();
                StartedUp = false;
            }
        }

        public delegate void D_OnVRAppQuit();
        public static D_OnDashboardChange OnDashboardChange = (bool open) => { };
        public delegate void D_OnStandbyChange(bool inStandby);
        public static D_OnStandbyChange OnStandbyChange = (bool inStandby) => { };
        public delegate void D_OnChaperoneSettingsChange();
        public static D_OnChaperoneSettingsChange OnChaperoneSettingsChange = () => { };
        public delegate void D_OnDashboardChange(bool open);
        public static D_OnVRAppQuit OnVRAppQuit = () => { };

        public static void UpdateEvents()
        {
            VREvent_t event_T = new VREvent_t();

            while (VR_Sys.PollNextEvent(ref event_T, EVENT_SIZE))
                switch ((EVREventType)event_T.eventType)
                {
                    case EVREventType.VREvent_DashboardActivated:
                        OnDashboardChange(true);
                        break;
                    case EVREventType.VREvent_DashboardDeactivated:
                        OnDashboardChange(false);
                        break;

                    case EVREventType.VREvent_EnterStandbyMode:
                        OnStandbyChange(true);
                        break;
                    case EVREventType.VREvent_LeaveStandbyMode:
                        OnStandbyChange(false);
                        break;

                    case EVREventType.VREvent_ChaperoneSettingsHaveChanged:
                        OnChaperoneSettingsChange();
                        break;

                    case EVREventType.VREvent_Quit:
                        OnVRAppQuit();
                        break;
                }
        }
    }
}
