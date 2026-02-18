using UnityEngine;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.UI
{
    public class UIScreen : MonoBehaviour, IUIScreen
    {
        protected bool isInitialized;

        public virtual string ScreenName => GetType().Name;

        public void Initialize()
        {
            if (isInitialized) return;
            OnInitialize();
            isInitialized = true;
        }

        protected virtual void OnInitialize() { }

        public virtual void Show()
        {
            if (!isInitialized)
                Initialize();
            gameObject.SetActive(true);
            OnShow();
        }

        protected virtual void OnShow() { }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            OnHide();
        }

        protected virtual void OnHide() { }
    }
}
