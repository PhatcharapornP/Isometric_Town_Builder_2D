#nullable enable
using UnityEngine;

public static class SaveKeyGlobal
{
    public const string BuildObjectSaveKey = "BdsK";
    public const string QuizSaveKey = "QsK";
}

public class SaveLoadManager //each system are handle their own save and load, must be refactor to here for easier maintainance
{
    public void LoadGameSave()
    {
    }

    public void SaveGame()
    {
    }
}
