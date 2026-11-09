using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Rhizomatic Capacitor",
    description: "Gain {Charge:energyIcons()} and draw {Charge} cards.\nWhen retained, increase energy and draw values by 1, up to {MaxCharge:diff()}."
)]
public partial class RhizomaticCapacitorCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust, CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(Charge), 1), new(nameof(MaxCharge), 4)];

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
        MaxCharge.UpgradeValueTo(6);
    }

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side != Owner.Creature.Side || Pile?.Type is not PileType.Hand)
            return Task.CompletedTask;

        ++Charge.BaseValue;
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(Charge.IntValue, Owner);
        await CardPileCmd.Draw(choiceContext, Charge.IntValue, Owner);
        await Owner.AddGeneratedCardToPile<Void>(PileType.Draw, position: CardPilePosition.Random);
    }
}
