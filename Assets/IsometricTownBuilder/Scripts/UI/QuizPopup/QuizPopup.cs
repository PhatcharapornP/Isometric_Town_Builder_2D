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
   [SerializeField] private TextMeshProUGUI headerText;
   [SerializeField] private TextMeshProUGUI contentText;
   [SerializeField] private GenericButton answerOne;
   [SerializeField] private GenericButton answerTwo;
   [SerializeField] private GenericButton answerThree;
   [SerializeField] private GenericButton answerFour;
   
   public Action<int> OnClickAnswer;

   public bool IsShowingQuizPopup => gameObject.activeInHierarchy;

   public void ShowQuizPopup(QuizPopupDetail popupDetail)
   {
      headerText.SetText(popupDetail.HeaderText);
      contentText.SetText(popupDetail.ContentText);
      answerOne.SetButtonText(popupDetail.answerOneText);
      answerTwo.SetButtonText(popupDetail.answerTwoText);
      answerThree.SetButtonText(popupDetail.answerThreeText);
      answerFour.SetButtonText(popupDetail.answerFourText);
      answerOne.AddToClickAction(ClickAnswerOne);
      answerTwo.AddToClickAction(ClickAnswerTwo);
      answerThree.AddToClickAction(ClickAnswerThree);
      answerFour.AddToClickAction(ClickAnswerFour);
      
      EnableQuizPopup(true);
   }

   private void ClickAnswerOne()
   {
      OnClickAnswer?.Invoke(0);
   }
   
   private void ClickAnswerTwo()
   {
      OnClickAnswer?.Invoke(1);
   }
   
   private void ClickAnswerThree()
   {
      OnClickAnswer?.Invoke(2);
   }
   
   private void ClickAnswerFour()
   {
      OnClickAnswer?.Invoke(3);
   }


   private void OnDisable()
   {
      answerOne.ClearCallback();
      answerTwo.ClearCallback();
      answerThree.ClearCallback();
      answerFour.ClearCallback();
   }

   public void EnableQuizPopup(bool enable)
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

   public QuizPopupDetail(
      string headerText,
      string contentText,
      string answerOne,
      string answerTwo,
      string answerThree,
      string answerFour
      )
   {
      HeaderText = headerText;
      ContentText = contentText;
      answerOneText = answerOne;
      answerTwoText = answerTwo;
      answerThreeText = answerThree;
      answerFourText = answerFour;
   }
}
