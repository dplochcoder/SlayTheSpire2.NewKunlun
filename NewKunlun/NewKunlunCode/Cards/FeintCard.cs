using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Feint",
    description: "Deal {Damage:diff()} damage. {IfUpgraded:show:Draw 1 card. |}Place 1 card from your hand on top of your deck."
)]
public partial class FeintCard()
    : NewKunlunCard(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2M, ValueProp.Move)];

    protected override void OnUpgrade() => Damage.UpgradeValueTo(4M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);

        if (IsUpgraded)
            await CardPileCmd.Draw(choiceContext, Owner);

        var cards = await CardSelectCmd.FromCombatPile(
            choiceContext,
            PileType.Hand.GetPile(Owner),
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1, 1)
        );
        foreach (var card in cards)
            await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
    }
}
