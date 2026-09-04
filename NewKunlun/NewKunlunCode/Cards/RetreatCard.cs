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
    title: "Retreat",
    description: "Gain {Block:diff()} block. Discard 2 cards. Exhaust 1 card from your hand.",
    customPromptA: "Select 2 cards to discard.",
    customPromptB: "Select 1 card to exhaust."
)]
public partial class RetreatCard()
    : NewKunlunCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(16M, ValueProp.Move)];

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);

        await CardSelectCmd.FromHandForDiscard(
            choiceContext,
            Owner,
            new CardSelectorPrefs(this.CustomPromptA, 2),
            _ => true,
            this
        );
        var exhausts = await CardSelectCmd.FromHand(
            choiceContext,
            cardPlay.Player,
            new CardSelectorPrefs(this.SelectionScreenPrompt, 1),
            _ => true,
            this
        );
        foreach (var card in exhausts)
            await CardCmd.Exhaust(choiceContext, card);
    }
}
