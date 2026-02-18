using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Managers;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Settings screen with audio, haptic, and data-reset controls.
    /// Reads/writes AudioManager and IHapticService state.
    /// </summary>
    public class SettingsScreen : UIScreen
    {
        [Header("Sound")]
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("Haptics")]
        [SerializeField] private Toggle vibrationToggle;

        [Header("Data")]
        [SerializeField] private Button resetDataButton;

        [Header("Navigation")]
        [SerializeField] private Button closeButton;

        private IHapticService hapticService;

        protected override void OnInitialize()
        {
            hapticService = ServiceLocator.IsRegistered<IHapticService>()
                ? ServiceLocator.Get<IHapticService>()
                : null;

            // Music
            if (musicToggle != null)
                musicToggle.onValueChanged.AddListener(OnMusicToggled);

            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            // SFX
            if (sfxToggle != null)
                sfxToggle.onValueChanged.AddListener(OnSfxToggled);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

            // Vibration
            if (vibrationToggle != null)
                vibrationToggle.onValueChanged.AddListener(OnVibrationToggled);

            // Reset
            if (resetDataButton != null)
                resetDataButton.onClick.AddListener(OnResetDataClicked);

            // Close
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);
        }

        protected override void OnShow()
        {
            RefreshUI();
        }

        // ─── Refresh ────────────────────────────────────────────────────

        private void RefreshUI()
        {
            var audio = AudioManager.Instance;
            if (audio != null)
            {
                if (musicToggle != null) musicToggle.SetIsOnWithoutNotify(audio.IsMusicEnabled);
                if (sfxToggle != null) sfxToggle.SetIsOnWithoutNotify(audio.IsSfxEnabled);
            }

            if (vibrationToggle != null && hapticService != null)
                vibrationToggle.SetIsOnWithoutNotify(hapticService.IsEnabled);
        }

        // ─── Callbacks ──────────────────────────────────────────────────

        private void OnMusicToggled(bool isOn)
        {
            AudioManager.Instance?.SetMusicEnabled(isOn);
        }

        private void OnMusicVolumeChanged(float value)
        {
            AudioManager.Instance?.SetMusicVolume(value);
        }

        private void OnSfxToggled(bool isOn)
        {
            AudioManager.Instance?.SetSfxEnabled(isOn);
        }

        private void OnSfxVolumeChanged(float value)
        {
            AudioManager.Instance?.SetSfxVolume(value);
        }

        private void OnVibrationToggled(bool isOn)
        {
            if (hapticService != null)
                hapticService.IsEnabled = isOn;
        }

        private void OnResetDataClicked()
        {
            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.ShowPopup("ConfirmPopup", new ConfirmPopupData
                {
                    Title = "Reset Data",
                    Message = "Are you sure you want to reset all progress? This cannot be undone.",
                    ConfirmLabel = "Reset",
                    CancelLabel = "Cancel",
                    OnConfirm = DoResetData
                });
            }
            else
            {
                DoResetData();
            }
        }

        private void DoResetData()
        {
            var save = ServiceLocator.IsRegistered<ISaveService>()
                ? ServiceLocator.Get<ISaveService>()
                : null;
            save?.DeleteAll();

            var dataProvider = ServiceLocator.IsRegistered<Data.PlayerDataProvider>()
                ? ServiceLocator.Get<Data.PlayerDataProvider>()
                : null;
            dataProvider?.Reset();

            Debug.Log("[SettingsScreen] All data reset.");
        }

        private void OnCloseClicked()
        {
            UIManager.Instance.ShowScreen(nameof(MenuScreen));
        }
    }
}
