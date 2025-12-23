using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class BootstrapController
{
    private const string LOBBY_SCENE = "Lobby";
    private const float MIN_SHOW_TIME = 2f;

    private readonly BootstrapView _view;

    public BootstrapController(BootstrapView view)
    {
        _view = view;
        _view.SetProgress(0);
    }

    public async Task StartAsync()
    {
        float startTime = Time.realtimeSinceStartup;

        var operation = SceneManager.LoadSceneAsync(LOBBY_SCENE);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress01 = Mathf.Clamp01(operation.progress / 0.9f);
            _view.SetProgress(Mathf.RoundToInt(progress01 * 100));

            bool sceneReady = operation.progress >= 0.9f;
            bool minTimePassed =
                Time.realtimeSinceStartup - startTime >= MIN_SHOW_TIME;

            if (sceneReady && minTimePassed)
            {
                _view.SetProgress(100);
                operation.allowSceneActivation = true;
            }

            await Task.Yield();
        }
    }
}