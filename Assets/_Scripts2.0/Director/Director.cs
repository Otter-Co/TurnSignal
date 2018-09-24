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


    [Header("Object Refs")]
    public Menu_Handler menuHandler;
    public Overlay_Unity menuOverlay;
    [Space(10)]
    public Floor_Handler floorHandler;
    public Overlay_Unity floorOverlay;
    public Twister twister;

    [Header("Steamworks Settings")]
    public uint appID = 0;

    [Header("App Options")]
    public TurnSignalOptions options =
        TurnSignalOptions.DefaultOptions;

    [Header("App Options File Settings")]
    public RootDirOpt rootDirectory = RootDirOpt.UserDir;
    public string optionsFilename = "turnsignal_opts.json";

    [Header("App Internal Settings")]
    public float handLinkSizeMultiplier = 0.2f;

    private WindowController winC;
    private OpenVR_Unity openVR;
    private Client steamClient;

    private bool dashboardOpen = false;
    private bool windowShowing = true;
    private bool turnsignalActive = true;
    void Start()
    {
        Facepunch.Steamworks.Config.ForUnity(Application.platform.ToString());

        winC = GetComponent<WindowController>();
        openVR = GetComponent<OpenVR_Unity>();
        options = LoadLocalOpts();

        menuHandler.SetUIValues(options);

        winC.ShowTaskbarIcon();

        if (options.HideMainWindow)
            winC.HideUnityWindow();

        if (options.EnableSteamworks)
        {
            steamClient = new Client(appID);

            var cloudOpts = LoadCloudOpts();

            if (cloudOpts.timestamp > options.timestamp)
                options = cloudOpts;
        }
    }

    void Update()
    {
        var curOpts = menuHandler.GetUIValues();
        if (!curOpts.Equals(options.cleaned))
            ApplyOptions(curOpts);

        if (options.EnableSteamworks && steamClient == null)
            steamClient = new Client(appID);
        else if (options.EnableSteamworks && steamClient != null)
            steamClient.Update();
        else if (!options.EnableSteamworks && steamClient != null)
        {
            steamClient.Dispose();
            steamClient = null;
        }

        if (options.HideMainWindow && windowShowing)
        {
            winC.HideTaskbarIcon();
            winC.HideUnityWindow();
            windowShowing = false;
        }
        else if (!options.HideMainWindow && !windowShowing)
        {
            winC.ShowTaskbarIcon();
            winC.ShowUnityWindow();
            windowShowing = true;
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

    public void ApplyOptions(TurnSignalOptions opts)
    {
        ApplyTransformOptions(opts);
        ApplyVisualOptions(opts);

        bool localGood = SaveLocalOpts(opts),
            cloudGood = (opts.EnableSteamworks) ? SaveCloudOpts(opts) : true;

        options = opts;
    }

    void ApplyVisualOptions(TurnSignalOptions opts)
    {
        if (floorOverlay.settings.WidthInMeters != opts.Scale)
            floorOverlay.settings.WidthInMeters = opts.Scale;

        if (opts.TwistRate != floorHandler.maxTurns)
            floorHandler.maxTurns = (int)opts.TwistRate;

        if (opts.PetalCount != opts.PetalCount)
        {
            // DoSomething();
        }

        if (opts.UseChaperoneColor)
        {
            var chapColor = openVR.GetChaperoneColor();
            if (floorOverlay.settings.Color != chapColor)
                floorOverlay.settings.Color = openVR.GetChaperoneColor();
        }
    }

    void ApplyTransformOptions(TurnSignalOptions opts)
    {
        var oT = floorOverlay.GetComponent<Overlay_Transform>();
        var fT = floorOverlay.transform;

        if (opts.LinkOptions == TurnSignalLinkOpts.None)
        {
            if (oT.transformType != Valve.VR.VROverlayTransformType.VROverlayTransform_Absolute)
                oT.transformType = Valve.VR.VROverlayTransformType.VROverlayTransform_Absolute;

            if (fT.position.y != opts.Height)
            {
                var curOPos = fT.position;
                curOPos.y = opts.Height;
                fT.position = curOPos;
            }
        }
        else
        {
            if (oT.transformType != Valve.VR.VROverlayTransformType.VROverlayTransform_TrackedDeviceRelative)
                oT.transformType = Valve.VR.VROverlayTransformType.VROverlayTransform_TrackedDeviceRelative;

            OpenVR_DeviceTracker.DeviceType properIndex = 0;

            if ((opts.LinkOptions & TurnSignalLinkOpts.RightFront) > 0)
                properIndex = OpenVR_DeviceTracker.DeviceType.RightController;
            else if ((opts.LinkOptions & TurnSignalLinkOpts.LeftFront) > 0)
                properIndex = OpenVR_DeviceTracker.DeviceType.LeftController;

            if (oT.relativeDevice != properIndex)
                oT.relativeDevice = properIndex;

            if (floorOverlay.settings.WidthInMeters != (opts.Scale * handLinkSizeMultiplier))
                floorOverlay.settings.WidthInMeters = (opts.Scale * handLinkSizeMultiplier);

            float adjHeight = (opts.Height * handLinkSizeMultiplier) * (
                (opts.LinkOptions & TurnSignalLinkOpts.Old_FlipSides) > 0 ? -1 : 1
            );

            if (fT.position.y != adjHeight)
            {
                var curOPos = fT.position;
                curOPos.y = adjHeight;
                fT.position = curOPos;
            }

            float curXRot = (opts.LinkOptions & TurnSignalLinkOpts.Old_FlipSides) > 0
                ? 270f
                : 90f;

            if (fT.eulerAngles.x != 270f)
            {
                var curRot = fT.eulerAngles;
                curRot.x = curXRot;
                fT.eulerAngles = curRot;
            }
        }
    }

    void UpdateFloorOverlay()
    {
        var fT = floorOverlay.transform;
        var hmdP = OVRLay.Pose.GetDevicePosition(OVRLay.Pose.HmdPoseIndex);
        var middleVec = new Vector3(0, options.Height, 0);

        if (!options.LinkOpatWithTwist && floorOverlay.settings.Alpha != options.Opacity)
            floorOverlay.settings.Alpha = options.Opacity;
        else if (options.LinkOpatWithTwist)
            floorOverlay.settings.Alpha = options.Opacity * Mathf.Abs(floorHandler.turnProgress);

        if (options.LinkOptions == TurnSignalLinkOpts.None)
        {
            var properRot = fT.position.y > hmdP.y
                ? 270f
                : 90f;

            if (fT.eulerAngles.x != properRot)
            {
                var curRot = fT.eulerAngles;
                curRot.x = properRot;
                fT.eulerAngles = curRot;
            }

            floorHandler.reversed = (fT.position.y > hmdP.y);

            if (options.FollowPlayerHeadeset)
            {
                var newPos = new Vector3(
                    hmdP.x,
                    options.Height,
                    hmdP.z
                );

                var speed = Time.deltaTime * options.FollowSpeed;

                fT.position = fT.position - ((newPos - fT.position) * (speed));
            }
            else if (fT.position != middleVec)
                fT.position = middleVec;
        }
        else
        {
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

