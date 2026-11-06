using UnityEditor;
using UnityEngine;

public static  class PlayerPrefsUtil
{
#if UNITY_EDITOR
    [MenuItem("Tools/Clear PlayerPrefs")]
    private static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log($"[PlayerPref tool] player pref cleared");
    }
#endif
}
