using System;
using UnityEngine;

public class GameOverMenu : BaseMenu
{
    private Action _backCallback;
    private Action _playCallback;

    public void Setup(Action backCallback, Action playCallback)
    {
        base.Setup();
        _backCallback = backCallback;
        _playCallback = playCallback;
    }

    public void Play()
    {
        Close();
        _playCallback?.Invoke();
    }
    
    public void Back()
    {
        Close();
        _backCallback?.Invoke();
    }
}
