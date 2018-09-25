using System;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using OVRLay;

public class OpenVR_Events : MonoBehaviour
{
    [Serializable] public class BoolEvent : UnityEvent<bool> { }
    [Serializable] public class VoidEvent : UnityEvent { }

    public BoolEvent OnDashboardChange;
    public BoolEvent OnStandbyChange;
    public VoidEvent OnChaperoneSettingsChange;
    public VoidEvent OnVRAppQuit;

    void Start()
    {
        if (OnDashboardChange == null)
            OnDashboardChange = new BoolEvent();

        if (OnStandbyChange == null)
            OnStandbyChange = new BoolEvent();

        if (OnChaperoneSettingsChange == null)
            OnChaperoneSettingsChange = new VoidEvent();

        if (OnVRAppQuit == null)
            OnVRAppQuit = new VoidEvent();

        OVR.OnDashboardChange += (open) => OnDashboardChange.Invoke(open);
        OVR.OnStandbyChange += (inStandby) => OnStandbyChange.Invoke(inStandby);
        OVR.OnChaperoneSettingsChange += () => OnChaperoneSettingsChange.Invoke();
        OVR.OnVRAppQuit += () => OnVRAppQuit.Invoke();
    }
}