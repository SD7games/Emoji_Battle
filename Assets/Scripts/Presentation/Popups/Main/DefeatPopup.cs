using TMPro;
using UnityEngine;

public sealed class DefeatPopup : ResultPopup
{
    public override PopupId Id => PopupId.Defeat;

    [SerializeField] private TMP_Text _messageText;

    private int _lastIndex = -1;

    private static readonly string[] DefeatTexts =
    {
        "Nice try!",
        "Almost!",
        "That was close!",
        "Keep going!",
        "Don't give up!",
        "Not this time.",
        "Try again.",
        "The opponent\n\n was tough.",
        "Ooops… \n\nnot this time."
    };

    public override void Show()
    {
        SetRandomText();
        base.Show();
    }

    private void SetRandomText()
    {
        if (DefeatTexts.Length == 0)
            return;

        int index;

        do
        {
            index = Random.Range(0, DefeatTexts.Length);
        }
        while (index == _lastIndex && DefeatTexts.Length > 1);

        _lastIndex = index;
        _messageText.text = DefeatTexts[index];
    }
}