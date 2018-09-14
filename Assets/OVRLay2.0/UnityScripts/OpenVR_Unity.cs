using System;
using UnityEngine;
using Valve.VR;
using OVRLay;

public class OpenVR_Unity : MonoBehaviour
{
    #region Public

    public bool connectedToOpenVR = false;
    [Space(10)]
    public bool connectOnStart = true;
    public EVRApplicationType appType = EVRApplicationType.VRApplication_Background;
    [Space(10)]
    public bool tryUntilConnect = false;
    public float timeBetweenTries = 5f;
    [Space(10)]
    public bool pollForEvents = true;
    public bool autoUpdatePoses = true;
    [Space(10)]
    public D_OpenVRConnected OpenVRConnected = () => { };
    public D_OpenVRDisconnected OpenVRDisconnected = () => { };
    public delegate void D_OpenVRConnected();
    public delegate void D_OpenVRDisconnected();
    [Space(10)]
    public OVR.D_OnDashboardChange OnDashboardChange;
    public OVR.D_OnStandbyChange OnStandbyChange;
    public OVR.D_OnChaperoneSettingsChange OnChaperoneSettingsChange;
    public OVR.D_OnVRAppQuit OnVRAppQuit;

    #endregion

    private float timeSinceLastTry = 0f;

    void Start()
    {
        if (connectOnStart)
            ConnectToSteam();

        OVR.OnDashboardChange += OnDashboardChange;
        OVR.OnStandbyChange += OnStandbyChange;
        OVR.OnChaperoneSettingsChange += OnChaperoneSettingsChange;
        OVR.OnVRAppQuit += OnVRAppQuit;
    }

    void Update()
    {
        if (!OVR.StartedUp && tryUntilConnect)
        {
            if (timeSinceLastTry < timeBetweenTries)
                timeSinceLastTry += Time.deltaTime;
            else
            {
                ConnectToSteam();
                timeSinceLastTry = 0f;
            }

            return;
        }

        if (autoUpdatePoses)
            UpdatePoses();

        if (pollForEvents)
            PollForEvents();
    }

    public void ConnectToSteam()
    {
        if (!OVR.StartedUp && OVR.Startup(appType))
        {
            connectedToOpenVR = true;
            OpenVRConnected();
        }

    }

    public void DisconnectFromSteam()
    {
        if (OVR.StartedUp)
        {
            OVR.Shutdown();
            connectedToOpenVR = false;
            OpenVRDisconnected();
        }
    }

    public void PollForEvents()
    {
        if (OVR.StartedUp)
            OVR.UpdateEvents();
    }

    public void UpdatePoses()
    {
        if (OVR.StartedUp)
            OVRLay.Pose.UpdatePoses();
    }
}