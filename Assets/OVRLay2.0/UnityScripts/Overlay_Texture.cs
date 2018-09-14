using System;
using UnityEngine;
using Valve.VR;
using OVRLay;

[RequireComponent(typeof(Overlay_Unity))]
public class Overlay_Texture : MonoBehaviour
{
    public Texture currentTexture;
    [Space(10)]
    public VRTextureBounds_t textureBounds = new VRTextureBounds_t();

    void Update()
    {

    }
}