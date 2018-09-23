using System;

[System.Serializable]
public enum TurnSignalLinkOpts
{
    None = 0,
    RightFront = 1,
    LeftFrom = 2,
    RightBack = 4,
    LeftBack = 8,
}

[System.Serializable]
public struct TurnSignalOptions
{
    public float timestamp;
    public bool StartWithSteamVR;
    public bool EnableSteamworks;
    public bool HideMainWindow;

    public bool FollowPlayerHeadeset;
    public bool UseChaperoneColor;
    public bool LinkOpatWithTwist;
    public bool OnlyShowInDashboard;

    public float Scale;
    public float Opacity;
    public float TwistRate;
    public float PetalCount;
    public float Height;
    public float followSpeed;

    public TurnSignalLinkOpts LinkOptions;

    public static TurnSignalOptions DefaultOptions = new TurnSignalOptions()
    {
        timestamp = 0,
        StartWithSteamVR = true,
        EnableSteamworks = true,
        HideMainWindow = true,
        FollowPlayerHeadeset = false,
        UseChaperoneColor = false,
        LinkOpatWithTwist = false,
        OnlyShowInDashboard = false,

        Scale = 1f,
        Opacity = 0.3f,
        TwistRate = 1f,
        PetalCount = 6,
        Height = 0,
        followSpeed = 2.5f,
        LinkOptions = TurnSignalLinkOpts.None
    };
}