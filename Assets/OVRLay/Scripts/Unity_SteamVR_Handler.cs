using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unity_SteamVR_Handler : MonoBehaviour
{
    public float steamVRPollTime = 0.05f;
    public bool connectedToSteam = false;

    [Space(10)]

    public GameObject hmdObject;
    public bool hmd_rot = true;
    public bool hmd_pos = true;

    public GameObject rightTrackerObj;
    public bool right_rot = true;
    public bool right_pos = true;

    public GameObject leftTrackerObj;
    public bool left_rot = true;
    public bool left_pos = true;

    [Space(10)]

    public bool autoUpdate = true;

    public bool debugLog = true;

    public UnityEvent onSteamVRConnect = new UnityEvent();
    public UnityEvent onSteamVRDisconnect = new UnityEvent();


    private OVR_Handler ovrHandler = OVR_Handler.instance;

    public OVR_Overlay_Handler overlayHandler { get { return ovrHandler.overlayHandler; } }
    public OVR_Pose_Handler poseHandler { get { return ovrHandler.poseHandler; } }

    private float lastSteamVRPollTime = 0f;
    private bool lastDebugLog = false;

    void Start()
    {
        // Will always do a check on start first, then use timer for polling
        lastSteamVRPollTime = steamVRPollTime + 1f;
        ovrHandler.onOpenVRChange += OnOpenVRChange;
    }

    void OnOpenVRChange(bool connected)
    {
        connectedToSteam = connected;

        if (!connected)
        {
            onSteamVRDisconnect.Invoke();
            ovrHandler.ShutDownOpenVR();
        }

    }
    void Update()
    {
        if (autoUpdate)
            UpdateHandler();
    }

    public void UpdateHandler()
    {
        if (!SteamVRStartup())
            return;

        ovrHandler.UpdateAll();

        if (lastDebugLog != debugLog)
        {
            ovrHandler.logDebug = debugLog;
            lastDebugLog = debugLog;
        }

        if (hmdObject)
            poseHandler.SetTransformToTrackedDevice(hmdObject.transform, poseHandler.hmdIndex);

        if (!hmd_pos)
            hmdObject.transform.position = Vector3.zero;

        if (!hmd_rot)
            hmdObject.transform.eulerAngles = Vector3.forward;

        if (poseHandler.rightActive && rightTrackerObj)
        {
            if (!rightTrackerObj.activeSelf)
                rightTrackerObj.SetActive(true);

            poseHandler.SetTransformToTrackedDevice(rightTrackerObj.transform, poseHandler.rightIndex);

            if (!right_pos)
                rightTrackerObj.transform.position = Vector3.zero;

            if (!right_rot)
                rightTrackerObj.transform.eulerAngles = Vector3.forward;
        }
        else if (rightTrackerObj)
            rightTrackerObj.SetActive(false);

        if (poseHandler.leftActive && leftTrackerObj)
        {
            if (!leftTrackerObj.activeSelf)
                leftTrackerObj.SetActive(true);

            poseHandler.SetTransformToTrackedDevice(leftTrackerObj.transform, poseHandler.leftIndex);

            if (!left_pos)
                leftTrackerObj.transform.position = Vector3.zero;

            if (!left_rot)
                leftTrackerObj.transform.eulerAngles = Vector3.forward;
        }
        else if (leftTrackerObj)
            leftTrackerObj.SetActive(false);
    }

    bool SteamVRStartup()
    {
        lastSteamVRPollTime += Time.deltaTime;

        if (ovrHandler.OpenVRConnected)
            return true;
        else if (lastSteamVRPollTime >= steamVRPollTime)
        {
            lastSteamVRPollTime = 0f;

            Debug.Log("Checking to see if SteamVR Is Running...");
            if (System.Diagnostics.Process.GetProcessesByName("vrserver").Length <= 0)
            {
                Debug.Log("VRServer not Running!");
                return false;
            }

            Debug.Log("Starting Up SteamVR Connection...");

            if (!ovrHandler.StartupOpenVR())
            {
                Debug.Log("Connection Failed :( !");
                return false;
            }
            else
            {
                Debug.Log("Connected to SteamVR!");

                onSteamVRConnect.Invoke();

                return true;
            }
        }
        else
            return false;
    }
    void OnApplicationQuit()
    {
        if (ovrHandler.OpenVRConnected)
            ovrHandler.ShutDownOpenVR();
    }
}
