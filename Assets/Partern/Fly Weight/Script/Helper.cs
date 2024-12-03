using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    static readonly Dictionary<float, WaitForSeconds> WaitForSeconds = new ();
    public static WaitForSeconds GetWaitForSeconds(float seconds)
    {
        if (WaitForSeconds.TryGetValue(seconds, out var forSeconds))
        {
            return forSeconds;
        }
        var waitForSeconds = new WaitForSeconds(seconds);
        WaitForSeconds.Add(seconds, waitForSeconds);
        return WaitForSeconds[seconds];
        
    }
}