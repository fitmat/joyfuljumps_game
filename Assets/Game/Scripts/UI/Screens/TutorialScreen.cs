﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI speedText;

    [SerializeField] TextMeshProUGUI animationText;
    [SerializeField] GameObject animationHolder;

    public GameDataTracker tracker;

    [SerializeField] private Animator _animationAnimator;

    private int _previousDistance = -1;

    private void OnEnable()
    {
        TutorialBlock.OnShowTutorialMsg += ShowTutorialMSG;
        LevelManager.OnGameEvent += OnGameEvent;
    }

    private void OnDisable()
    {
        TutorialBlock.OnShowTutorialMsg -= ShowTutorialMSG;
        LevelManager.OnGameEvent -= OnGameEvent;
    }

    private void Start()
    {
        speedText.text = "";
        animationHolder.SetActive(false);

        ShowTutorialMSG("Welcome", "none");
    }

    void ShowTutorialMSG(string msg, string animparameter)
    {
        animationText.text = msg;
        if (animparameter !=  "none")
        {
            if (!animationHolder.activeInHierarchy)
                animationHolder.SetActive(true);
            _animationAnimator.SetTrigger(animparameter);
        }
        else
        {
            if (animationHolder.activeInHierarchy)
                animationHolder.SetActive(false);
        }
    }

    void OnGameEvent(GSGGameEvent _event)
    {
        ShowTutorialMSG("", "none");
        if (_event == GSGGameEvent.LevelOver)
        {
            UiManager.Instance.LoadPopup(UiManager.Popup.GameOverScreen);
        }
    }

    private void Update()
    {
        if (_previousDistance != tracker.distanceCovered)
        {
            _previousDistance = tracker.distanceCovered;
            speedText.text = "" + _previousDistance;
        }
    }
}
