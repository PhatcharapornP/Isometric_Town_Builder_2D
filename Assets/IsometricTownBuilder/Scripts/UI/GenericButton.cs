#nullable enable
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GenericButton : MonoBehaviour
{
    private const float RAPID_CLICK_BLOCK_DURATION = 0.1f;

    [SerializeField] private bool blockRapidClick = true;
    [SerializeField] private TextMeshProUGUI? buttonText;
    [SerializeField] private Button? button;

    private float _lastClickTime = -Mathf.Infinity;

    public Action? ClickedAction = null;

    //for async subscription
    private IDisposable? buttonEvent;


    private event Func<CancellationToken, UniTask>? onClickAsync;

    void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    void OnEnable()
    {
        button.onClick.AddListener(OnButtonClicked);
        buttonEvent = button.OnClickAsAsyncEnumerable().SubscribeAwait(OnClickedAsync);
    }

    void OnDisable()
    {
        buttonEvent?.Dispose();
        button.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        ClearCallback();
    }

    private void OnButtonClicked()
    {
        if (!TryPassRapidClickGate())
        {
            return;
        }

        ClickedAction?.Invoke();
        PlaySfx();
    }
    
    public void AddToOnClickAsync(Func<CancellationToken, UniTask> onClickAsync)
    {
        this.onClickAsync += onClickAsync;
    }

    public void RemoveFromOnClickAsync(Func<CancellationToken, UniTask> onClickAsync)
    {
        this.onClickAsync -= onClickAsync;
    }

    private async UniTask OnClickedAsync(AsyncUnit _, CancellationToken cancellationToken)
    {
        if (onClickAsync != null)
        {
            await UniTask.WhenAll(
                onClickAsync.GetInvocationList()
                    .Cast<Func<CancellationToken, UniTask>>()
                    .Select(x => x(cancellationToken))
            );
        }
    }

    public void ClearCallback([CallerMemberName] string caller = "")
    {
        ClickedAction = null;
        onClickAsync = null;
    }

    private bool TryPassRapidClickGate()
    {
        if (!blockRapidClick)
        {
            return true;
        }

        float now = Time.unscaledTime;
        float elapsed = now - _lastClickTime;

        if (elapsed < RAPID_CLICK_BLOCK_DURATION)
        {
            return false;
        }

        _lastClickTime = now;
        return true;
    }

    private void PlaySfx()
    {
        //do play the sfx here, when have a chance...   
    }
}