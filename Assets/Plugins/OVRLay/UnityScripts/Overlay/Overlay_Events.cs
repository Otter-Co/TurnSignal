using System;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

[RequireComponent(typeof(Overlay_Unity))]
public class Overlay_Events : MonoBehaviour
{


    [Serializable] public class BoolEvent : UnityEvent<bool> { }
    [Serializable] public class VoidEvent : UnityEvent { }
    [Serializable] public class StringEvent : UnityEvent<string> { };
    [Serializable] public class DoubleStringEvent : UnityEvent<string, string> { }
    [Serializable] public class VREventEvent : UnityEvent<VREvent_t> { }

    public BoolEvent OnDashboardChange;
    public BoolEvent OnFocusChange;
    public BoolEvent OnVisibilityChange;
    public VoidEvent OnKeyboardDone;
    public VoidEvent OnKeyboardClose;
    public DoubleStringEvent OnKeyboardInput;
    public StringEvent OnError;
    public VREventEvent OnOtherEvent;

    private bool setup = false;
    private OVRLay.OVRLay overlay;

    void Start()
    {
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

        if (OnError == null)
            OnError = new StringEvent();

        if (OnOtherEvent == null)
            OnOtherEvent = new VREventEvent();
    }

    void Update()
    {
        if (!setup && overlay == null)
        {
            if ((overlay = GetComponent<Overlay_Unity>()?.overlay) == null)
                return;

            overlay.OnDashboardChange += (eventT, active) => OnDashboardChange.Invoke(active);
            overlay.OnFocusChange += (eventT, hasFocus) => OnFocusChange.Invoke(hasFocus);
            overlay.OnVisibilityChange += (eventT, visible) => OnVisibilityChange.Invoke(visible);
            overlay.OnKeyboardDone += (eventT) => OnKeyboardDone.Invoke();
            overlay.OnKeyboardClose += (eventT) => OnKeyboardClose.Invoke();
            overlay.OnKeyboardInput += (eventT, m, f) => OnKeyboardInput.Invoke(m, f);
            overlay.OnError += (err) => OnError.Invoke(err);
            overlay.OnOtherEvent += (eventT) => OnOtherEvent.Invoke(eventT);

            setup = true;
        }
    }
}