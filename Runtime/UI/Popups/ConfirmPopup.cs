using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Data passed to <see cref="ConfirmPopup.Show"/>.
    /// </summary>
    public class ConfirmPopupData
    {
        public string Title;
        public string Message;
        public string ConfirmLabel = "OK";
        public string CancelLabel = "Cancel";
        public Action OnConfirm;
        public Action OnCancel;
    }

    /// <summary>
    /// Generic confirmation dialog. Pass <see cref="ConfirmPopupData"/> to Show().
    /// </summary>
    public class ConfirmPopup : PopupBase
    {
        public override string PopupName => "ConfirmPopup";

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private TextMeshProUGUI confirmLabel;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TextMeshProUGUI cancelLabel;

        private Action onConfirm;
        private Action onCancel;

        protected override void Awake()
        {
            base.Awake();

            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirmClicked);
            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelClicked);
        }

        protected override void OnShow(object data)
        {
            if (data is ConfirmPopupData d)
            {
                if (titleText != null) titleText.text = d.Title ?? "";
                if (messageText != null) messageText.text = d.Message ?? "";
                if (confirmLabel != null) confirmLabel.text = d.ConfirmLabel;
                if (cancelLabel != null) cancelLabel.text = d.CancelLabel;
                onConfirm = d.OnConfirm;
                onCancel = d.OnCancel;
            }
        }

        protected override void OnHide()
        {
            onConfirm = null;
            onCancel = null;
        }

        private void OnConfirmClicked()
        {
            onConfirm?.Invoke();
            Hide();
        }

        private void OnCancelClicked()
        {
            onCancel?.Invoke();
            Hide();
        }
    }
}
