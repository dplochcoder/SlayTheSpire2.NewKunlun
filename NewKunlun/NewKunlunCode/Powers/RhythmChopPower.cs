using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Rhythm Chop",
    description: "Whenever you have 3 or more [gold]Qi Charges[/gold] at the start of your turn, pull {TalismanDash:cardName()} into your hand."
)]
public partial class RhythmChopPower : NewKunlunPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new TalismanDashVar<RhythmChopPower>(power =>
                TalismanDashCard.IsUpgradedAnywhere(power.Owner.Player)
            ),
        ];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState
    )
    {
        if (player.Creature.GetPowerAmount<QiChargePower>() < 3)
            return;

        var card = player.FindCard<TalismanDashCard>([
            PileType.Discard,
            PileType.Draw,
            PileType.Hand,
        ]);
        if (card != null)
            await CardPileCmd.Add(card, PileType.Hand.GetPile(player), CardPilePosition.Top);
    }
}
