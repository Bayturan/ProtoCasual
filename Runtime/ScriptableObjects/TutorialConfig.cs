using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    /// <summary>
    /// Configuration for the step-based tutorial system.
    /// Create via: Create → ProtoCasual → Config → Tutorial Config
    /// </summary>
    [CreateAssetMenu(menuName = "ProtoCasual/Config/Tutorial Config")]
    public class TutorialConfig : ScriptableObject
    {
        [Header("General")]
        [Tooltip("Auto-start tutorial on first launch.")]
        public bool autoStart = true;

        [Tooltip("Allow the player to skip the tutorial.")]
        public bool allowSkip = true;

        [Header("Steps")]
        public TutorialStepData[] steps;
    }

    [System.Serializable]
    public class TutorialStepData
    {
        public string stepId;
        public string title;
        [TextArea] public string message;
        public Sprite image;

        [Header("Highlight")]
        [Tooltip("Name of the UI element to highlight (found via FindDeepChild).")]
        public string highlightTarget;

        [Header("Completion")]
        [Tooltip("If true, step completes on any tap. Otherwise call CompleteCurrentStep() manually.")]
        public bool completeOnTap = true;

        [Tooltip("Auto-advance delay in seconds (0 = wait for tap/manual).")]
        public float autoAdvanceDelay;
    }
}
