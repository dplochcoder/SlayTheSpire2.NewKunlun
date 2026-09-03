using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Mob Quell Jade",
    description: "[gold]Talisman Dash[/gold] targets all enemies. Your next {IfUpgraded:show:2 [gold]Talisman Detonate[/gold]s deal|[gold]Talisman Detonate[/gold] deals} double damage."
)]
public partial class MobQuellJadeCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(DoubleDamages), 1M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.TalismanDashCard(Owner), Tip.TalismanDetonateCard(Owner)];

    protected override void OnUpgrade() => DoubleDamages.UpgradeValueTo(2M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MobQuellJadePower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<MobQuellJadeDoubleDamagePower>(
            choiceContext,
            Owner.Creature,
            DoubleDamages.BaseValue,
            Owner.Creature,
            this
        );
    }
}
