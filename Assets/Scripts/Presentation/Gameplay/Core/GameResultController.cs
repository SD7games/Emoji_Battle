using System.Collections;
using UnityEngine;

public sealed class GameResultController
{
    private readonly WinLineView _lines;
    private readonly GameRewardService _rewards;
    private readonly MonoBehaviour _coroutineRunner;
    private readonly InputController _input;

    private const float DRAW_DELAY = 0.8f;
    private const float POPUP_BLOCK_TIME = 0.3f;

    public GameResultController(
        WinLineView lines,
        GameRewardService rewards,
        MonoBehaviour coroutineRunner,
        InputController input)
    {
        _lines = lines;
        _rewards = rewards;
        _coroutineRunner = coroutineRunner;
        _input = input;
    }

    public void HandleGameOver(CellState winner, WinLineView.WinLineType? line)
    {
        _input.Block();

        GameRewardResult reward = _rewards.OnWin(winner);

        if (line.HasValue)
        {
            _lines.ShowWinLine(
                line.Value,
                () => ShowResultPopup(winner, reward)
            );
        }
        else
        {
            _coroutineRunner.StartCoroutine(ShowDrawDelayed(winner, reward));
        }
    }

    private IEnumerator ShowDrawDelayed(CellState winner, GameRewardResult reward)
    {
        yield return new WaitForSeconds(DRAW_DELAY);
        ShowResultPopup(winner, reward);
    }

    private void ShowResultPopup(CellState winner, GameRewardResult reward)
    {
        _coroutineRunner.StartCoroutine(_input.BlockForSeconds(POPUP_BLOCK_TIME));

        if (winner == CellState.Player)
        {
            if (reward.AllEmojisUnlocked)
            {
                PopupService.I.Show(PopupId.Complete);
                return;
            }

            if (reward.EmojiUnlocked)
            {
                PopupService.I.Show(PopupId.Victory);
                return;
            }

            PopupService.I.Show(PopupId.Complete);
            return;
        }

        if (winner == CellState.AI)
        {
            PopupService.I.Show(PopupId.Defeat);
            return;
        }

        PopupService.I.Show(PopupId.Draw);
    }
}