#nullable enable
using System;
using TMPro;
using UnityEngine;

//TODO: Refactor this to be contain in some container or DI later
/// <summary>
/// This only shows the question and open to be answer button sub only
/// </summary>

public class QuizPopup : MonoBehaviour
{
   public static QuizPopup? Instance;
   [SerializeField] private TextMeshProUGUI headerText;
   [SerializeField] private TextMeshProUGUI contentText;
   [SerializeField] private GenericButton answerOne;
   [SerializeField] private GenericButton answerTwo;
   [SerializeField] private GenericButton answerThree;
   [SerializeField] private GenericButton answerFour;

   public bool IsShowingQuizPopup => gameObject.activeInHierarchy;

   private void Awake()
   {
      Instance = this;
   }

   public void ShowQuizPopup(QuizPopupDetail popupDetail)
   {
      headerText.SetText(popupDetail.HeaderText);
      contentText.SetText(popupDetail.ContentText);
      answerOne.SetButtonText(popupDetail.answerOneText);
      answerTwo.SetButtonText(popupDetail.answerTwoText);
      answerThree.SetButtonText(popupDetail.answerThreeText);
      answerFour.SetButtonText(popupDetail.answerFourText);
      answerOne.ClickedAction += popupDetail.OnClickAnswerOne;
      answerTwo.ClickedAction += popupDetail.OnClickAnswerTwo;
      answerThree.ClickedAction += popupDetail.OnClickAnswerThree;
      answerFour.ClickedAction += popupDetail.OnClickAnswerFour;
      
      EnableQuizPopup(true);
   }

   private void OnDisable()
   {
      answerOne.ClearCallback();
      answerTwo.ClearCallback();
      answerThree.ClearCallback();
      answerFour.ClearCallback();
   }

   private void EnableQuizPopup(bool enable)
   {
      if (gameObject.activeInHierarchy == enable)
      {
         return;
      }
      
      gameObject.SetActive(enable);
   }
}

public class QuizPopupDetail
{
   public string HeaderText;
   public string ContentText;
   public string answerOneText;
   public string answerTwoText;
   public string answerThreeText;
   public string answerFourText;
   public Action OnClickAnswerOne;
   public Action OnClickAnswerTwo;
   public Action OnClickAnswerThree;
   public Action OnClickAnswerFour;
}
