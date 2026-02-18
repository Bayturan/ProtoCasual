using UnityEngine;
using System;
using System.Collections;

namespace ProtoCasual.Core.Utilities
{
    public class Timer
    {
        private float duration;
        private float currentTime;
        private bool isRunning;
        private bool isPaused;
        private Action onComplete;
        private Action<float> onTick;

        public float CurrentTime => currentTime;
        public float Duration => duration;
        public float Progress => duration > 0 ? currentTime / duration : 0;
        public bool IsRunning => isRunning;
        public bool IsPaused => isPaused;

        public Timer(float duration)
        {
            this.duration = duration;
            this.currentTime = 0;
            this.isRunning = false;
            this.isPaused = false;
        }

        public void Start(Action onComplete = null, Action<float> onTick = null)
        {
            this.onComplete = onComplete;
            this.onTick = onTick;
            currentTime = 0;
            isRunning = true;
            isPaused = false;
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Resume()
        {
            isPaused = false;
        }

        public void Stop()
        {
            isRunning = false;
            isPaused = false;
            currentTime = 0;
        }

        public void Update(float deltaTime)
        {
            if (!isRunning || isPaused) return;

            currentTime += deltaTime;
            onTick?.Invoke(currentTime);

            if (currentTime >= duration)
            {
                isRunning = false;
                onComplete?.Invoke();
            }
        }

        public void SetDuration(float newDuration)
        {
            duration = newDuration;
        }

        public void Reset()
        {
            currentTime = 0;
            isRunning = false;
            isPaused = false;
        }
    }

    public class TimerManager : MonoBehaviour
    {
        private static TimerManager instance;
        public static TimerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("TimerManager");
                    instance = go.AddComponent<TimerManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        public static Coroutine WaitForSeconds(float seconds, Action callback)
        {
            return Instance.StartCoroutine(Instance.WaitCoroutine(seconds, callback));
        }

        private IEnumerator WaitCoroutine(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }

        public static Coroutine WaitForFrames(int frames, Action callback)
        {
            return Instance.StartCoroutine(Instance.WaitFramesCoroutine(frames, callback));
        }

        private IEnumerator WaitFramesCoroutine(int frames, Action callback)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
            callback?.Invoke();
        }
    }
}
