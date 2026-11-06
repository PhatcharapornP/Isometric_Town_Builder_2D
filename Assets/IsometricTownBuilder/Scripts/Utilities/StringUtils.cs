using UnityEngine;

public static class StringUtils
{
    public static string LogColor(this string msg, Color color)
    {
#if UNITY_EDITOR
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{msg}</color>";
#else
            return msg;
#endif
    }
}