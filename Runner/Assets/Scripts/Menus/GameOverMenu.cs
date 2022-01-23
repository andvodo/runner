using System;

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
        _playCallback?.Invoke();
    }
    
    public void Back()
    {
        _backCallback?.Invoke();
        Close();
    }
}
