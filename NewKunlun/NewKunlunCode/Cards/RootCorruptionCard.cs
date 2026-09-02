using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Root Corruption",
    description: "At the start of your turn, gain 1 energy, draw {CardDraw:diff()} {CardDraw:cond:>1?cards|card}, transform 1 card in your hand into [gold]Malfunction[/gold] and discard it."
)]
public partial class RootCorruptionCard()
    : NewKunlunCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(CardDraw), 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Card<MalfunctionCard>()];

    protected override void OnUpgrade() => CardDraw.UpgradeValueTo(2M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<Powers.RootCorruptionPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
        power?.CardDraw.BaseValue += CardDraw.BaseValue;
    }
}
