﻿using GodSpeedGames.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Move
{
    None,
    Right,
    Left,
    Jump,
    Bend,
    Running,
    StopRunning,
    KeyPause,
}

public enum MatKey
{
    None,
    KeyLeft,
    KeyRight,
    KeyEnter
}

public class InputController : PersistentSingleton<InputController>
{
    public bool buildForMobile = false;

    public static System.Action<Move> OnSetMove;
    public static System.Action<MatKey> OnGotKey;

    private Move _currentMove = Move.None;
    private bool isRunning = false;

    [SerializeField] protected bool isInputActive = false;

    private string _lastMatData = "";

    protected virtual void OnEnable()
    {
        UnityFitmatBridge.OnGotActionFromBridge += OnGotActionFromBridge;
        //SwipeControl.OnSwipe += OnFingerSwipe;
        GSGButtons.OnGotActionFromButton += GotActionFromButton;
    }

    protected virtual void OnDisable()
    {
        UnityFitmatBridge.OnGotActionFromBridge -= OnGotActionFromBridge;
        //SwipeControl.OnSwipe -= OnFingerSwipe;
        GSGButtons.OnGotActionFromButton -= GotActionFromButton;
    }

    public virtual void EnableInput()
    {
        isInputActive = true;
    }

    public virtual void DisableInput()
    {
        isInputActive = false;
    }

    public virtual void PlayMove(Move move)
    {
        OnSetMove?.Invoke(move);
    }

#if UNITY_EDITOR
    protected virtual void Update()
    {
        if (buildForMobile)
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            SetKeyMove(MatKey.KeyRight);

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            SetKeyMove(MatKey.KeyLeft);

        // no need for Keyboard return mapping. It is mapped by default
        //  else if (Input.GetKeyDown(KeyCode.Return))
        //SetKeyMove(MatKey.KeyEnter);

        if (Input.GetAxis("Vertical") > 0)
        {
            isRunning = true;
            SetMove(Move.Running);
        }
        else if (isRunning)
        {
            isRunning = false;
            SetMove(Move.StopRunning);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            SetMove(Move.Jump);
        else if (Input.GetKeyDown(KeyCode.Escape))
            SetMove(Move.KeyPause);

    }
#endif

    protected virtual void GotActionFromButton(GSGButtons.ButtonType button)
    {
        if (button == GSGButtons.ButtonType.Run)
        {
            isRunning = true;
            SetMove(Move.Running);
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.AddPlayerAction("running");
        }
        else if (isRunning)
        {
            isRunning = false;
            SetMove(Move.StopRunning);
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.AddPlayerAction(PlayerSession.PlayerActions.STOP);
        }

        if (button == GSGButtons.ButtonType.Jump)
        {
            SetMove(Move.Jump);
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.AddPlayerAction(PlayerSession.PlayerActions.JUMP);
        }
    }

    protected virtual void OnFingerSwipe(SwipeControl.SwipeDirection direction)
    {
        switch (direction)
        {
            case SwipeControl.SwipeDirection.Null:
                SetMove(Move.None);
                break;

            case SwipeControl.SwipeDirection.Jump:
                SetMove(Move.Jump);
                break;

            case SwipeControl.SwipeDirection.Right:
                SetMove(Move.Right);
                break;

            case SwipeControl.SwipeDirection.Left:
                SetMove(Move.Left);
                break;

            default:
                break;
        }
    }

    protected virtual void OnGotActionFromBridge(string data)
    {
        if (data == "Jumping")
        {
            SetMove(Move.Jump);

            int jump = PlayerData.JumpStep;
            PlayerData.JumpStep = jump + 1;
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.AddPlayerAction(PlayerSession.PlayerActions.JUMP);
        }

        else if (data == "Running")
        {
            SetMove(Move.Running);
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.AddPlayerAction("running");
        }
        else if (data == "Running Stopped")
        {
            SetMove(Move.StopRunning);
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.AddPlayerAction(PlayerSession.PlayerActions.STOP);
        }
        else if (data == "Pause" && _lastMatData != "Pause")
            SetMove(Move.KeyPause);

        else if (data == "Left")
            SetKeyMove(MatKey.KeyLeft);

        else if (data == "Right")
            SetKeyMove(MatKey.KeyRight);

        else if (data == "Enter")
            SetKeyMove(MatKey.KeyEnter);

        _lastMatData = data;
    }

    protected virtual void SetMove(Move move)
    {
        if (isInputActive)
        {
            _currentMove = move;
            OnSetMove?.Invoke(_currentMove);
        }
    }

    protected virtual void SetKeyMove(MatKey move)
    {
        OnGotKey?.Invoke(move);
    }
}
