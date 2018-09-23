using System;
using UnityEngine;

public class Options_Handler : MonoBehaviour
{
    public Overlay_Unity floorOverlay;
    public Floor_Handler floorHandler;
    public Twister twister;

    public bool ApplyOptions(TurnSignalOptions opts)
    {
        return true;
    }
}