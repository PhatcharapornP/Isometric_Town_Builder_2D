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
        ,[CallerMemberName] string caller = ""
#endif
    )
    {
        PlayerPrefs.SetString(key, value);
        bool getStringSuccess = TryGetString(key, out string result);
        if (!getStringSuccess || string.IsNullOrEmpty(result))
        {
#if UNITY_EDITOR
            Debug.Log($"[PlayerPref] SaveString failed key: {key} | value: {value} | caller: {caller}".LogColor(new Color(1f, 0f,                    0.1f)));
#endif
        }
    }

    public static void SaveInt(
        string key,
        int value
#if UNITY_EDITOR
        , [CallerMemberName] string caller = ""
#endif
    )
    {
        PlayerPrefs.SetInt(key, value);

        bool getStringSuccess = TryGetInt(key, out int result);
        if (!getStringSuccess)
        {
#if UNITY_EDITOR
            Debug.Log($"[PlayerPref] SaveInt failed key: {key} | value: {value} | caller: {caller}".LogColor(new Color(1f, 0f,                    0.1f)));
#endif
        }
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public static bool TryGetString(
        string key,
        out string result
#if UNITY_EDITOR
        , [CallerMemberName] string caller = ""
#endif
    )
    {
        result = "";
        if (HasKey(key))
        {
            result = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(result))
            {
#if UNITY_EDITOR
                Debug.Log($"[PlayerPref] TryGetString Result from key: {key} is empty. {result} | caller: {caller}".LogColor(new Color(1f, 0.15f,0f)));
#endif

                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"[PlayerPref] TryGetString Doesn't contain key: {key} | caller: {caller}".LogColor(new Color(1f, 0.15f, 0f)));
#endif
            return false;
        }
    }

    public static bool TryGetInt(
        string key,
        out int result
#if UNITY_EDITOR
        , [CallerMemberName] string caller = ""
#endif
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
#if UNITY_EDITOR
            Debug.Log($"[PlayerPref] TryGetInt Doesn't contain key: {key} | caller: {caller}".LogColor(new Color(1f, 0.15f, 0f)));
#endif
            
            return false;
        }
    }
}