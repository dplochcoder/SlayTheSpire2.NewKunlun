using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Metal Detector",
    description: "Choose 1 of 3 random cards in your discard pile to transform into {DarkSteel:cardName()}."
)]
public partial class MetalDetectorCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardNameVar<DarkSteelCard>(() => IsUpgraded)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.DarkSteel(upgrade: IsUpgraded)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IReadOnlyList<CardModel> options =
        [
            .. PileType
                .Discard.GetPile(Owner)
                .Cards.Where(c => c.IsTransformable)
                .ToList()
                .StableShuffle(Owner.RunState.Rng.CombatCardSelection)
                .Take(3),
        ];
        var card = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner);
        if (card == null)
            return;

        var replacement = CombatState!.CreateCard<DarkSteelCard>(Owner);
        if (IsUpgraded)
            CardCmd.Upgrade(replacement);

        await CardCmd.Transform(card, replacement);
    }
}
