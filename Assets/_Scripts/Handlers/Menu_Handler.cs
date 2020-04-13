using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Menu_Handler : MonoBehaviour
{
    public Slider scaleSlider;
    public Slider opacitySlider;
    public Slider twistRateSlider;
    public Slider petalSlider;
    public Slider heightSlider;
    public Slider followSpeedSlider;
    public Slider forwardArrowAngle;

    public ValueButtonUI scaleButtonUI;
    public ValueButtonUI opacityButtonUI;
    public ValueButtonUI twistRateButtonUI;
    public ValueButtonUI petalButtonUI;
    public ValueButtonUI heightButtonUI;
    public ValueButtonUI followButtonUI;
    public ValueButtonUI forwardButtonUI;

    [Space(5)]

    public Toggle startWithVRToggle;
    public Toggle enableSteamWorksToggle;
    public Toggle hideMainWindowToggle;
    public Toggle followPlayerToggle;
    public Toggle useChapColorToggle;
    public Toggle tieTwistToggle;
    public Toggle onlyShowInDashToggle;
    public Toggle forwardArrow;

    [Space(5)]

    public Toggle disableLinkToggle;
    public Toggle rightLinkToggle;
    public Toggle leftLinkToggle;
    public Toggle flipSidesToggle;

    public void SetUIValues(TurnSignalOptions opts)
    {
        startWithVRToggle.isOn = opts.StartWithSteamVR;
        enableSteamWorksToggle.isOn = opts.EnableSteamworks;
        hideMainWindowToggle.isOn = opts.ShowMainWindow;
        followPlayerToggle.isOn = opts.FollowPlayerHeadeset;
        useChapColorToggle.isOn = opts.UseChaperoneColor;
        tieTwistToggle.isOn = opts.LinkOpatWithTwist;
        onlyShowInDashToggle.isOn = opts.OnlyShowInDashboard;

        forwardArrow.isOn = opts.ForwardArrow;

        scaleSlider.value = opts.Scale;
        opacitySlider.value = opts.Opacity;
        twistRateSlider.value = opts.TwistRate;
        petalSlider.value = opts.PetalCount;
        heightSlider.value = opts.Height;
        followSpeedSlider.value = opts.FollowSpeed;

        forwardButtonUI.currentValue = opts.ForwardArrowAngle;
        scaleButtonUI.currentValue = opts.Scale;
        opacityButtonUI.currentValue = opts.Opacity;
        twistRateButtonUI.currentValue = opts.TwistRate;
        petalButtonUI.currentValue = opts.PetalCount;
        heightButtonUI.currentValue = opts.Height;
        followButtonUI.currentValue = opts.FollowSpeed;


        disableLinkToggle.isOn = opts.LinkOptions == TurnSignalLinkOpts.None;
        rightLinkToggle.isOn = (opts.LinkOptions == TurnSignalLinkOpts.RightFront);
        leftLinkToggle.isOn = (opts.LinkOptions == TurnSignalLinkOpts.LeftFront);
        flipSidesToggle.isOn = (opts.LinkOptions & TurnSignalLinkOpts.Old_FlipSides) > 0;
    }

    public TurnSignalOptions GetUIValues()
    {
        TurnSignalLinkOpts linkOpts = TurnSignalLinkOpts.None;

        if (rightLinkToggle.isOn)
            linkOpts |= TurnSignalLinkOpts.RightFront;
        else if (leftLinkToggle.isOn)
            linkOpts |= TurnSignalLinkOpts.LeftFront;

        if ((rightLinkToggle.isOn || leftLinkToggle.isOn) && flipSidesToggle.isOn)
            linkOpts |= TurnSignalLinkOpts.Old_FlipSides;

        return new TurnSignalOptions() {
            StartWithSteamVR = startWithVRToggle.isOn,
            EnableSteamworks = enableSteamWorksToggle.isOn,
            ShowMainWindow = hideMainWindowToggle.isOn,
            FollowPlayerHeadeset = followPlayerToggle.isOn,
            UseChaperoneColor = useChapColorToggle.isOn,
            LinkOpatWithTwist = tieTwistToggle.isOn,
            OnlyShowInDashboard = onlyShowInDashToggle.isOn,
            ForwardArrow = forwardArrow.isOn,


            ForwardArrowAngle = forwardButtonUI.currentValue,
            Scale = scaleButtonUI.currentValue,
            Opacity = opacityButtonUI.currentValue,
            TwistRate = twistRateButtonUI.currentValue,
            PetalCount = petalButtonUI.currentValue,
            Height = heightButtonUI.currentValue,
            FollowSpeed = followButtonUI.currentValue,

            LinkOptions = linkOpts,
        };
    }
}