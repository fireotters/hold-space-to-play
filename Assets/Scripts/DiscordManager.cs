using System;
using System.Collections;
using Discord;
using UnityEngine;

public class DiscordManager : MonoBehaviour
{
    public static DiscordManager Instance { get; private set; }
    private Discord.Discord discord;
    private ActivityManager activityManager;
    
    private const long DiscordClientId = 641934700033867787; // THIS IS ULTRA, HELLA, MEGA BAD TO DO THIS PROBABLY SHOULD BE LOADED IN FROM SOMEWHERE AND NOT HARDCODED.
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            discord = new Discord.Discord(DiscordClientId, (UInt64) CreateFlags.NoRequireDiscord);
            activityManager = discord.GetActivityManager();
        }
        else
        {
            Destroy(gameObject);
        }

    }
    
    private void Update()
    {
        discord.RunCallbacks();
    }

    /// <summary>
    /// Updates current activity in Rich Presence
    /// </summary>
    /// <param name="activity">Activity definition</param>
    /// <returns>Status flag</returns>
    public bool UpdateDiscordRp(Activity activity)
    {
        var operationResult = false;

        if (activityManager != null)
        {
            activityManager.UpdateActivity(activity, result =>
            {
                if (result == Result.Ok)
                    operationResult = true;
                else 
                    Debug.LogError("Uh oh something shat itself while updating rich presence.\nResult code: " + result);
            });
        }
        else
        {
            Debug.LogError("activityManager is null");
        }
        return operationResult;
    }

    private void OnApplicationQuit()
    {
        activityManager.ClearActivity(result =>
        {
            Debug.Log(result == Result.Ok
                ? "Presence cleared successfully!"
                : "uh oh how did you debug this thing again? Result: " + result);
        });
        
        discord.Dispose();
    }

}