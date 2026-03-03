using UnityEngine.UIElements;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.UI.Popups
{
    public class RewardPopupData
    {
        public string Title = "Reward!";
        public RewardEntry[] Rewards;
    }

    /// <summary>Reward display popup — shows earned rewards with a collect button.</summary>
    public class RewardPopup : PopupController
    {
        public override string PopupName => "RewardPopup";

        private Label titleLabel;
        private VisualElement rewardContainer;

        protected override void OnBind()
        {
            titleLabel = Lbl("title-text");
            rewardContainer = Q("reward-container");
            Btn("collect-btn")?.RegisterCallback<ClickEvent>(_ => Hide());
        }

        protected override void OnShow(object data)
        {
            rewardContainer?.Clear();

            if (data is RewardPopupData d)
            {
                if (titleLabel != null) titleLabel.text = d.Title;
                if (d.Rewards != null && rewardContainer != null)
                {
                    foreach (var reward in d.Rewards)
                    {
                        if (reward == null) continue;
                        string typeName = reward.Type switch
                        {
                            RewardType.SoftCurrency => "Coins",
                            RewardType.HardCurrency => "Gems",
                            RewardType.Item => reward.RewardId,
                            _ => "Reward"
                        };
                        var entry = new Label($"+{reward.Amount} {typeName}");
                        entry.AddToClassList("reward-entry");
                        rewardContainer.Add(entry);
                    }
                }
            }
        }

        protected override void OnHide()
        {
            rewardContainer?.Clear();
        }
    }
}
