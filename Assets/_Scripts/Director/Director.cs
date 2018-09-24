using System;
using System.IO;
using UnityEngine;
using Facepunch.Steamworks;
public class Director : MonoBehaviour
{
    public void ResetRotation() => floorHandler.currentTurnValue = 0f;
    public void ResetOptions() => menuHandler.SetUIValues(TurnSignalOptions.DefaultOptions);
    public void ToggleEnabled() => turnsignalActive = !turnsignalActive;

    public void SetDashboardState(bool state) => dashboardOpen = state;

    public void SetIdleMode() => wantedFPS = idleFPS;
    public void SetActiceMode() => wantedFPS = activeFPS;

    public void ShowWindow() => menuHandler.hideMainWindowToggle.isOn = false;
    public void QuitApp() => Application.Quit();


    [Header("Object Refs")]
    public Menu_Handler menuHandler;
    public Overlay_Unity menuOverlay;
    public Camera menuCamera;
    [Space(10)]
    public Floor_Handler floorHandler;
    public Overlay_Unity floorOverlay;
    public Camera floorCamera;
    public Twister twister;

    [Header("App Internal Settings")]
    public int activeFPS = 90;
    public int idleFPS = 5;
    [Space(10)]
    public float handLinkSizeMultiplier = 0.2f;

    [Header("Steamworks Settings")]
    public uint appID = 0;
    public string pchAppKey = "TurnSignal";

    [Header("App Options")]
    public TurnSignalOptions options =
        TurnSignalOptions.DefaultOptions;

    [Header("App Options File Settings")]
    public RootDirOpt rootDirectory = RootDirOpt.UserDir;
    public string optionsFilename = "turnsignal_opts.json";

    public int currentFPS = 0;
    public int wantedFPS = 0;

    private WindowController winC;
    private OpenVR_Unity openVR;
    private Client steamClient;

    private bool dashboardOpen = false;
    private bool turnsignalActive = true;

    void Start()
    {
        Facepunch.Steamworks.Config.ForUnity(Application.platform.ToString());

        winC = GetComponent<WindowController>();
        openVR = GetComponent<OpenVR_Unity>();
        options = LoadLocalOpts();

        winC.ShowTrayIcon();

        if (options.EnableSteamworks)
        {
            steamClient = new Client(appID);

            var cloudOpts = LoadCloudOpts();
            if (cloudOpts.timestamp > options.timestamp)
                options = cloudOpts;
        }

        menuHandler.SetUIValues(options);
        ApplyOptions(options);
    }

    void OnApplicationQuit()
    {
        SaveLocalOpts(options);
        if (options.EnableSteamworks)
            SaveCloudOpts(options);

        steamClient.Dispose();
        openVR.DisconnectFromOpenVR();
    }

    void Update()
    {
        if (currentFPS != wantedFPS)
            Application.targetFrameRate = currentFPS = wantedFPS;

        var curOpts = menuHandler.GetUIValues();
        if (!curOpts.Equals(options.cleaned))
            ApplyOptions(curOpts);

        if (options.EnableSteamworks && steamClient == null)
            steamClient = new Client(appID);
        if (options.EnableSteamworks && steamClient != null)
            steamClient.Update();
        else if (!options.EnableSteamworks && steamClient != null)
        {
            steamClient.Dispose();
            steamClient = null;
        }

        if (options.HideMainWindow && winC.windowVisible)
        {
            winC.HideTaskbarIcon();
            winC.HideUnityWindow();
            menuCamera.enabled = false;
        }
        else if (!options.HideMainWindow && !winC.windowVisible)
        {
            winC.ShowTaskbarIcon();
            winC.ShowUnityWindow();
            menuCamera.enabled = true;
        }

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

    void ApplyOptions(TurnSignalOptions opts)
    {
        if (openVR.connectedToOpenVR)
        {
            OVRLay.OVR.Applications.SetApplicationAutoLaunch(
                        pchAppKey,
                        opts.StartWithSteamVR
                    );

            floorHandler.maxTurns = (int)opts.TwistRate;

            if ((int)opts.PetalCount != twister.petalCount)
                twister.petalCount = (int)opts.PetalCount;

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

        bool localGood = SaveLocalOpts(opts),
            cloudGood = (opts.EnableSteamworks) ? SaveCloudOpts(opts) : true;

        options = opts;
    }

    void UpdateFloorOverlay()
    {
        if (!openVR.connectedToOpenVR)
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
        string fullPath = GetRootDir() + optionsFilename;

        if (File.Exists(fullPath))
            return JsonUtility.FromJson<TurnSignalOptions>(File.ReadAllText(fullPath));
        else
            return TurnSignalOptions.DefaultOptions;
    }
    public TurnSignalOptions LoadCloudOpts()
    {
        if (
            steamClient != null &&
            steamClient.RemoteStorage.IsCloudEnabledForAccount &&
            steamClient.RemoteStorage.IsCloudEnabledForApp &&
            steamClient.RemoteStorage.FileExists(optionsFilename)
        )
            return JsonUtility.FromJson<TurnSignalOptions>(
                steamClient.RemoteStorage.ReadString(optionsFilename)
            );
        else
            return TurnSignalOptions.DefaultOptions;
    }
    public bool SaveLocalOpts(TurnSignalOptions opts)
    {
        if (rootDirectory == RootDirOpt.DontSave)
            return true;

        string fullPath = GetRootDir() + optionsFilename;

        opts.timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        File.WriteAllText(fullPath, JsonUtility.ToJson(opts));

        return (
            File.Exists(fullPath) &&
            LoadLocalOpts().timestamp == opts.timestamp
        );
    }
    public bool SaveCloudOpts(TurnSignalOptions opts)
    {
        if (rootDirectory == RootDirOpt.DontSave)
            return true;

        opts.timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        if (
            steamClient != null &&
            steamClient.RemoteStorage.IsCloudEnabledForAccount &&
            steamClient.RemoteStorage.IsCloudEnabledForApp
        )
        {
            if (!steamClient.RemoteStorage.FileExists(optionsFilename))
                steamClient.RemoteStorage.CreateFile(optionsFilename);

            return steamClient.RemoteStorage.WriteString(optionsFilename, JsonUtility.ToJson(opts));
        }

        return false;
    }
}

