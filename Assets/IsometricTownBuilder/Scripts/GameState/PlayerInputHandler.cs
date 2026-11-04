#nullable enable
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private bool isHolding = false;
    [field: SerializeField] public bool MuteInput { get; private set; } = false;

    [SerializeField] private float holdingTimer = 1f;
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
            currentHoldingTime = 0;
            isHolding = true;
            Debug.Log($"{GetType().Name}.UpdateHoldingTimer -> invoking start holding".LogColor(Color.cornflowerBlue));
            OnStartHolding?.Invoke();
        }
        else //while Holding
        {
            WhileHolding?.Invoke(currentHoldingTime);
            currentHoldingTime += Time.deltaTime;

            if (currentHoldingTime >= holdingTimer)
            {
                OnEndHolding?.Invoke(ScreenPos);
            }
        }
    }

    private void ResetHoldingState(
#if UNITY_EDITOR
        [CallerMemberName] string caller = ""
#endif
    )
    {
        isHolding = false;
        currentHoldingTime = 0;
        OnResetHolding?.Invoke();
#if UNITY_EDITOR
        Debug.Log($"{GetType().Name}.ResetHoldingState() -> called from: {caller}".LogColor(Color.orange));
#endif
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
}