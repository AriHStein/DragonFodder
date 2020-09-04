using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeManager
{
    static int pauseCount = 0;
    public static bool Paused { get { return pauseCount > 0; } }
    public static void Pause()
    {
        pauseCount++;
        if (pauseCount > 0)
        {
            Time.timeScale = 0;
        }
    }

    public static void Unpause()
    {
        pauseCount--;
        pauseCount = Mathf.Max(0, pauseCount);
        if (pauseCount == 0)
        {
            Time.timeScale = 1;
        }
    }

    public static void TogglePause()
    {
        if(pauseCount > 0)
        {
            Unpause();
        } else
        {
            Pause();
        }
    }
}
