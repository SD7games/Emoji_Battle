using UnityEngine;
using TMPro;

public sealed class DrawPopup : ResultPopup
{
    public override PopupId Id => PopupId.Draw;

    [SerializeField] private TMP_Text _messageText;

    private static readonly string[] DrawTexts =
    {
        "Nobody won.\n\nNobody lost.\n\nAwkward…",

        "Same moves.\n\nSame result.",

        "Nobody wins.\n\nNobody loses.\n\nThe board is confused.",

        "Equal skills.\n\nEqual mistakes.",

        "That was… \n\nsomething."
    };

    private int _lastIndex = -1;

    public override void Show()
    {
        SetRandomText();
        base.Show();
    }

    private void SetRandomText()
    {
        if (_messageText == null || DrawTexts.Length == 0)
            return;

        int index;
        do
        {
            index = Random.Range(0, DrawTexts.Length);
        }
        while (index == _lastIndex && DrawTexts.Length > 1);

        _lastIndex = index;
        _messageText.text = DrawTexts[index];
    }
}