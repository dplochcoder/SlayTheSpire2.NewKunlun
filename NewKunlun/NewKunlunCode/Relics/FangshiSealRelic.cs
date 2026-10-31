using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Fangshi Seal",
    description: "Upgrade all cards in your deck.\nAll cards in your deck become [gold]Eternal[/gold].",
    flavor: ""
)]
public class FangshiSealRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Eternal)];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained()
    {
        foreach (var card in Owner.Deck.Cards)
        {
            card.AddKeyword(CardKeyword.Eternal);
            if (card.IsUpgradable)
                CardCmd.Upgrade(card, CardPreviewStyle.None);
        }

        CardCmd.Preview(Owner.Deck.Cards);
        return Task.CompletedTask;
    }
}
