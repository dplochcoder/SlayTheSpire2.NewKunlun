using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Regenerate",
    description: "Whenever you play [gold]Talisman Detonate[/gold], gain 1 [gold]Qi Charge[/gold]."
)]
public class RegenerateCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tips.TalismanDetonateCard(Owner), Tips.Power<QiChargePower>()];

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RegeneratePower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
    }
}
