using System;
using Facepunch.Steamworks;
using UnityEngine;

public class Director : MonoBehaviour
{
    [Header("App Options")]
    public TurnSignalOptions options =
        TurnSignalOptions.DefaultOptions;

    [Header("App Option File Settings")]
    public RootDirOpt rootDirectory = RootDirOpt.UserDir;
    public string baseFilename = "turnsignal_opts.json";
    [Header("Steamworks Settings")]
    public uint appID = 0;
    public bool disableSteamWorks = false;


    private Options_Handler optHandler = null;
    private Client steamClient;

    void Start()
    {
        optHandler = GetComponent<Options_Handler>();
    }

    void Update()
    {

    }

    void SteamworksStartup()
    {
        if (steamClient != null)
            return;

        Facepunch.Steamworks.Config.ForUnity(Application.platform.ToString());
        steamClient = new Client(appID);
    }

    void SteamworksUpdate()
    {
        if (steamClient != null)
            steamClient.Update();
    }
    void SteamworksShutdown()
    {
        steamClient.Dispose();
        steamClient = null;
    }

    public bool LoadOptions(bool checkCloud = true)
    {
        return true;
    }
    public bool SaveOptions(bool toCloud = true)
    {
        return true;
    }
}

public enum RootDirOpt
{
    DontSave,
    AppDir,
    UserDir,
    TempDir,
}