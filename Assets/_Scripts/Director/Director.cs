using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Valve.VR;
using OVRLay;

public partial class Director : MonoBehaviour
{
    public string appKey = "";
    [Space(10)]
    public Overlay_Unity floorOverlay;
    public Overlay_Unity menuOverlay;
    [Space(10)]
    public Floor_Handler floorRig;
    public Menu_Handler menuRig;
    [Space(10)]
    public float floorOverlayHandScale = 0.2f;
    public bool flipSides = false;
    public Unity_Overlay.OverlayTrackedDevice floorOverlayDevice = Unity_Overlay.OverlayTrackedDevice.None;
    [Space(10)]
    public int windowWidth = 800;
    public int windowHeight = 600;
    [Space(10)]
    public int runningFPS = 90;
    public int idleFPS = 5;
    [Space(10)]
    public GameObject hmdO;
    public float floorOverlayHeight = 0f;
    public float floorOverlayFollowSpeed = 1.5f;
    public float floorOverlayFollowSpeedRatio = 1.0f;



    private Steam_Handler steamHandler;
    private Prefs_Handler prefs;
    private WindowController winC;

    private bool twistTied = false;

    private int targetFPS = 0;
    private int lastFps = 0;

    void Start()
    {
        targetFPS = idleFPS;

        steamHandler = GetComponent<Steam_Handler>();

        // Init SteamWorks.net
        if (steamHandler.connectedToSteam)
            Debug.Log("Starting up SteamWorks!");
        else
            Debug.Log("SteamWorks Init Failed!");

        prefs = GetComponent<Prefs_Handler>();
        winC = GetComponent<WindowController>();

        string old_prefsPath = Application.dataPath + "/../";

        string prefsPath = Application.persistentDataPath + "/";
        string prefsFileName = "prefs.json";

        prefs.SetFilePath(prefsPath, prefsFileName);

        if (prefs.checkNeedToMove(old_prefsPath))
            prefs.movePrefsToNewDir(old_prefsPath);

        prefs.Load();
    }

    public void OnApplicationQuit()
    {
        prefs.Save();
    }

    // @TODO - Fix the hell outta this mess
    // Refactor the whole speghetto-mixed app state
    void Update()
    {
        UpdateFPS();
        UpdateTwistTie();

        UpdateFloorOverlayRot();
        UpdateFloorOverlayPos();

        SetWindowSize();
    }

    public void UpdateFPS()
    {
        if (lastFps != targetFPS)
        {
            lastFps = targetFPS;
            Application.targetFrameRate = targetFPS;
        }
    }

    // Ties Opacity to Twist Progress
    public void UpdateTwistTie()
    {
        if (twistTied)
        {
            var oldOpat = prefs.Opacity;
            floorOverlay.settings.Alpha = oldOpat * floorRig.turnProgress;
        }
        else if (floorOverlay.settings.Alpha != prefs.Opacity)
            floorOverlay.settings.Alpha = prefs.Opacity;

    }

    public void UpdateFloorOverlayRot()
    {
        if (hmdO.transform.position.y < floorOverlayHeight || (floorOverlayDevice != Unity_Overlay.OverlayTrackedDevice.None && flipSides))
        {
            var foT = floorOverlay.transform;

            if (foT.eulerAngles.x != 270f)
                foT.eulerAngles = new Vector3(270f, foT.eulerAngles.y, foT.eulerAngles.z);

            if (floorOverlayDevice == Unity_Overlay.OverlayTrackedDevice.None)
                floorRig.reversed = true;
            else if (flipSides)
                floorRig.reversed = false;
            else
                floorRig.reversed = true;
        }
        else
        {
            var foT = floorOverlay.transform;

            if (foT.eulerAngles.x != 90f)
                foT.eulerAngles = new Vector3(90f, foT.eulerAngles.y, foT.eulerAngles.z);

            if (floorRig.reversed)
                floorRig.reversed = false;
        }
    }

    public void UpdateFloorOverlayPos()
    {

        var foP = floorOverlay.transform.position;

        if (!prefs.FollowPlayerHeadset ||
            floorOverlayDevice != Unity_Overlay.OverlayTrackedDevice.None)
        {
            if (foP.x != 0 || foP.z != 0)
                floorOverlay.transform.position = new Vector3(0, floorOverlayHeight, 0);
        }
        else if (prefs.FollowPlayerHeadset)
        {
            var newPos = new Vector3(
                hmdO.transform.position.x,
                floorOverlayHeight,
                hmdO.transform.position.z
            );

            var speed = Time.deltaTime * floorOverlayFollowSpeed;

            floorOverlay.transform.position = foP + ((newPos - foP) * (speed * floorOverlayFollowSpeedRatio));
        }
    }

    // Recursion DOOMSDAY
    public void SetWindowSize(int lvl = 0, int maxLvl = 5)
    {
        if (Screen.width != windowWidth || Screen.height != windowHeight)
            Screen.SetResolution(windowWidth, windowHeight, false);

        if (Screen.width != windowWidth || Screen.height != windowHeight)
        {
            if (lvl < maxLvl)
                SetWindowSize(lvl + 1, maxLvl);
        }
    }

    public void OnSteamVRConnect()
    {
        targetFPS = runningFPS;
    }

    public void OnSteamVRDisconnect()
    {
        if (prefs.StartWithSteamVR)
        {
            // Logging Quit. On Quit.
            Debug.Log("Quitting!");

            Application.Quit();
        }
        else
            targetFPS = idleFPS;
    }

    bool ErrorCheck(EVRApplicationError err)
    {
        bool e = err != EVRApplicationError.None;

        if (e)
            Debug.Log("App Error: " + OVR.Applications.GetApplicationsErrorNameFromEnum(err));

        return e;
    }


    public void OnError(string err)
    {
        Debug.Log(err);
    }
}
