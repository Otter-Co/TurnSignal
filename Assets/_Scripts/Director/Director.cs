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
    public int targetWindowWidth = 1280;
    public int targetWindowHeight = 800;
    [Space(10)]
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

    [Header("App Log File Options")]
    public int maxLogSizeInMB = 5;
    public float timeInSecondsBetweenChecks = 10f;

    private int currentFPS = 0;
    private int wantedFPS = 0;

    public WindowController winC;
    public OpenVR_Unity openVR;
    public Client steamClient;

    private bool dashboardOpen = false;
    private bool turnsignalActive = true;

    private float timeSinceLastLogCheck = 0f;

    void Startup()
    {
        EnsureSaneLogsize();
    }

    void OnApplicationQuit()
    {
        if (steamClient != null)
            steamClient.Dispose();
    }

    void Update()
    {
        timeSinceLastLogCheck += Time.deltaTime;
        if (timeSinceLastLogCheck >= timeInSecondsBetweenChecks)
        {
            EnsureSaneLogsize();
            timeSinceLastLogCheck = 0;
        }

        if (winC == null || openVR == null)
        {
            winC = GetComponent<WindowController>();
            openVR = GetComponent<OpenVR_Unity>();

            if (winC == null || openVR == null)
                return;

            SetWindowSize();
            winC.ShowTrayIcon();

            winC = GetComponent<WindowController>();
            openVR = GetComponent<OpenVR_Unity>();

            options = LoadLocalOpts();

            if (options.EnableSteamworks)
            {
                try
                {
                    Facepunch.Steamworks.Config.ForUnity(Application.platform.ToString());
                    steamClient = new Client(appID);

                    var cloudOpts = LoadCloudOpts();
                    if (cloudOpts.timestamp != null && cloudOpts.timestamp > options.timestamp)
                        options = cloudOpts;
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

            menuHandler.SetUIValues(options);
            ApplyOptions(options);

            return;
        }

        if (currentFPS != wantedFPS)
            Application.targetFrameRate = currentFPS = wantedFPS;

        var curOpts = menuHandler.GetUIValues();
        if (!curOpts.Equals(options.cleaned))
            ApplyOptions(curOpts);

        if (options.EnableSteamworks && steamClient == null)
        {
            try
            {
                Facepunch.Steamworks.Config.ForUnity(Application.platform.ToString());
                steamClient = new Client(appID);

                var cloudOpts = LoadCloudOpts();
                if (cloudOpts.timestamp != null && cloudOpts.timestamp > options.timestamp)
                    options = cloudOpts;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        else if (options.EnableSteamworks && steamClient != null)
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

        bool localGood = SaveLocalOpts(opts),
            cloudGood = (opts.EnableSteamworks) ? SaveCloudOpts(opts) : true;

        if (localGood)
            Debug.Log("Succesfully wrote Local Options File.");
        else
            Debug.Log(
                "Error writing Local Options File: \n" +
                GetRootDir() + optionsFilename
            );

        if (opts.EnableSteamworks && cloudGood)
            Debug.Log("Succesfully Wrote Options to SteamCloud.");
        else if (opts.EnableSteamworks)
            Debug.Log("Error writing Options file to SteamCloud.");
        else
            Debug.Log("Skipping Options file SteamCloud Write.");

        options = opts;
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
    // Recursion DOOMSDAY
    public void SetWindowSize(int lvl = 0, int maxLvl = 5)
    {
        if (Screen.width != targetWindowWidth || Screen.height != targetWindowHeight)
            Screen.SetResolution(targetWindowWidth, targetWindowHeight, false);

        if (Screen.width != targetWindowWidth || Screen.height != targetWindowHeight)
        {
            if (lvl < maxLvl)
                SetWindowSize(lvl + 1, maxLvl);
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
            steamClient?.RemoteStorage != null &&
            steamClient.RemoteStorage.IsCloudEnabledForAccount &&
            steamClient.RemoteStorage.IsCloudEnabledForApp &&
            steamClient.RemoteStorage.FileExists(optionsFilename)
        )
            return JsonUtility.FromJson<TurnSignalOptions>(
                steamClient.RemoteStorage.ReadString(optionsFilename)
            );
        else
            return options;
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
    public bool SaveCloudOpts(TurnSignalOptions opts)
    {
        if (rootDirectory == RootDirOpt.DontSave)
            return true;

        if (
            steamClient?.RemoteStorage != null &&
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

