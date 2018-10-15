using System;
using System.IO;
using UnityEngine;
using Facepunch.Steamworks;
using System.Collections.Generic;

public class Director : MonoBehaviour
{
    public void ResetRotation() => floorHandler.currentTurnValue = 0f;
    public void ResetOptions() => menuHandler.SetUIValues(TurnSignalOptions.DefaultOptions);
    public void ToggleEnabled() => turnsignalActive = !turnsignalActive;

    public void SetDashboardState(bool state) => dashboardOpen = state;

    public void SetIdleMode() => targetFPS = idleFPS;
    public void SetActiceMode() => targetFPS = activeFPS;

    public void ToggleMode(bool active)
    {
        if (active)
            SetActiceMode();
        else
            SetIdleMode();
    }

    public void ToggleShowWindow() =>
        menuHandler.hideMainWindowToggle.isOn = !menuHandler.hideMainWindowToggle.isOn;
    public void QuitApp() => Application.Quit();


    [Header("Object Refs")]
    public Menu_Handler menuHandler;
    public Overlay_Unity menuOverlay;
    [Space(10)]
    public Floor_Handler floorHandler;
    public Overlay_Unity floorOverlay;
    public Twister twister;

    [Header("App Internal Settings")]
    public int activeFPS = 90;
    public int idleFPS = 5;
    [Space(10)]
    public float handLinkSizeMultiplier = 0.2f;

    [Header("Steamworks Settings")]
    public uint appID = 0;
    public string pchAppKey = "TurnSignal";
    public bool disableSteamworksInEditor = true;

    [Header("App Options")]
    public TurnSignalOptions options =
        TurnSignalOptions.DefaultOptions;

    [Header("App Options File Settings")]
    public RootDirOpt rootDirectory = RootDirOpt.UserDir;
    public string optionsFilename = "turnsignal_opts.json";

    [Header("App Log File Options")]
    public int maxLogSizeInMB = 5;
    public float timeInSecondsBetweenChecks = 10f;

    private int targetFPS = 0;
    private int currentFPS = 0;

    private Window_Handler winH;
    private OpenVR_Unity openVR;
    private List<string> startupArgs;

    private bool dashboardOpen = false;
    private bool turnsignalActive = true;
    private float timeSinceLastLogCheck = 0f;

    private bool steamConfigured = false;

    void Start()
    {
        winH = GetComponent<Window_Handler>();
        openVR = GetComponent<OpenVR_Unity>();

        startupArgs = new List<string>(System.Environment.GetCommandLineArgs());

        ApplyOptions(LoadLocalOpts());

        if (disableSteamworksInEditor && Application.isEditor)
            options.EnableSteamworks = false;

        if (!options.EnableSteamworks && startupArgs.Find(arg => arg.ToLower().Equals("--steam")) != null)
        {
            winH.CloseAndRestartApp();
            return;
        }
        else if (options.EnableSteamworks)
        {
            var cloudOpts = LoadCloudOpts();

            if (cloudOpts.timestamp != null && cloudOpts.timestamp > options.timestamp)
                ApplyOptions(cloudOpts);
        }
    }

    void Update()
    {
        timeSinceLastLogCheck += Time.deltaTime;
        if (timeSinceLastLogCheck >= timeInSecondsBetweenChecks)
        {
            EnsureSaneLogsize();
            timeSinceLastLogCheck = 0;
        }

        if (currentFPS != targetFPS)
            Application.targetFrameRate = currentFPS = targetFPS;

        var curOpts = menuHandler.GetUIValues();
        if (!curOpts.cleaned.Equals(options.cleaned))
            ApplyOptions(curOpts);

        if (!options.ShowMainWindow && winH.WindowVisible)
            winH.HideWindow();
        else if (options.ShowMainWindow && !winH.WindowVisible)
            winH.ShowWindow();

        if (turnsignalActive)
        {
            if (options.OnlyShowInDashboard)
                floorOverlay.enabled = dashboardOpen;
            else if (!floorOverlay.enabled)
                floorOverlay.enabled = true;

            UpdateFloorOverlay();
        }
        else if (floorOverlay.enabled)
            floorOverlay.enabled = false;
    }

    void UpdateFloorOverlay()
    {
        if (openVR != null && !openVR.connectedToOpenVR)
            return;

        var fT = floorOverlay.transform;
        var hmdP = OVRLay.Pose.GetDevicePosition(OVRLay.Pose.HmdPoseIndex);
        var middleVec = new Vector3(0, options.Height, 0);

        var properRot = (
                (options.LinkOptions & TurnSignalLinkOpts.Old_FlipSides) > 0 ||
                (options.LinkOptions == TurnSignalLinkOpts.None && fT.position.y > hmdP.y)
            ) ? 270f : 90f;

        if (fT.eulerAngles.x != properRot)
            fT.eulerAngles = new Vector3(properRot, fT.eulerAngles.y, fT.eulerAngles.z);

        if (!options.LinkOpatWithTwist && floorOverlay.settings.Alpha != options.Opacity)
            floorOverlay.settings.Alpha = options.Opacity;
        else if (options.LinkOpatWithTwist)
            floorOverlay.settings.Alpha = options.Opacity * Mathf.Abs(floorHandler.turnProgress);

        if (options.LinkOptions == TurnSignalLinkOpts.None)
        {
            floorHandler.reversed = (fT.position.y > hmdP.y);

            if (options.FollowPlayerHeadeset)
                fT.position = Vector3.Lerp(
                    fT.position,
                    new Vector3(hmdP.x, options.Height, hmdP.z),
                    (Time.deltaTime * options.FollowSpeed)
                );
            else if (fT.position != middleVec)
                fT.position = middleVec;
        }
        else
        {
            middleVec.y = (options.Height * handLinkSizeMultiplier) * (
                (options.LinkOptions & TurnSignalLinkOpts.Old_FlipSides) > 0 ? -1 : 1
            );

            if (fT.position != middleVec)
                fT.position = middleVec;
        }
    }

    void ApplyOptions(TurnSignalOptions opts)
    {
        if (openVR != null && openVR.connectedToOpenVR)
        {
            OVRLay.OVR.Applications.SetApplicationAutoLaunch(
                        pchAppKey,
                        opts.StartWithSteamVR
                    );

            floorHandler.maxTurns = (int)opts.TwistRate;

            if ((int)opts.PetalCount != twister.petalCount)
                twister.petalCount = opts.PetalCount > 0 ? (int)(opts.PetalCount) : twister.petalCount;

            if (opts.UseChaperoneColor)
                floorOverlay.settings.Color = openVR.GetChaperoneColor();
            else
                floorOverlay.settings.Color = UnityEngine.Color.white;

            var oT = floorOverlay.GetComponent<Overlay_Transform>();
            var fT = floorOverlay.transform;

            if (opts.LinkOptions == TurnSignalLinkOpts.None)
            {
                floorOverlay.settings.WidthInMeters = opts.Scale;
                oT.transformType = Valve.VR.VROverlayTransformType.VROverlayTransform_Absolute;
            }
            else
            {
                OpenVR_DeviceTracker.DeviceType properIndex = 0;

                if ((opts.LinkOptions & TurnSignalLinkOpts.RightFront) > 0)
                    properIndex = OpenVR_DeviceTracker.DeviceType.RightController;
                else if ((opts.LinkOptions & TurnSignalLinkOpts.LeftFront) > 0)
                    properIndex = OpenVR_DeviceTracker.DeviceType.LeftController;

                floorOverlay.settings.WidthInMeters = (opts.Scale * handLinkSizeMultiplier);
                oT.relativeDevice = properIndex;
                oT.transformType = Valve.VR.VROverlayTransformType.VROverlayTransform_TrackedDeviceRelative;
            }
        }

        opts.timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        bool localGood = SaveLocalOpts(opts);

        if (localGood)
            Debug.Log("Succesfully wrote Local Options to File: \n" + GetRootDir() + optionsFilename);
        else
            Debug.Log("Error writing Local Options to File: \n" + GetRootDir() + optionsFilename);

        bool cloudGood = (opts.EnableSteamworks) ? SaveCloudOpts(opts) : true;

        if (opts.EnableSteamworks && cloudGood)
            Debug.Log("Succesfully Wrote Options to SteamCloud.");
        else if (opts.EnableSteamworks)
            Debug.Log("Error writing Options file to SteamCloud.");
        else
            Debug.Log("Skipping Options file SteamCloud Write.");

        options = opts;
        menuHandler.SetUIValues(opts);
    }

    public enum RootDirOpt
    {
        DontSave,
        AppDir,
        UserDir,
        TempDir,
    }

    string GetRootDir()
    {
        switch (rootDirectory)
        {
            case RootDirOpt.AppDir:
                return Application.dataPath + "/";
            case RootDirOpt.UserDir:
                return Application.persistentDataPath + "/";
            case RootDirOpt.TempDir:
            case RootDirOpt.DontSave:
            default:
                return "./";
        }
    }
    public TurnSignalOptions LoadLocalOpts()
    {
        if (rootDirectory == RootDirOpt.DontSave)
            return TurnSignalOptions.DefaultOptions;

        string fullPath = GetRootDir() + optionsFilename;

        if (File.Exists(fullPath))
            return JsonUtility.FromJson<TurnSignalOptions>(File.ReadAllText(fullPath));
        else
        {
            Debug.Log("Could not find Preferences File!");
            return TurnSignalOptions.DefaultOptions;
        }

    }

    public bool SaveLocalOpts(TurnSignalOptions opts)
    {
        if (rootDirectory == RootDirOpt.DontSave)
            return true;

        string fullPath = GetRootDir() + optionsFilename;

        File.Delete(fullPath);
        if (File.Exists(fullPath))
            return false;
        File.WriteAllText(fullPath, JsonUtility.ToJson(opts));

        return (File.Exists(fullPath));
    }

    public TurnSignalOptions LoadCloudOpts()
    {
        if (rootDirectory == RootDirOpt.DontSave)
            return options;

        if (!steamConfigured)
        {
            Facepunch.Steamworks.Config.ForUnity(Application.platform.ToString());
            steamConfigured = true;
        }

        using (Client steam = new Client(appID))
        {
            try
            {
                if (
                    steam?.RemoteStorage != null &&
                    steam.RemoteStorage.IsCloudEnabledForAccount &&
                    steam.RemoteStorage.IsCloudEnabledForApp &&
                    steam.RemoteStorage.FileExists(optionsFilename)
                )
                    return JsonUtility.FromJson<TurnSignalOptions>(
                        steam.RemoteStorage.ReadString(optionsFilename)
                    );
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        return options;
    }

    public bool SaveCloudOpts(TurnSignalOptions opts)
    {
        if (rootDirectory == RootDirOpt.DontSave)
            return true;

        if (!steamConfigured)
        {
            Facepunch.Steamworks.Config.ForUnity(Application.platform.ToString());
            steamConfigured = true;
        }

        using (Client steam = new Client(appID))
        {
            try
            {
                if (
                steam?.RemoteStorage != null &&
                steam.RemoteStorage.IsCloudEnabledForAccount &&
                steam.RemoteStorage.IsCloudEnabledForApp
                )
                {
                    if (!steam.RemoteStorage.FileExists(optionsFilename))
                        steam.RemoteStorage.CreateFile(optionsFilename);

                    return steam.RemoteStorage.WriteString(optionsFilename, JsonUtility.ToJson(opts));
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        return false;
    }

    private const string _log_filename = "output_log.txt";
    public void EnsureSaneLogsize()
    {
        int maxLogSize = 1024 * 1024 * maxLogSizeInMB;
        string targetFilePath = Application.persistentDataPath + "/" + _log_filename;
        try
        {
            if (
                File.Exists(targetFilePath)
            )
            {
                FileInfo fileI = new FileInfo(targetFilePath);

                if (fileI.Length > maxLogSize)
                {
                    File.Delete(targetFilePath);
                    Debug.Log("Deleted Insanly sized Log file. Sorry if this is an issue :( !");
                }
                else
                    Debug.Log("Just checked, Log file size still sane :) !");

                fileI = null;
            }
        }
        catch (Exception err)
        {
            Debug.Log(err);
        }
    }


}

