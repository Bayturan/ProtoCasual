using System;
using UnityEngine.UIElements;

namespace ProtoCasual.Core.UI.Popups
{
    public class ConfirmPopupData
    {
        public string Title;
        public string Message;
        public string ConfirmLabel = "OK";
        public string CancelLabel = "Cancel";
        public Action OnConfirm;
        public Action OnCancel;
    }

    /// <summary>Generic confirmation dialog — title, message, confirm/cancel buttons.</summary>
    public class ConfirmPopup : PopupController
    {
        public override string PopupName => "ConfirmPopup";

        private Label titleLabel;
        private Label messageLabel;
        private Button confirmBtn;
        private Button cancelBtn;

        private Action onConfirm;
        private Action onCancel;

        protected override void OnBind()
        {
            titleLabel = Lbl("title-text");
            messageLabel = Lbl("message-text");
            confirmBtn = Btn("confirm-btn");
            cancelBtn = Btn("cancel-btn");

            confirmBtn?.RegisterCallback<ClickEvent>(_ => { onConfirm?.Invoke(); Hide(); });
            cancelBtn?.RegisterCallback<ClickEvent>(_ => { onCancel?.Invoke(); Hide(); });
        }

        protected override void OnShow(object data)
        {
            if (data is ConfirmPopupData d)
            {
                if (titleLabel != null) titleLabel.text = d.Title ?? "";
                if (messageLabel != null) messageLabel.text = d.Message ?? "";
                if (confirmBtn != null) confirmBtn.text = d.ConfirmLabel;
                if (cancelBtn != null) cancelBtn.text = d.CancelLabel;
                onConfirm = d.OnConfirm;
                onCancel = d.OnCancel;
            }
        }

        protected override void OnHide()
        {
            onConfirm = null;
            onCancel = null;
        }
    }
}
