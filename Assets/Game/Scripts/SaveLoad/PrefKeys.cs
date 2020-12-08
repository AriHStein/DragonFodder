using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrefKeys
{
    const string m_continueGameKey = "ContinueGame";
    public static string ContinueFileName { 
        get {
            return PlayerPrefs.GetString(m_continueGameKey);
        } 
    }
    public static void SetContinueFileName(string fileName)
    {
        PlayerPrefs.SetString(m_continueGameKey, fileName);
    }
    public static void ClearContinueFile()
    {
        PlayerPrefs.DeleteKey(m_continueGameKey);
    }
}
