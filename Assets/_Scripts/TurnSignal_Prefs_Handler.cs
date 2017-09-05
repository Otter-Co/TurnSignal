using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

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

    public int LinkDevice 
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

        if(overrideP != null)
            p = overrideP;
        else
            p = prefs;

        p.lastEditTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        string text = JsonUtility.ToJson(prefs, true);
        string fullP = _filePath + _fileName;

        Debug.Log("Writing Local Prefs!");
        File.WriteAllText(fullP, text);

        if(!skipSteam)
            SteamSave();

        return File.Exists(fullP);
    }

    public bool SteamSave() 
    {
        if(SteamManager.Initialized && SteamRemoteStorage.IsCloudEnabledForAccount())
        {
            string text = JsonUtility.ToJson(prefs, true);
            
            var bytes = System.Text.Encoding.ASCII.GetBytes(text);
            var byteCount = System.Text.Encoding.ASCII.GetByteCount(text);

            Debug.Log("Writing Prefs to SteamCloud!");
            return SteamRemoteStorage.FileWrite(_fileName, bytes, byteCount);
        }
        else
            return false;
    }

    public bool Load()
    {
        TurnSignalPrefs fileP = FileLoad();
        TurnSignalPrefs steamP = SteamLoad();

        bool res = false;
        
        bool skipSteam = false;
        TurnSignalPrefs p = new TurnSignalPrefs();

        if(fileP != null && steamP != null) 
        {
            if(fileP.lastEditTime >= steamP.lastEditTime)
                p = fileP;
            else 
            {
                p = steamP;
                skipSteam = true;
            }

            res = true;
        }
        else if(fileP == null && steamP != null)
        {
            p = steamP;
            skipSteam = true;
            res = true;
        }
        else if(fileP != null && steamP == null)
        {
            p = fileP;
            res = true;
        }
        
        prefs = p;
        Save(skipSteam);
        
        return res;
    }

    public TurnSignalPrefs SteamLoad() 
    {
        if(SteamManager.Initialized && SteamRemoteStorage.IsCloudEnabledForAccount())
        {
            if(SteamRemoteStorage.FileExists(_fileName))
            {
                string text = "";
                var byteCount = SteamRemoteStorage.GetFileSize(_fileName);
                var bytes = new byte[byteCount];
                
                Debug.Log("Reading Prefs from SteamCloud!");
                var fileC = SteamRemoteStorage.FileRead(_fileName, bytes, byteCount);

                if(fileC > 0)
                    text = System.Text.Encoding.ASCII.GetString(bytes);
                    
                var o = (TurnSignalPrefs) JsonUtility.FromJson(text, typeof(TurnSignalPrefs));

                if(o != null)
                    return o;        
            }   
        }
        
        return null;
    }

    public TurnSignalPrefs FileLoad()
    {
        string fullP = _filePath + _fileName;
        
        if(!File.Exists(fullP))
            return null;

        Debug.Log("Reading Local Prefs!");
        string text = File.ReadAllText(fullP);

        return (TurnSignalPrefs) JsonUtility.FromJson(text, typeof(TurnSignalPrefs));
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
    public Int32 lastEditTime = 0;

    public float Scale = 2f;
    public float Opacity = 0.03f;
    public float Height = 0f;

    public int TwistRate = 10;
    public int PetalCount = 6;

    public bool StartWithSteamVR = true;
    public bool HideMainWindow = false;
    public bool UseChaperoneColor = false;
    public bool LinkOpacityWithTwist = false;
    public bool OnlyShowInDashboard = false;

    public int LinkDevice = 0;
    public bool FlipSides = false;
}
