using System;
using System.Collections;
using UnityEngine;

public sealed class InternetService : MonoBehaviour
{
    public static InternetService I { get; private set; }

    public static event Action<bool> OnlineStateChanged;

    private bool _lastState;

    public static bool IsOnline =>
        Application.internetReachability != NetworkReachability.NotReachable;

    private void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        _lastState = IsOnline;
        StartCoroutine(CheckRoutine());
    }

    private IEnumerator CheckRoutine()
    {
        var wait = new WaitForSeconds(2f);

        while (true)
        {
            bool current = IsOnline;

            if (current != _lastState)
            {
                _lastState = current;
                OnlineStateChanged?.Invoke(current);
            }

            yield return wait;
        }
    }
}