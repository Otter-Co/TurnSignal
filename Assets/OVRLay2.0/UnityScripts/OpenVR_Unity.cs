using System;
using UnityEngine;
using UnityEngine.Events;
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
    public VoidEvent OpenVRConnected;
    public VoidEvent OpenVRDisconnected;
    [Serializable] public class VoidEvent : UnityEvent { }

    #endregion

    private float timeSinceLastTry = 0f;

    void Start()
    {
        if (OpenVRConnected == null)
            OpenVRConnected = new VoidEvent();

        if (OpenVRDisconnected == null)
            OpenVRConnected = new VoidEvent();

        if (connectOnStart)
            ConnectToSteam();
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
            OpenVRConnected.Invoke();
        }

    }

    public void DisconnectFromSteam()
    {
        if (OVR.StartedUp)
        {
            OVR.Shutdown();
            connectedToOpenVR = false;
            OpenVRDisconnected.Invoke();
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