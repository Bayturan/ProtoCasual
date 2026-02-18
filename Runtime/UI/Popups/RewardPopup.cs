using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Data passed to <see cref="RewardPopup.Show"/>.
    /// </summary>
    public class RewardPopupData
    {
        public string Title = "Reward!";
        public RewardEntry[] Rewards;
    }

    /// <summary>
    /// Shows granted rewards (coins, gems, items) in a popup.
    /// </summary>
    public class RewardPopup : PopupBase
    {
        public override string PopupName => "RewardPopup";

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Transform rewardContainer;
        [SerializeField] private GameObject rewardEntryPrefab;
        [SerializeField] private Button collectButton;

        protected override void Awake()
        {
            base.Awake();
            if (collectButton != null)
                collectButton.onClick.AddListener(() => Hide());
        }

        protected override void OnShow(object data)
        {
            ClearContainer();

            if (data is RewardPopupData d)
            {
                if (titleText != null) titleText.text = d.Title;

                if (d.Rewards != null && rewardEntryPrefab != null && rewardContainer != null)
                {
                    foreach (var reward in d.Rewards)
                    {
                        if (reward == null) continue;
                        var go = Instantiate(rewardEntryPrefab, rewardContainer);
                        var label = go.GetComponentInChildren<TextMeshProUGUI>();
                        if (label != null)
                        {
                            string typeName = reward.Type switch
                            {
                                RewardType.SoftCurrency => "Coins",
                                RewardType.HardCurrency => "Gems",
                                RewardType.Item => reward.RewardId,
                                _ => "Reward"
                            };
                            label.text = $"+{reward.Amount} {typeName}";
                        }
                    }
                }
            }
        }

        protected override void OnHide()
        {
            ClearContainer();
        }

        private void ClearContainer()
        {
            if (rewardContainer == null) return;
            for (int i = rewardContainer.childCount - 1; i >= 0; i--)
                Destroy(rewardContainer.GetChild(i).gameObject);
        }
    }
}
