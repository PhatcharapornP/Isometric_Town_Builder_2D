#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//TODO: Wanna DI this instead of just static value this class, but need to finish with other things first

public class UtilPopup : MonoBehaviour
{
    public static UtilPopup? Instance;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private Button clickAnywhereButton;
    [SerializeField] private GenericButton confirmButton;
    [SerializeField] private GenericButton cancelButton;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPopup(NotifyPopupClickToClose popupDetails)
    {
        clickAnywhereButton.onClick.AddListener(ClosePopup);
        EnableClickAnywhereButton(true);
        SetupPopupTextAndShow(popupDetails.Title, popupDetails.Message);
    }

    public void ShowPopup(PopupWithOneButton popupDetails)
    {
        if (popupDetails.OnClickAction != null)
        {
            confirmButton.ClickedAction = popupDetails.OnClickAction;
        }

        if (popupDetails.OnClickAsync != null)
        {
            confirmButton.AddToOnClickAsync(OnClickAsyncFunc);
        }

        EnableConfirmButton(popupDetails.OnClickAction != null || popupDetails.OnClickAsync != null);
        SetupPopupTextAndShow(popupDetails.Title, popupDetails.Message);
        EnableClickAnywhereButton(true);

        async UniTask OnClickAsyncFunc(CancellationToken ct)
        {
            popupDetails.OnClickAsync?.Invoke(ct);
        }
    }

    public void ShowPopup(PopupWithTwoButtons popupDetails)
    {
        if (popupDetails.OnClickActionOne != null)
        {
            confirmButton.ClickedAction = popupDetails.OnClickActionOne;
        }

        if (popupDetails.OnClickActionTwo != null)
        {
            cancelButton.ClickedAction = popupDetails.OnClickActionTwo;
        }

        if (popupDetails.OnClickAsyncOne != null)
        {
            confirmButton.AddToOnClickAsync(OnClickAsyncFuncOne);
        }

        if (popupDetails.OnClickAsyncTwo != null)
        {
            cancelButton.AddToOnClickAsync(OnClickAsyncFuncTwo);
        }

        bool hasAnyConfirmAction = popupDetails.OnClickActionOne != null || popupDetails.OnClickAsyncOne != null;
        bool hasAnyCancelAction = popupDetails.OnClickActionTwo != null || popupDetails.OnClickAsyncTwo != null;

        EnableConfirmButton(hasAnyConfirmAction);
        EnableCancelButton(hasAnyCancelAction);
        SetupPopupTextAndShow(popupDetails.Title, popupDetails.Message);
        EnableClickAnywhereButton(true);

        async UniTask OnClickAsyncFuncOne(CancellationToken ct)
        {
            popupDetails.OnClickAsyncOne?.Invoke(ct);
        }

        async UniTask OnClickAsyncFuncTwo(CancellationToken ct)
        {
            popupDetails.OnClickAsyncTwo?.Invoke(ct);
        }
    }

    private void SetupPopupTextAndShow(string title, string message)
    {
        headerText.SetText(title);
        contentText.SetText(message);
        gameObject.SetActive(true);
    }

    private void EnableConfirmButton(bool enable)
    {
        confirmButton.gameObject.SetActive(enable);
    }

    private void EnableCancelButton(bool enable)
    {
        cancelButton.gameObject.SetActive(enable);
    }

    private void EnableClickAnywhereButton(bool enable)
    {
        clickAnywhereButton.gameObject.SetActive(enable);
    }

    public void ClosePopup()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        //TODO: Do transition later
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        confirmButton.ClearCallback();
        cancelButton.ClearCallback();
        clickAnywhereButton.onClick.RemoveAllListeners();
        EnableConfirmButton(false);
        EnableCancelButton(false);
        EnableClickAnywhereButton(false);
    }
}

public abstract class BasePopup
{
    public abstract string Title { get; }
    public abstract string Message { get; }
}

public class NotifyPopupClickToClose : BasePopup
{
    public override string Title { get; }
    public override string Message { get; }

    public NotifyPopupClickToClose(
        string title,
        string message
    )
    {
        Title = title;
        Message = message;
    }
}

public class PopupWithOneButton : BasePopup
{
    public override string Title { get; }
    public override string Message { get; }
    public Action? OnClickAction;
    public Func<CancellationToken, UniTask>? OnClickAsync;

    public PopupWithOneButton(
        string title,
        string message,
        Action? onClickAction,
        Func<CancellationToken, UniTask>? onClickAsync
    )
    {
        Title = title;
        Message = message;
        OnClickAction = onClickAction;
        OnClickAsync = onClickAsync;
    }
}

public class PopupWithTwoButtons : BasePopup
{
    public override string Title { get; }
    public override string Message { get; }
    public Action? OnClickActionOne;
    public Action? OnClickActionTwo;
    public Func<CancellationToken, UniTask>? OnClickAsyncOne;
    public Func<CancellationToken, UniTask>? OnClickAsyncTwo;

    public PopupWithTwoButtons(
        string title,
        string message,
        Action? onClickActionOne,
        Action? onClickActionTwo,
        Func<CancellationToken, UniTask>? onClickAsyncOne,
        Func<CancellationToken, UniTask>? onClickAsyncTwo
    )
    {
        Title = title;
        Message = message;
        OnClickActionOne = onClickActionOne;
        OnClickActionTwo = onClickActionTwo;
        OnClickAsyncOne = onClickAsyncOne;
        OnClickAsyncTwo = onClickAsyncTwo;
    }
}