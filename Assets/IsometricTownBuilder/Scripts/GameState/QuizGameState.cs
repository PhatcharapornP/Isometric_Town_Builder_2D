#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public class QuizGameState : BaseGameState
{
    public override GameStateType StateType => GameStateType.Building;
    private GameplayUIManager gameplayUIManager;
    private GameStateManager gameStateManager;
    private QuizHandler quizHandler;
    private QuizPopup quizPopup;
    private bool isInitialized = false;
    
    public override void Initialize()
    {
        InitializeQuizState().Forget();
    }

    private async UniTask InitializeQuizState()
    {
        await UniTask.WaitUntil(() => GamePlayManager.Instance != null
                                      && GamePlayManager.Instance.GameStateManager != null
                                      && GamePlayManager.Instance.GamePlayUI != null
                                      && GamePlayManager.Instance.GameConfigContainer != null
                                      && GamePlayManager.Instance.GamePlayUI.QuizPopup != null
        );
        
        gameStateManager = GamePlayManager.Instance.GameStateManager;
        gameplayUIManager = GamePlayManager.Instance.GamePlayUI;
        quizPopup = gameplayUIManager.QuizPopup;
        quizHandler = new QuizHandler(GamePlayManager.Instance.GameConfigContainer.QuizQuestions);
        
        //sub to onClickAnswer
        quizPopup.OnClickAnswer += OnClickAnswer;
        quizHandler.OnCorrectAnswer += GoToNextQuestion;
        
        isInitialized = true;
    }

    public override void Enter()
    {
        if (!isInitialized)
        {
            return;
        }
        
        GoToNextQuestion();
    }

    private void GoToNextQuestion()
    {
        var questionDetail = quizHandler.GetNextQuestion();
        if (questionDetail == null)
        {
            NoMoreQuizGoToIdle();

            return;
        }
        quizPopup.ShowQuizPopup(questionDetail);
    }

    private void NoMoreQuizGoToIdle()
    {
        gameplayUIManager.EnableQuizPopup(false);
        //should've been localized, if have time...
        gameplayUIManager.ShowMidScreenText("You've passed");
        gameStateManager.GoToState(GameStateType.Idle);
    }

    private void OnClickAnswer(int choiceIndex)
    {
        quizHandler.ProcessAnswer(choiceIndex);
        quizHandler.SaveQuizData();

        if (!quizHandler.HasNextQuestion)
        {
            NoMoreQuizGoToIdle();
        }
    }

    public override void Exit()
    {
        quizHandler.SaveQuizData();
    }
}

public class QuizHandler
{
    private QuizQuestions quizQuestions;
    private SavedQuizData? runtimeSaveQuizData;
    private Dictionary<int,QuizData> quizDict = new Dictionary<int, QuizData>();
    private int currentQuizId = -1;
    public event Action? OnCorrectAnswer;
    public event Action? OnWrongAnswer;

    public bool HasNextQuestion => quizDict.Count > 0;
    
    public QuizHandler(QuizQuestions questionCollection)
    {
        quizQuestions = questionCollection;
        quizDict.Clear();
        currentQuizId = -1;
        foreach (var quizData in quizQuestions.QuizDataCollection)
        {
            quizDict[quizData.QuizId] = quizData;
        }
        LoadSaveQuiz();
    }

    private void LoadSaveQuiz()
    {
        PlayerPrefManager.TryGetString(SaveKeyGlobal.QuizSaveKey,out string saveStringValue);
        runtimeSaveQuizData  = JsonConvert.DeserializeObject<SavedQuizData>(saveStringValue);
        if (runtimeSaveQuizData == null)
        {
            runtimeSaveQuizData = new SavedQuizData();
        }
        foreach (var quizId in runtimeSaveQuizData.finishedQuizId)
        {
            currentQuizId = quizId;
            quizDict.Remove(quizId);
        }
    }

    public void SaveQuizData()
    {
        string saveStringValue = JsonConvert.SerializeObject(runtimeSaveQuizData);  
        PlayerPrefManager.SaveString(SaveKeyGlobal.QuizSaveKey, saveStringValue);
    }

    public void UpdateFinishedQuizId(int quizId)
    {
        if (runtimeSaveQuizData.finishedQuizId == null)
        {
            runtimeSaveQuizData.finishedQuizId = new List<int>();
        }
        runtimeSaveQuizData.finishedQuizId.Add(quizId);
        quizDict.Remove(quizId);
    }

    public QuizPopupDetail? GetNextQuestion()
    {
        if (quizDict.Count <= 0)
        {
            return null;
        }

        int nextQuizId = currentQuizId + 1;
        bool gotQuestion = quizDict.TryGetValue(nextQuizId, out var nextQuestion);

        if (!gotQuestion)
        {
            return null;
        }
        
        currentQuizId = nextQuizId;
        var quizDetail = new QuizPopupDetail(
            headerText: "Pick the right answer", //Should've been localized
            contentText: nextQuestion.Question,
            answerOne: nextQuestion.Choices[0],
            answerTwo: nextQuestion.Choices[1],
            answerThree: nextQuestion.Choices[2],
            answerFour: nextQuestion.Choices[3]
        );

        return quizDetail;
    }

    public bool IsCorrectAnswer(int choiceIndex)
    {
        return quizDict.ContainsKey(currentQuizId) && quizDict[currentQuizId].Answers.Contains(choiceIndex);
    }

    public bool ProcessAnswer(int choiceIndex)
    {
        bool isCorrectAnswer = IsCorrectAnswer(choiceIndex);
        if (isCorrectAnswer)
        {
            UpdateFinishedQuizId(currentQuizId);
            OnCorrectAnswer?.Invoke();
        }
        else
        {
            OnWrongAnswer?.Invoke();           
        }

        return isCorrectAnswer;
    }
}