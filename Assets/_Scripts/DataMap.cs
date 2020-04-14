using System;

[System.Serializable]
public enum TurnSignalLinkOpts
{
    None = 0,
    RightFront = 1,
    LeftFront = 4,
    RightBack = 2,
    LeftBack = 8,
    Old_FlipSides = 16,
}

[System.Serializable]
public struct TurnSignalOptions
{
    public float? timestamp;
    public bool StartWithSteamVR;
    public bool EnableSteamworks;
    public bool ShowMainWindow;

    public bool FollowPlayerHeadeset;
    public bool UseChaperoneColor;
    public bool LinkOpatWithTwist;
    public bool OnlyShowInDashboard;
    public bool ForwardArrow;
    public float ForwardArrowAngle;

    public float Scale;
    public float Opacity;
    public float TwistRate;
    public float PetalCount;
    public float Height;
    public float FollowSpeed;

    public TurnSignalLinkOpts LinkOptions;

    public TurnSignalOptions cleaned
    {
        get
        {
            var clone = this;
            clone.timestamp = null;
            return clone;
        }
    }

    public static TurnSignalOptions DefaultOptions = new TurnSignalOptions() {
        timestamp = null,
        StartWithSteamVR = true,
        EnableSteamworks = true,
        ShowMainWindow = true,
        FollowPlayerHeadeset = false,
        UseChaperoneColor = false,
        LinkOpatWithTwist = false,
        OnlyShowInDashboard = false,
        ForwardArrow = true,
        ForwardArrowAngle = 0,

        Scale = 1f,
        Opacity = 0.15f,
        TwistRate = 7f,
        PetalCount = 3,
        Height = 0,
        FollowSpeed = 10f,
        LinkOptions = TurnSignalLinkOpts.None
    };
}