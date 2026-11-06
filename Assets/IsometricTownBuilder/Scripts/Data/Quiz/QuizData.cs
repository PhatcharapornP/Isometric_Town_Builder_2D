using System;
using System.Collections.Generic;

[Serializable]
public class SavedQuizData
{
    public List<int> finishedQuizId;

    public SavedQuizData()
    {
        finishedQuizId = new List<int>();
    }
}

[Serializable]
public class QuizData
{
    public int QuizId;
    public string Question;
    public string[] Choices = new string[4];
    public int[] Answers;
}
