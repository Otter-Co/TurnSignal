using System;
using UnityEngine;
using Valve.VR;

public class Overlay_Unity : MonoBehaviour
{
    public string overlayName = "Unity Overlay";
    public string overlayKey = "unity-overlay";
    [Space(10)]
    public bool created = false;
    [Space(10)]
    public Overlay_Unity_Settings settings = new Overlay_Unity_Settings()
    {
        WidthInMeters = 1,
        Color = Color.white,
        Alpha = 1f,
        Visible = true,
        HighQuality = false,
    };
}

[System.Serializable]
public struct Overlay_Unity_Settings
{
    public float WidthInMeters;
    public Color Color;
    public float Alpha;
    public bool Visible;
    public bool HighQuality;
}