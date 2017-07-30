using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSignal_Prefs_Handler : MonoBehaviour 
{   
    private float _Scale = 2f;
	public float Scale 
    {
        get 
        {
            _Scale = PlayerPrefs.GetFloat("scale", 2f);
            return _Scale;
        }
        set 
        {
            _Scale = value;
            PlayerPrefs.SetFloat("scale", _Scale);
        }
    }

    private float _Opacity = 1f;
    public float Opacity 
    {
        get 
        {                
            _Opacity = PlayerPrefs.GetFloat("opacity", 0.03f);
            return _Opacity;
        }
        set
        {
            _Opacity = value;
            PlayerPrefs.SetFloat("opacity", _Opacity);
        }
    }

    private int _TwistRate = 10;
    public int TwistRate 
    {
        get
        {
            _TwistRate = PlayerPrefs.GetInt("twistrate", 10);    
            return _TwistRate;
        }
        set
        {
            _TwistRate = value;
            PlayerPrefs.SetInt("twistrate", _TwistRate);
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
            _Petals = PlayerPrefs.GetInt("petalcount", 6);
            return _Petals;
        }
        set 
        {
            _Petals = value;
            PlayerPrefs.SetInt("petalcount", value);
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
    private bool _StartWithSteamVR = true;
    public bool StartWithSteamVR
    {
        get 
        {
            _StartWithSteamVR = (PlayerPrefs.GetInt("startwithsteamvr", 1) == 1);
            return _StartWithSteamVR;
        }
        set
        {
            _StartWithSteamVR = value;
            PlayerPrefs.SetInt("startwithsteamvr", _StartWithSteamVR ? 1 : 0);
        }
    }

    private bool _UseChaperoneColor = false;
    public bool UseChaperoneColor 
    {
        get 
        {
            _UseChaperoneColor = (PlayerPrefs.GetInt("usechapcolor", 0) == 1);
            return _UseChaperoneColor;
        }
        set 
        {
            _UseChaperoneColor = value;
            PlayerPrefs.SetInt("usechapcolor", _UseChaperoneColor ? 1 : 0);
        }
    }

    private bool _LinkOpacityWithTwist = false;
    public bool LinkOpacityWithTwist
    {
        get 
        {
            
            _LinkOpacityWithTwist = (PlayerPrefs.GetInt("linktwistalpha", 0) == 1);
            return _LinkOpacityWithTwist;
        }
        set 
        {
            _LinkOpacityWithTwist = value;
            PlayerPrefs.SetInt("linktwistalpha", _LinkOpacityWithTwist ? 1 : 0);
        }
    }

    private bool _OnlyShowInDashboard = false;
    public bool OnlyShowInDashboard 
    {
        get 
        {
            _OnlyShowInDashboard = (PlayerPrefs.GetInt("onlyshowindash", 0) == 1);
            return _OnlyShowInDashboard;
        }
        set 
        {
            _OnlyShowInDashboard = value;
            PlayerPrefs.SetInt("onlyshowindash", (_OnlyShowInDashboard ? 1 : 0));
        }
    }

    public void Save()
    {
        PlayerPrefs.Save();
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
    }
}
