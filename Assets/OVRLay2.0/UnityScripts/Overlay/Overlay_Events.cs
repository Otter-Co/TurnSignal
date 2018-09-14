using System;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

[RequireComponent(typeof(Overlay_Unity))]
public class Overlay_Events : MonoBehaviour
{
    private Overlay_Unity u_overlay;
    private OVRLay.OVRLay overlay;

    [Serializable] public class BoolEvent : UnityEvent<bool> { }
    [Serializable] public class VoidEvent : UnityEvent { }
    [Serializable] public class DoubleStringEvent : UnityEvent<string, string> { }

    public BoolEvent OnDashboardChange;
    public BoolEvent OnFocusChange;
    public BoolEvent OnVisibilityChange;
    public VoidEvent OnKeyboardDone;
    public VoidEvent OnKeyboardClose;
    public DoubleStringEvent OnKeyboardInput;


    void Start()
    {
        u_overlay = GetComponent<Overlay_Unity>();
        overlay = u_overlay.overlay;

        if (OnDashboardChange == null)
            OnDashboardChange = new BoolEvent();

        if (OnFocusChange == null)
            OnFocusChange = new BoolEvent();

        if (OnVisibilityChange == null)
            OnVisibilityChange = new BoolEvent();

        if (OnKeyboardDone == null)
            OnKeyboardDone = new VoidEvent();

        if (OnKeyboardClose == null)
            OnKeyboardClose = new VoidEvent();

        if (OnKeyboardInput == null)
            OnKeyboardInput = new DoubleStringEvent();

        overlay.OnDashboardChange += (active) => OnDashboardChange.Invoke(active);
        overlay.OnFocusChange += (hasFocus) => OnFocusChange.Invoke(hasFocus);
        overlay.OnVisibilityChange += (visible) => OnVisibilityChange.Invoke(visible);
        overlay.OnKeyboardDone += () => OnKeyboardDone.Invoke();
        overlay.OnKeyboardClose += () => OnKeyboardClose.Invoke();
        overlay.OnKeyboardInput += (m, f) => OnKeyboardInput.Invoke(m, f);
    }
}