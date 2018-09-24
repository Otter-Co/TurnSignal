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
            ConnectToOpenVR();
    }

    void Update()
    {
        if (!OVR.StartedUp && tryUntilConnect)
        {
            if (timeSinceLastTry < timeBetweenTries)
                timeSinceLastTry += Time.deltaTime;
            else
            {
                ConnectToOpenVR();
                timeSinceLastTry = 0f;
            }

            return;
        }

        if (autoUpdatePoses)
            UpdatePoses();

        if (pollForEvents)
            PollForEvents();
    }

    public void ConnectToOpenVR()
    {
        if (!OVR.StartedUp && OVR.Startup(appType))
        {
            connectedToOpenVR = true;
            OpenVRConnected.Invoke();
        }

    }

    public void DisconnectFromOpenVR()
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

    public Color GetChaperoneColor()
    {
        Color ret = new Color(1, 1, 1, 1);

        if (OVR.Settings == null)
            return ret;

        var collSec = OpenVR.k_pch_CollisionBounds_Section;
        var error = EVRSettingsError.None;

        int r = 255, g = 255, b = 255, a = 255;
        r = OVR.Settings.GetInt32(collSec, OpenVR.k_pch_CollisionBounds_ColorGammaR_Int32, ref error);
        g = OVR.Settings.GetInt32(collSec, OpenVR.k_pch_CollisionBounds_ColorGammaG_Int32, ref error);
        b = OVR.Settings.GetInt32(collSec, OpenVR.k_pch_CollisionBounds_ColorGammaB_Int32, ref error);
        a = OVR.Settings.GetInt32(collSec, OpenVR.k_pch_CollisionBounds_ColorGammaA_Int32, ref error);

        ret.r = (float)r / 255;
        ret.g = (float)g / 255;
        ret.b = (float)b / 255;
        ret.a = (float)a / 255;

        return ret;
    }
}