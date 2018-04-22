using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TurnSignal_Prefs_Handler : MonoBehaviour
{
    public TurnSignal_Steam_Handler steamHandler;

    private TurnSignalPrefs prefs = new TurnSignalPrefs();
    private string _filePath = "";
    private string _fileName = "";

    public void SetFilePath(string path, string fileName)
    {
        _filePath = path;
        _fileName = fileName;
    }

    public float Scale
    {
        get
        {
            return prefs.Scale;
        }
        set
        {
            prefs.Scale = value;
            Save();
        }
    }

    public float Opacity
    {
        get
        {
            return prefs.Opacity;
        }
        set
        {
            prefs.Opacity = value;
            Save();
        }
    }

    public float Height
    {
        get
        {
            return prefs.Height;
        }
        set
        {
            prefs.Height = value;
            Save();
        }
    }

    public float FollowSpeed
    {
        get
        {
            return prefs.FollowSpeed;
        }
        set
        {
            value = value > 1f ? 1f : value;
            value = value < 0.1f ? 0.1f : value;

            prefs.FollowSpeed = value;
            Save();
        }
    }

    public int TwistRate
    {
        get
        {
            return prefs.TwistRate;
        }
        set
        {
            prefs.TwistRate = value;
            Save();
        }
    }

    public float TwistRateF
    {
        get
        {
            return (float)TwistRate;
        }
        set
        {
            TwistRate = (int)value;
        }
    }

    public int Petals
    {
        get
        {
            return prefs.PetalCount;
        }
        set
        {
            prefs.PetalCount = value;
            Save();
        }
    }

    public float PetalsF
    {
        get
        {
            return (float)Petals;
        }
        set
        {
            Petals = (int)value;
        }
    }

    public bool StartWithSteamVR
    {
        get
        {
            return prefs.StartWithSteamVR;
        }
        set
        {
            prefs.StartWithSteamVR = value;
            Save();
        }
    }

    public bool EnableSteamWorks
    {
        get
        {
            return prefs.EnableSteamWorks;
        }
        set
        {
            prefs.EnableSteamWorks = value;
            Save();
        }
    }

    public bool FollowPlayerHeadset
    {
        get
        {

            return prefs.FollowPlayerHeadset;
        }
        set
        {
            prefs.FollowPlayerHeadset = value;
            Save();
        }
    }

    public bool HideMainWindow
    {
        get
        {
            return prefs.HideMainWindow;
        }
        set
        {
            prefs.HideMainWindow = value;
            Save();
        }
    }


    public bool UseChaperoneColor
    {
        get
        {
            return prefs.UseChaperoneColor;
        }
        set
        {
            prefs.UseChaperoneColor = value;
            Save();
        }
    }

    public bool LinkOpacityWithTwist
    {
        get
        {
            return prefs.LinkOpacityWithTwist;
        }
        set
        {
            prefs.LinkOpacityWithTwist = value;
            Save();
        }
    }

    public bool OnlyShowInDashboard
    {
        get
        {
            return prefs.OnlyShowInDashboard;
        }
        set
        {
            prefs.OnlyShowInDashboard = value;
            Save();
        }
    }

    public TurnSignalPrefsLinkDevice LinkDevice 
    {
        get
        {
            return prefs.LinkDevice;
        }
        set
        {
            prefs.LinkDevice = value;
            Save();
        }
    }

    public bool FlipSides
    {
        get
        {
            return prefs.FlipSides;
        }
        set
        {
            prefs.FlipSides = value;
            Save();
        }
    }


    public bool Save(bool skipSteam = false, TurnSignalPrefs overrideP = null)
    {
        TurnSignalPrefs p;

        if (overrideP != null)
            p = overrideP;
        else
            p = prefs;

        p.lastEditTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        string text = JsonUtility.ToJson(prefs, true);
        string fullP = _filePath + _fileName;

        Debug.Log("Writing Local Prefs!");

        File.WriteAllText(fullP, text);

        if (!skipSteam && prefs.EnableSteamWorks)
            if (SteamSave())
                Debug.Log("Successfully Saved SteamCloud Prefs!");
            else
                Debug.Log("Failed to save SteamCloud Prefs!");

        return File.Exists(fullP);
    }


    public bool SteamSave()
    {
        if (steamHandler.StartUp())
        {
            Debug.Log("Writing SteamCloud Prefs!");

            var c = steamHandler.steamClient;
            if (c.RemoteStorage.IsCloudEnabledForAccount)
                return c.RemoteStorage.WriteString(_fileName, JsonUtility.ToJson(prefs, true));
        }

        return false;
    }

    public void Load()
    {
        TurnSignalPrefs p = new TurnSignalPrefs();
        TurnSignalPrefs fileP = FileLoad();

        if (fileP != null)
        {
            p = fileP;

            if (fileP.EnableSteamWorks)
            {
                var steamP = SteamLoad();

                if (steamP != null && steamP.lastEditTime >= fileP.lastEditTime)
                    p = steamP;
            }
        }
        else
        {
            var steamP = SteamLoad();
            if (steamP != null)
                p = steamP;
        }

        prefs = p;
        Save();
    }

    public TurnSignalPrefs FileLoad()
    {
        string fullP = _filePath + _fileName;

        Debug.Log("Reading Local Prefs!");

        if (!File.Exists(fullP))
            return null;

        string text = File.ReadAllText(fullP);

        return (TurnSignalPrefs)JsonUtility.FromJson(text, typeof(TurnSignalPrefs));
    }

    public TurnSignalPrefs SteamLoad()
    {
        if (steamHandler.StartUp())
        {
            var c = steamHandler.steamClient;

            Debug.Log("Reading SteamCloud Prefs!");

            if (c.RemoteStorage.IsCloudEnabledForAccount && c.RemoteStorage.FileExists(_fileName))
                return (TurnSignalPrefs)JsonUtility.FromJson(c.RemoteStorage.ReadString(_fileName), typeof(TurnSignalPrefs));
        }

        return null;
    }


    public void Reset()
    {
        prefs = new TurnSignalPrefs();
        Save();
        Load();
    }
}

// #enterpriseNames
public enum TurnSignalPrefsLinkDevice 
{
    None,
    Right,
    Left
}

[System.Serializable] public class TurnSignalPrefs 
{
    public Int32 lastEditTime = 0;

    public float Scale = 2f;
    public float Opacity = 0.03f;
    public float Height = 0f;
    public float FollowSpeed = 1f;

    public int TwistRate = 10;
    public int PetalCount = 6;

    public bool StartWithSteamVR = true;
    public bool EnableSteamWorks = true;
    public bool HideMainWindow = false;

    public bool FollowPlayerHeadset = false;
    public bool UseChaperoneColor = false;
    public bool LinkOpacityWithTwist = false;

    public bool OnlyShowInDashboard = false;

    public TurnSignalPrefsLinkDevice LinkDevice = TurnSignalPrefsLinkDevice.None;
    public bool FlipSides = false;
}
