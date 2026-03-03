using UnityEngine;
using UnityEngine.UIElements;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.UI.Popups;

namespace ProtoCasual.Core.UI.Screens
{
    /// <summary>Settings screen — audio toggles/sliders, vibration, reset data.</summary>
    public class SettingsScreen : ScreenController
    {
        public override string ScreenName => "SettingsScreen";

        private Toggle musicToggle;
        private Slider musicVolSlider;
        private Toggle sfxToggle;
        private Slider sfxVolSlider;
        private Toggle vibrationToggle;

        private IHapticService hapticService;

        protected override void OnBind()
        {
            ServiceLocator.TryGet<IHapticService>(out hapticService);

            musicToggle = Tgl("music-toggle");
            musicVolSlider = Sld("music-volume");
            sfxToggle = Tgl("sfx-toggle");
            sfxVolSlider = Sld("sfx-volume");
            vibrationToggle = Tgl("vibration-toggle");

            musicToggle?.RegisterValueChangedCallback(e => AudioManager.Instance?.SetMusicEnabled(e.newValue));
            musicVolSlider?.RegisterValueChangedCallback(e => AudioManager.Instance?.SetMusicVolume(e.newValue));
            sfxToggle?.RegisterValueChangedCallback(e => AudioManager.Instance?.SetSfxEnabled(e.newValue));
            sfxVolSlider?.RegisterValueChangedCallback(e => AudioManager.Instance?.SetSfxVolume(e.newValue));
            vibrationToggle?.RegisterValueChangedCallback(e =>
            {
                if (hapticService != null) hapticService.IsEnabled = e.newValue;
            });

            Btn("reset-data-btn")?.RegisterCallback<ClickEvent>(OnResetDataClicked);
            Btn("close-btn")?.RegisterCallback<ClickEvent>(OnCloseClicked);
        }

        public override void OnShow() { RefreshUI(); }

        private void RefreshUI()
        {
            var audio = AudioManager.Instance;
            if (audio != null)
            {
                musicToggle?.SetValueWithoutNotify(audio.IsMusicEnabled);
                sfxToggle?.SetValueWithoutNotify(audio.IsSfxEnabled);
            }
            if (hapticService != null)
                vibrationToggle?.SetValueWithoutNotify(hapticService.IsEnabled);
        }

        private void OnResetDataClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            UIToolkitManager.Instance?.ShowPopup("ConfirmPopup", new ConfirmPopupData
            {
                Title = "Reset Data",
                Message = "Are you sure you want to reset all progress? This cannot be undone.",
                ConfirmLabel = "Reset",
                CancelLabel = "Cancel",
                OnConfirm = DoReset
            });
        }

        private void DoReset()
        {
            ServiceLocator.TryGet<ISaveService>(out var save);
            save?.DeleteAll();
            ServiceLocator.TryGet<Data.PlayerDataProvider>(out var dp);
            dp?.Reset();
            Debug.Log("[SettingsScreen] All data reset.");
        }

        private void OnCloseClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            UIToolkitManager.Instance?.ShowScreen("MainScreen");
        }
    }
}
