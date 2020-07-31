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

    public static Activity StartGameActivity(int levelLoaded)
    {
        return new Activity {
            State = "Exploring Level " + levelLoaded.ToString(),
            Timestamps =
            {
                Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            },
            Assets =
            {
                LargeImage = "hs2p_caves"
            },
            Instance = false
        };
    }
}