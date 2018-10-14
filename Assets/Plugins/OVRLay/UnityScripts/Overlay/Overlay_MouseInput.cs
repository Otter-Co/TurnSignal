using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using OVRLay;

[RequireComponent(typeof(Overlay_Unity))]
public class Overlay_MouseInput : MonoBehaviour
{
    [Header("Overlay Mouse Input Settings")]
    public VROverlayInputMethod inputMethod = VROverlayInputMethod.None;
    [Space(10)]
    public bool simulateUnityInput = false;
    public EventSystem eventSystem;
    public float mouseDownTimeUntilDrag = 0.1f;
    public Vector2 textureSize;

    [Header("Overlay Mouse Values")]
    [Space(10)]
    public Vector2 rawMousePosition;
    [Space(10)]
    public Vector2 lastAdjustedMousePos;
    [Space(10)]
    public bool mouseDown = false;
    public float mouseDownTime = 0f;
    [Space(10)]
    public float lastMoveUpdate = 0f;
    public float lastButtonUpdate = 0f;
    [Space(10)]

    private Overlay_MouseBaseInput inputOverride;
    private HashSet<Selectable> lastTargs = new HashSet<Selectable>();
    private HashSet<Selectable> currentTargs = new HashSet<Selectable>();
    private HmdVector2_t mouseScale = new HmdVector2_t() { v0 = 1f, v1 = 1f };

    private float reverseAspect = 0f;
    private int widthMulti = 0;
    private int heightMulti = 0;

    private Overlay_Unity u_overlay;
    public OVRLay.OVRLay overlay;

    public Vector2 GetAdjustMousePos()
    {
        float mouseX = widthMulti * rawMousePosition.x;
        float mouseY = heightMulti * (1f - (rawMousePosition.y / reverseAspect));

        mouseY = !float.IsNaN(mouseY) ? mouseY : 0;

        return new Vector2(mouseX, mouseY);
    }

    void UpdateRawMousePosition(VREvent_t eventT)
    {
        var mD = eventT.data.mouse;
        Debug.Log(mD.x + "," + mD.y);

        rawMousePosition.x = mD.x;
        rawMousePosition.y = mD.y;

        lastMoveUpdate = 0;
    }

    void UpdateRawMouseButton(VREvent_t eventT, bool state)
    {
        // UpdateRawMousePosition(mD);
        var mD = eventT.data.mouse;
        Debug.Log(mD.x + "," + mD.y);

        switch ((EVRMouseButton)mD.button)
        {
            case EVRMouseButton.Left:
                mouseDown = state;
                break;
        }

        lastButtonUpdate = 0;
    }

    void Update()
    {
        if (overlay == null)
        {
            u_overlay = GetComponent<Overlay_Unity>();

            if (u_overlay.overlay != null)
                overlay = u_overlay.overlay;
            else
                return;

            overlay.OnMouseMove += (data) => UpdateRawMousePosition(data);
            overlay.OnMouseDown += (data) => UpdateRawMouseButton(data, true);
            overlay.OnMouseUp += (data) => UpdateRawMouseButton(data, false);

            inputOverride = gameObject.AddComponent<Overlay_MouseBaseInput>().SetOverlayInput(this);
            eventSystem.currentInputModule.inputOverride = inputOverride;

            return;
        }

        if (overlay.Created)
        {
            int ttW = widthMulti = (int)textureSize.x,
                ttH = heightMulti = (int)textureSize.y;

            reverseAspect = (float)ttH / (float)ttW;
            mouseScale.v1 = reverseAspect;

            overlay.InputMethod = inputMethod;
            overlay.MouseScale = mouseScale;

            lastMoveUpdate += Time.deltaTime;
            lastButtonUpdate += Time.deltaTime;

            if (mouseDown)
                mouseDownTime += Time.deltaTime;
            else if (mouseDownTime > 0f)
                mouseDownTime = 0f;

            lastAdjustedMousePos = GetAdjustMousePos();
        }
    }
}

