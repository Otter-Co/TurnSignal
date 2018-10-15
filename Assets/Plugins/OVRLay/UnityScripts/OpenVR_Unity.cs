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
            if (timeSinceLastTry >= timeBetweenTries)
            {
                ConnectToOpenVR();
                timeSinceLastTry = 0f;
            }
            else
                timeSinceLastTry += Time.deltaTime;
        }
        else if (OVR.StartedUp)
        {
            OVR.UpdateEvents();
            OVRLay.Pose.UpdatePoses();
        }
    }

    void OnApplicationQuit()
    {
        if (connectedToOpenVR)
            DisconnectFromOpenVR();
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

    private Color _chapColor = Color.white;
    private EVRSettingsError _chapColorError = EVRSettingsError.None;
    public Color GetChaperoneColor()
    {
        if (OVR.Settings == null)
            return _chapColor;

        _chapColor.r = (int)(OVR.Settings.GetInt32(OpenVR.k_pch_CollisionBounds_Section, OpenVR.k_pch_CollisionBounds_ColorGammaR_Int32, ref _chapColorError) / 255);
        _chapColor.g = (OVR.Settings.GetInt32(OpenVR.k_pch_CollisionBounds_Section, OpenVR.k_pch_CollisionBounds_ColorGammaG_Int32, ref _chapColorError) / 255);
        _chapColor.b = (OVR.Settings.GetInt32(OpenVR.k_pch_CollisionBounds_Section, OpenVR.k_pch_CollisionBounds_ColorGammaB_Int32, ref _chapColorError) / 255);
        _chapColor.a = (OVR.Settings.GetInt32(OpenVR.k_pch_CollisionBounds_Section, OpenVR.k_pch_CollisionBounds_ColorGammaA_Int32, ref _chapColorError) / 255);

        return _chapColor;
    }
}