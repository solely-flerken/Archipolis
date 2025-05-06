using System;
using System.Threading;
using System.Threading.Tasks;
using UI;
using UnityEngine;

namespace State
{
    public static class LoadingStatus
    {
        public static string CurrentStatus = "Initialising...";
        public static float ProgressLerp;
        public static int Progress { get; private set; }

        private static CancellationTokenSource _cts;

        public static void UpdateProgress(int newProgress)
        {
            if (Progress != newProgress)
            {
                return;
            }
            
            Progress = newProgress;
            StartSmoothing();
        }
        
        private static void StartSmoothing()
        {
            _cts?.Cancel(); // cancel any previous smoothing
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            SmoothToTargetAsync(token);
        }

        private static async void SmoothToTargetAsync(CancellationToken token)
        {
            try
            {
                while (Math.Abs(ProgressLerp - Progress) > 0.1f)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    ProgressLerp = Mathf.Lerp(ProgressLerp, Progress, 0.1f);
                    await Task.Delay(16, token);
                }

                ProgressLerp = Progress;
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
            BindableProperty<string>.Bind(() => LoadingStatus.CurrentStatus);

        public readonly BindableProperty<float> ProgressLerp =
            BindableProperty<float>.Bind(() => LoadingStatus.ProgressLerp);
    }
}