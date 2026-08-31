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
    description: "Gain {Block:diff()} block. Discard {Discards:diff()} cards. Exhaust {Exhausts:plural:card|cards}.",
    customPromptA: "Select {Discards:plural:card|cards} to discard.",
    customPromptB: "Select {Exhausts:plural:card|cards} to exhaust."
)]
public partial class RetreatCard()
    : NewKunlunCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(16M, ValueProp.Move),
            new DynamicVar(nameof(Discards), 2M),
            new DynamicVar(nameof(Exhausts), 1M),
        ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded ? [CardKeyword.Retain, CardKeyword.Exhaust] : [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);

        var discards = await CardSelectCmd.FromCombatPile(
            choiceContext,
            PileType.Hand.GetPile(Owner),
            cardPlay.Player,
            new CardSelectorPrefs(
                this.CustomPromptA,
                (int)Discards.BaseValue,
                (int)Discards.BaseValue
            )
        );
        foreach (var card in discards)
            await CardCmd.Discard(choiceContext, card);

        var exhausts = await CardSelectCmd.FromCombatPile(
            choiceContext,
            PileType.Hand.GetPile(Owner),
            cardPlay.Player,
            new CardSelectorPrefs(
                this.CustomPromptB,
                (int)Exhausts.BaseValue,
                (int)Exhausts.BaseValue
            )
        );
        foreach (var card in exhausts)
            await CardCmd.Exhaust(choiceContext, card);
    }
}
