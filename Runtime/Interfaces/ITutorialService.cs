using System;

namespace ProtoCasual.Core.Interfaces
{
    /// <summary>
    /// Step-based tutorial / onboarding system.
    /// </summary>
    public interface ITutorialService
    {
        event Action<int, string> OnStepStarted;
        event Action<int, string> OnStepCompleted;
        event Action OnTutorialCompleted;

        bool IsTutorialActive { get; }
        bool IsTutorialCompleted { get; }
        int CurrentStepIndex { get; }

        void StartTutorial();
        void CompleteCurrentStep();
        void SkipTutorial();
        void Reset();
    }
}
