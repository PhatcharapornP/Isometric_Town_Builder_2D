using System.Runtime.CompilerServices;
using UnityEngine;

public static class PlayerPrefManager
{
    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    
    public static void SaveString(
        string key,
        string value
        #if UNITY_EDITOR
        ,
        [CallerMemberName] string caller = ""
        #endif
        )
    {
        PlayerPrefs.SetString(key, value);
        bool getStringSuccess = TryGetString(key, out string result);
        if (!getStringSuccess || string.IsNullOrEmpty(result))
        {
        }
    }
    
    public static void SaveInt(
        string key,
        int value,
        [CallerMemberName] string caller = ""
    )
    {
        PlayerPrefs.SetInt(key, value);

        bool getStringSuccess = TryGetInt(key, out int result);
        if (!getStringSuccess)
        {
            Debug.Log($"PlayerPrefSaveInt failed key: {key} | value: {value} | caller: {caller}".LogColor(new Color(1f, 0f, 0.1f)));
        }
    }
    
    public static void DeleteKey(string key, [CallerMemberName] string caller = "")
    {
        PlayerPrefs.DeleteKey(key);
    }
    
    public static bool TryGetString(
        string key,
        out string result,
        [CallerMemberName] string caller = ""
    )
    {
        result = "";
        if (HasKey(key))
        {
            result = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(result))
            {
                Debug.Log($"Result from key: {key} is empty. {result} | caller: {caller}".LogColor(new Color(1f, 0.15f, 0f)));
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            Debug.Log($"Doesn't contain key: {key} | caller: {caller}".LogColor(new Color(1f, 0.15f, 0f)));
            return false;
        }
    }
    
    public static bool TryGetInt(
        string key,
        out int result,
        [CallerMemberName] string caller = ""
    )
    {
        if (HasKey(key))
        {
            result = PlayerPrefs.GetInt(key);
            return true;
        }
        else
        {
            result = -1;
            Debug.Log($"Doesn't contain key: {key} | caller: {caller}".LogColor(new Color(1f, 0.15f, 0f)));
            return false;
        }
    }
  
}
