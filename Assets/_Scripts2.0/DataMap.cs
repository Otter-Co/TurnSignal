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

    public static TurnSignalOptions DefaultOptions = new TurnSignalOptions()
    {
        timestamp = null,
        StartWithSteamVR = true,
        EnableSteamworks = true,
        HideMainWindow = true,
        FollowPlayerHeadeset = false,
        UseChaperoneColor = false,
        LinkOpatWithTwist = false,
        OnlyShowInDashboard = false,

        Scale = 1f,
        Opacity = 0.3f,
        TwistRate = 10f,
        PetalCount = 6,
        Height = 0,
        FollowSpeed = 10f,
        LinkOptions = TurnSignalLinkOpts.None
    };
}