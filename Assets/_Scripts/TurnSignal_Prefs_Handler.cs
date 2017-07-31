using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class TurnSignal_Prefs_Handler : MonoBehaviour 
{   
    private TurnSignalPrefs prefs = new TurnSignalPrefs();
    private string _filePath = "";
    public void SetFilePath(string path)
    {
        _filePath = path;
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
        File.WriteAllText(_filePath, text);
        return File.Exists(_filePath);
    }
    public bool Load()
    {
        if(!File.Exists(_filePath))
            return false;

        string text = File.ReadAllText(_filePath);
        var o = (TurnSignalPrefs) JsonUtility.FromJson(text, typeof(TurnSignalPrefs));
        if(o != null)
        {
            prefs = o;
            return true;
        }
        else 
            return false;
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
    public int TwistRate = 10;
    public int PetalCount = 6;
    public bool StartWithSteamVR = true;
    public bool UseChaperoneColor = false;
    public bool LinkOpacityWithTwist = false;
    public bool OnlyShowInDashboard = false;
}
