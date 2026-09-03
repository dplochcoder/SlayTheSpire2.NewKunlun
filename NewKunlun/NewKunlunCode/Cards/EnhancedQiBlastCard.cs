using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Enhanced Qi Blast",
    description: "Whenever you spend 3 or more [gold]Qi Charges[/gold] on [gold]Talisman Detonate[/gold], place 1 {IfUpgraded:show:[green]Azure Sand+[/green]|[gold]Azure Sand[/gold]} on top of your draw pile."
)]
public class EnhancedQiBlastCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.QiCharge(), Tip.TalismanDetonateCard(Owner), Tip.AzureSandCard(upgrade: IsUpgraded)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<EnhancedQiBlastPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
        power?.Upgraded.BaseValue = IsUpgraded ? 1 : 0;
    }
}
