using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using OVRLay;

public class Overlay_MouseBaseInput : BaseInput
{
    public Overlay_MouseInput overlayMouseInput;
    private Vector2 lastMousePos;

    private bool overlayValid
    {
        get => (
            overlayMouseInput.simulateUnityInput &&
            overlayMouseInput.overlay.Created &&
            overlayMouseInput.overlay.Visible &&
            overlayMouseInput.overlay.HasFocus
        );
    }

    public Overlay_MouseBaseInput SetOverlayInput(Overlay_MouseInput omi)
    {
        overlayMouseInput = omi;
        return this;
    }

    public override bool mousePresent { get => overlayValid ? true : base.mousePresent; }
    public override Vector2 mouseScrollDelta { get => overlayValid ? Vector2.zero : base.mouseScrollDelta; }

    public override Vector2 mousePosition
    {
        get => overlayValid ? overlayMouseInput.GetAdjustMousePos() : base.mousePosition;
    }

    public override float GetAxisRaw(string axisName)
    {
        if (!overlayValid)
            return base.GetAxisRaw(axisName);

        float ret = 0;

        switch (axisName)
        {
            case "Horizontal":
                ret = mousePosition.x - overlayMouseInput.lastAdjustedMousePos.x;
                break;
            case "Vertical":
                ret = mousePosition.y - overlayMouseInput.lastAdjustedMousePos.y;
                break;
            default:
                break;
        }

        return ret;
    }

    public override bool GetMouseButtonUp(int button)
    {
        if (!overlayValid)
            return base.GetMouseButtonUp(button);

        switch (button)
        {
            case 0:
                return !overlayMouseInput.mouseDown;
            case 1:
            case 2:
            default:
                return true;
        }
    }

    public override bool GetMouseButtonDown(int button)
    {
        if (!overlayValid)
            return base.GetMouseButtonDown(button);

        switch (button)
        {
            case 0:
                return overlayMouseInput.mouseDown;
            case 1:
            case 2:
            default:
                return false;
        }
    }

    public override bool GetMouseButton(int button)
    {
        if (!overlayValid)
            return base.GetMouseButtonDown(button);

        switch (button)
        {
            case 0:
                return (
                    overlayMouseInput.mouseDown &&
                    overlayMouseInput.mouseDownTime >= overlayMouseInput.mouseDownTimeUntilDrag
                );
            case 1:
            case 2:
            default:
                return false;
        }
    }
}