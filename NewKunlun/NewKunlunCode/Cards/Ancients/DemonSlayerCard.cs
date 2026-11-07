using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Demon Slayer",
    description: "Deal {Damage:diff()} damage.\nPermanently increase damage by {IncreaseDamage:diff()}.\nRemove a random Attack from your deck.\nRemove [gold]Demon Slayer[/gold] if no other attacks remain."
)]
public partial class DemonSlayerCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(43, ValueProp.Move), new(nameof(IncreaseDamage), 13)];

    protected override void OnUpgrade() => IncreaseDamage.UpgradeValueTo(19);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
            return;

        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithHeavySlashVfx()
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        this.Permanently(c => c.Damage.BaseValue += c.IncreaseDamage.IntValue);

        List<CardModel> cards =
        [
            .. Owner.Deck.Cards.Where(c =>
                c is { Type: CardType.Attack, IsRemovable: true } && c != DeckVersion
            ),
        ];
        var removed = Owner.RunState.Rng.CombatCardSelection.NextItem(cards) ?? DeckVersion;
        if (removed != null)
        {
            var inCombat = Owner.PlayerCombatState?.AllCards.FirstOrDefault(c =>
                c.DeckVersion == removed
            );
            if (inCombat != null)
                await CardPileCmd.RemoveFromCombat(inCombat);
            await CardPileCmd.RemoveFromDeck(removed, showPreview: inCombat == null);
        }
    }
}
