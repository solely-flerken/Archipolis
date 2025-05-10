using System;
using System.Threading;
using System.Threading.Tasks;
using UI;
using UnityEngine;

namespace GameInitialization
{
    public static class LoadingProgressManager
    {
        public static string LoadingMessage = "Initialising...";
        public static float SmoothProgress { get; private set; }
        public static float Progress { get; private set; }

        private static CancellationTokenSource _cts;

        public static void UpdateProgress(float newProgress)
        {
            if (Mathf.Approximately(Progress, newProgress))
            {
                return;
            }

            Progress = newProgress;
            StartSmoothing();
        }

        public static void Reset()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            Progress = 0f;
            SmoothProgress = 0f;
        }

        private static void StartSmoothing()
        {
            _cts?.Cancel(); // cancel any previous smoothing
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            _ = SmoothToTargetAsync(token);
        }

        private static async Task SmoothToTargetAsync(CancellationToken token)
        {
            try
            {
                while (Math.Abs(SmoothProgress - Progress) > 0.1f)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    SmoothProgress = Mathf.MoveTowards(SmoothProgress, Progress, 1f);
                    await Task.Delay(16, token); // (1000 / 16) = 60 fps
                }

                SmoothProgress = Progress;
            }
            catch (TaskCanceledException)
            {
                // Do nothing - smoothing was interrupted
            }
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