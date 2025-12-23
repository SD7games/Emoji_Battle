using TMPro;
using UnityEngine;

public sealed class DefeatPopup : ResultPopup
{
    public override PopupId Id => PopupId.Defeat;

    [SerializeField] private TMP_Text _messageText;

    private static readonly string[] DefeatTexts =
    {
        "Nice try!",
        "Almost!",
        "That was close!",
        "Keep going!",
        "Don't give up!",
        "Not this time.",
        "Try again.",
        "The opponent was tough.",
        "Ooops… not this time."
    };

    public override void Show()
    {
        SetRandomText();
        base.Show();
    }

    private void SetRandomText()
    {
        int index = Random.Range(0, DefeatTexts.Length);
        _messageText.text = DefeatTexts[index];
    }
}