using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Yi;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Rhizomatic Bomb",
    description: "[glow]Exhaust[/glow] your draw pile or discard pile, whichever is larger.\nDeal {Damage:diff()} to all enemies."
)]
public partial class RhizomaticBombCard()
    : NewKunlunCard(5, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(70, ValueProp.Move)];

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (RunState == null || Owner.Creature.CombatState == null)
            return;

        List<CardModel> draw = [.. PileType.Draw.GetPile(Owner).Cards];
        List<CardModel> discard = [.. PileType.Discard.GetPile(Owner).Cards];
        var pile =
            draw.Count > discard.Count ? draw
            : discard.Count > draw.Count ? discard
            : RunState.Rng.CombatCardSelection.NextBool() ? draw
            : discard;

        await DamageCmd
            .Attack((decimal)Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(Owner.Creature.CombatState)
            .WithHyperbeamVfx(Owner)
            .Execute(choiceContext);
    }
}
