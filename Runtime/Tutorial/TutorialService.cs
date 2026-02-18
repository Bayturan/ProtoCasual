using System;
using UnityEngine;
using ProtoCasual.Core.Data;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Tutorial
{
    /// <summary>
    /// Step-based tutorial / onboarding service.
    /// Reads steps from <see cref="TutorialConfig"/> and persists completion in
    /// <see cref="PlayerData.TutorialData"/>.
    /// Register as <see cref="ITutorialService"/> in ServiceLocator.
    /// </summary>
    public class TutorialService : ITutorialService
    {
        public event Action<int, string> OnStepStarted;
        public event Action<int, string> OnStepCompleted;
        public event Action OnTutorialCompleted;

        private readonly TutorialConfig config;
        private readonly TutorialSaveData data;
        private readonly PlayerDataProvider dataProvider;
        private readonly IAnalyticsService analytics;

        public bool IsTutorialActive { get; private set; }
        public bool IsTutorialCompleted => data.IsCompleted;
        public int CurrentStepIndex => data.CurrentStep;

        public TutorialService(TutorialConfig config, PlayerDataProvider dataProvider,
            IAnalyticsService analytics = null)
        {
            this.config = config;
            this.dataProvider = dataProvider;
            this.analytics = analytics;
            data = dataProvider.Data.Tutorial;
        }

        public void StartTutorial()
        {
            if (data.IsCompleted || config == null || config.steps == null || config.steps.Length == 0)
                return;

            IsTutorialActive = true;
            data.CurrentStep = 0;
            dataProvider.Save();

            var step = config.steps[0];
            OnStepStarted?.Invoke(0, step.stepId);
            analytics?.TutorialStep(0, step.stepId);
            Debug.Log($"[TutorialService] Started — step 0: {step.stepId}");
        }

        public void CompleteCurrentStep()
        {
            if (!IsTutorialActive || config == null) return;

            int idx = data.CurrentStep;
            if (idx < 0 || idx >= config.steps.Length) return;

            var step = config.steps[idx];
            OnStepCompleted?.Invoke(idx, step.stepId);
            analytics?.TutorialStep(idx, step.stepId);
            Debug.Log($"[TutorialService] Completed step {idx}: {step.stepId}");

            int next = idx + 1;
            if (next >= config.steps.Length)
            {
                FinishTutorial();
            }
            else
            {
                data.CurrentStep = next;
                dataProvider.Save();

                var nextStep = config.steps[next];
                OnStepStarted?.Invoke(next, nextStep.stepId);
                analytics?.TutorialStep(next, nextStep.stepId);
            }
        }

        public void SkipTutorial()
        {
            if (config != null && !config.allowSkip) return;
            FinishTutorial();
            Debug.Log("[TutorialService] Tutorial skipped.");
        }

        public void Reset()
        {
            data.IsCompleted = false;
            data.CurrentStep = 0;
            IsTutorialActive = false;
            dataProvider.Save();
        }

        private void FinishTutorial()
        {
            data.IsCompleted = true;
            IsTutorialActive = false;
            dataProvider.Save();
            OnTutorialCompleted?.Invoke();
            Debug.Log("[TutorialService] Tutorial completed.");
        }
    }
}
