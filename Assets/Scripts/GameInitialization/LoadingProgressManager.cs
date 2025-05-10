using System;
using System.Collections;
using UI;
using UnityEngine;

namespace GameInitialization
{
    public class LoadingProgressManager : MonoBehaviour
    {
        public static LoadingProgressManager Instance;

        public static string LoadingMessage = string.Empty;
        public static float SmoothProgress { get; private set; }
        public static float Progress { get; private set; }

        private Coroutine _smoothProgressCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public void Reset()
        {
            StopSmoothing();
            Progress = 0f;
            SmoothProgress = 0f;
            LoadingMessage = string.Empty;
        }

        public void UpdateProgress(float newProgress)
        {
            if (Mathf.Approximately(Progress, newProgress))
            {
                return;
            }

            Progress = newProgress;

            _smoothProgressCoroutine = StartCoroutine(SmoothProgressCoroutine());
        }

        private static IEnumerator SmoothProgressCoroutine()
        {
            while (Math.Abs(SmoothProgress - Progress) > 0.1f)
            {
                SmoothProgress = Mathf.MoveTowards(SmoothProgress, Progress, 1);
                yield return null;
            }

            SmoothProgress = Progress;
        }

        private void StopSmoothing()
        {
            if (_smoothProgressCoroutine == null)
            {
                return;
            }

            StopCoroutine(_smoothProgressCoroutine);
            _smoothProgressCoroutine = null;
        }
    }

    public class ViewModelLoadingStatus
    {
        public readonly BindableProperty<string> CurrentStatus =
            BindableProperty<string>.Bind(() => LoadingProgressManager.LoadingMessage);

        public readonly BindableProperty<float> ProgressLerp =
            BindableProperty<float>.Bind(() => LoadingProgressManager.SmoothProgress);
    }
}