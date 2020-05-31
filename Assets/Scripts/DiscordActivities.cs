using System;
using Discord;

public static class DiscordActivities
{
    public static readonly Activity MainMenuActivity = new Activity
    {
        State = "In the Main Menu",
        Assets =
        {
            LargeImage = "smile"
        },
        Instance = false
    };

    public static Activity StartGameActivity()
    {
        return new Activity {
            State = "Exploring the caves...",
            Timestamps =
            {
                Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            },
            Assets =
            {
                LargeImage = "hs2p_caves",
                LargeText = "What secrets awaits in there...?"
            },
            Instance = false
        };
    }
}