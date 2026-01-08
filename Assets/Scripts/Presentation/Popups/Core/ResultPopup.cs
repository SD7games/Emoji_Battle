using System;

public abstract class ResultPopup : PopupBase
{
    public event Action Closed;

    private bool _closed;

    public override void Show()
    {
        _closed = false;
        base.Show();
    }

    public override void Hide()
    {
        if (_closed)
            return;

        _closed = true;

        base.Hide();
        Closed?.Invoke();
    }
}