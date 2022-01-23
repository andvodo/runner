using System;
using TMPro;
using UnityEngine;

public class MainMenu : BaseMenu
{
    [SerializeField] private TMP_Text _highscoreLabel;
    [SerializeField] private GameObject _highscoreNode;

    private Action _playCallback;

    public void Open()
    {
        base.Open();
        int highscore = PlayerPrefs.GetInt("highscore");
        _highscoreNode.SetActive(highscore > 0);
        _highscoreLabel.text = highscore.ToString();
    }
    
    public void Setup(Action playCallback)
    {
        base.Setup();
        _playCallback = playCallback;
    }

    public void Play()
    {
        _playCallback?.Invoke();
    }
}
