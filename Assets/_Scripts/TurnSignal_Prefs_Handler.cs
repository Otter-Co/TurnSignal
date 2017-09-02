using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Steamworks;

public class TurnSignal_Prefs_Handler : MonoBehaviour 
{   
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
            return (float) TwistRate;
        }
        set 
        {
            TwistRate = (int) value;
        }
    }

    private int _Petals = 6;
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
            return (float) Petals;
        }
        set 
        {
            Petals = (int) value;
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

    public bool Save()
    {
        string text = JsonUtility.ToJson(prefs);

        if(SteamManager.Initialized && SteamRemoteStorage.IsCloudEnabledForAccount())
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(text);
            var byteCount = System.Text.Encoding.ASCII.GetByteCount(text);

            bool result = SteamRemoteStorage.FileWrite(_fileName, bytes, byteCount);

            Debug.Log("Writing Prefs to SteamCloud!");
        }

        string fullP = _filePath + _fileName;

        File.WriteAllText(fullP, text);
        return File.Exists(fullP);
    }

    public bool Load()
    {
        string text = "";
        if(SteamManager.Initialized && SteamRemoteStorage.IsCloudEnabledForAccount())
        {
            if(SteamRemoteStorage.FileExists(_fileName))
            {
                var byteCount = SteamRemoteStorage.GetFileSize(_fileName);
                var bytes = new byte[byteCount];

                var fileC = SteamRemoteStorage.FileRead(_fileName, bytes, byteCount);

                if(fileC > 0)
               text = System.Text.Encoding.ASCII.GetString(bytes);

                Debug.Log("Reading Prefs from SteamCloud!");
            }
        }
        else if(File.Exists(_filePath))
        {
            text = File.ReadAllText(_filePath);
        }
    
        var o = (TurnSignalPrefs) JsonUtility.FromJson(text, typeof(TurnSignalPrefs));
        if(o != null)
        {
            prefs = o;
            return true;
        }
        else
        {
            prefs = new TurnSignalPrefs();
            return false;
        }
    }
    public void Reset()
    {
        prefs = new TurnSignalPrefs();
        Save();
        Load();
    }
}

[System.Serializable]
public class TurnSignalPrefs 
{
    public float Scale = 2f;
    public float Opacity = 0.03f;
    public float Height = 0f;
    public int TwistRate = 10;
    public int PetalCount = 6;
    public bool StartWithSteamVR = true;
    public bool UseChaperoneColor = false;
    public bool LinkOpacityWithTwist = false;
    public bool OnlyShowInDashboard = false;
}
