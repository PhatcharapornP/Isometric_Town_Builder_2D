#nullable enable
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputState
{
    Undefined,
    OnStartPressed,
    OnStartHolding,
    WhileHolding,
    OnEndHolding,
    OnReleased,
    OnResetHolding
}

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private bool isHolding = false;
    [field: SerializeField] public bool MuteInput { get; private set; } = false;

    [SerializeField] private float holdingTimer = 1f;
    [SerializeField] private float minHoldTimer = .2f;
    [SerializeField] private float currentHoldingTime = 0f;

    [Space] [SerializeField] private Vector3 screenToWorldPos = Vector3.zero;
    [field:SerializeField] public Vector3 ScreenPos { get; private set; } = Vector3.zero;
    [field:SerializeField] public Vector2 ClickPos { get; private set; }= Vector2.zero;

    public event Action? OnStartPressed;
    public event Action? OnStartHolding;
    public event Action<float>? WhileHolding;
    public event Action<Vector3>? OnEndHolding;
    public event Action<Vector3>? OnReleased;
    public event Action? OnResetHolding;

    public event Action<Vector3>? InputPositionUpdated;
    
    public InputState  InputState { get; private set; } = InputState.Undefined;

    private void Awake()
    {
        OnStartPressed += UpdateStateOnStartPressed;
        OnStartHolding += UpdateStateOnStartHolding;
        WhileHolding += UpdateStateWhileHolding;
        OnEndHolding += UpdateStateOnEndHolding;
        OnReleased += UpdateStateOnReleased;
        OnResetHolding += UpdateStateOnResetHolding;
    }

    private void Update()
    {
        if (MuteInput)
        {
            return;
        }

        UpdateInputPosition();
        InputPositionUpdated?.Invoke(ScreenPos);

        if (IsInputHolding())
        {
            UpdateHoldingTimer();
        }

        TriggerOnTouchReleased();
        TriggerOnMouseReleased();
    }

    public void SetMuteInput(bool mute)
    {
        MuteInput = mute;
    }

    private bool IsInputHolding()
    {
        if (Touchscreen.current != null)
        {
            return Touchscreen.current.primaryTouch.press.isPressed;
        }
        
        if (Mouse.current != null)
        {
            return Mouse.current.leftButton.isPressed;
        }

        return false;
    }

    private void UpdateHoldingTimer()
    {
        if (!isHolding) //start holding
        {
            Debug.Log($"[Input] UpdateHoldingTimer -> trigger OnStartHolding".LogColor(new Color(0.69f, 0.93f, 0.75f)));
            currentHoldingTime = 0;
            isHolding = true;
            OnStartHolding?.Invoke();
        }
        else //while Holding
        {
            currentHoldingTime += Time.deltaTime;

            if (currentHoldingTime >= minHoldTimer)
            {
                WhileHolding?.Invoke(currentHoldingTime);    
            }
            

            if (currentHoldingTime >= holdingTimer && isHolding)
            {
                Debug.Log($"[Input] UpdateHoldingTimer -> trigger OnEndHolding".LogColor(new Color(0.93f, 0.41f, 0.29f)));
                OnEndHolding?.Invoke(ScreenPos);
                currentHoldingTime = 0;
                isHolding = false;
            }
        }
    }

    private void ResetHoldingState(
#if UNITY_EDITOR
        [CallerMemberName] string caller = ""
#endif
    )
    {
#if UNITY_EDITOR
        Debug.Log($"[Input] ResetHoldingState() -> called from: {caller} trigger OnResetHolding".LogColor(Color.orange));
#endif
        isHolding = false;
        currentHoldingTime = 0;
        OnResetHolding?.Invoke();
    }

    private void UpdateInputPosition()
    {
        var touchscreen = Touchscreen.current;
        if (touchscreen != null)
        {
            ClickPos = touchscreen.primaryTouch.position.ReadValue();
            ScreenPos = new Vector3(ClickPos.x, ClickPos.y, 10f);
            return;
        }

        if (Mouse.current != null)
        {
            ClickPos = Mouse.current.position.ReadValue();
            //camera z is -10
            ScreenPos = new Vector3(ClickPos.x, ClickPos.y, 10f);
        }
    }

    private void TriggerOnTouchReleased()
    {
        var touchscreen = Touchscreen.current;
        if (touchscreen == null)
        {
            return;
        }

        bool wasReleased = touchscreen.primaryTouch.press.wasReleasedThisFrame;
        if (wasReleased)
        {
            OnReleased?.Invoke(ScreenPos);
            ResetHoldingState();
        }
    }

    private void TriggerOnMouseReleased()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            OnReleased?.Invoke(ScreenPos);
            ResetHoldingState();
        }
    }

    #region InputState

    private void UpdateStateOnResetHolding()
    {
        InputState = InputState.OnResetHolding;
    }

    private void UpdateStateOnReleased(Vector3 obj)
    {
        InputState = InputState.OnReleased;
    }

    private void UpdateStateOnEndHolding(Vector3 obj)
    {
        InputState = InputState.OnEndHolding;
    }

    private void UpdateStateWhileHolding(float timer)
    {
        InputState = InputState.WhileHolding;
    }

    private void UpdateStateOnStartHolding()
    {
        InputState = InputState.OnStartHolding;
    }

    private void UpdateStateOnStartPressed()
    {
        InputState = InputState.OnStartPressed;
    }

    #endregion
}