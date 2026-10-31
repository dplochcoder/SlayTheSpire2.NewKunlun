using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Events;

[Pool(typeof(CurseCardPool))]
[CardLocalization(
    title: "Anguish",
    description: "If this card is in your hand at the end of your turn, lose {MaxHp} max hp."
)]
public partial class AnguishCard()
    : NewKunlunCard(1, CardType.Curse, CardRarity.Event, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Eternal];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(nameof(MaxHp), 2)];

    public override int MaxUpgradeLevel => 0;

    public override bool HasTurnEndInHandEffect => true;

    protected override Task OnTurnEndInHand(PlayerChoiceContext choiceContext) =>
        CreatureCmd.LoseMaxHp(choiceContext, Owner.Creature, MaxHp.IntValue, true);
}
