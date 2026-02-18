using UnityEngine;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Base class for popup overlays managed by <see cref="PopupManager"/>.
    /// Extend this for custom popups (ConfirmPopup, RewardPopup, etc.).
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PopupBase : MonoBehaviour, IPopup
    {
        public abstract string PopupName { get; }
        public bool IsVisible { get; private set; }

        protected CanvasGroup canvasGroup;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
            SetVisible(false);
        }

        public virtual void Show(object data = null)
        {
            SetVisible(true);
            OnShow(data);
        }

        public virtual void Hide()
        {
            SetVisible(false);
            OnHide();
        }

        protected virtual void OnShow(object data) { }
        protected virtual void OnHide() { }

        private void SetVisible(bool visible)
        {
            IsVisible = visible;
            gameObject.SetActive(visible);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = visible;
                canvasGroup.blocksRaycasts = visible;
            }
        }
    }
}
