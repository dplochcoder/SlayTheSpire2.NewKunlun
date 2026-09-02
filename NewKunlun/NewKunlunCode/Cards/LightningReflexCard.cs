using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Lightning Reflex",
    description: "Gain {Block:diff()} block. Shuffle {NumCards:diff()} [green]Twitch+[/green] with {AdroitAmount:diff()} [gold]Adroit[/gold] into your draw pile."
)]
public partial class LightningReflexCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(3M, ValueProp.Move),
            new DynamicVar(nameof(NumCards), 2M),
            new DynamicVar(nameof(AdroitAmount), 3M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tips.Card<TwitchCard>(upgraded: true), .. Tips.Adroit()];

    protected override void OnUpgrade()
    {
        Block.UpgradeValueTo(4M);
        NumCards.UpgradeValueTo(3M);
        AdroitAmount.UpgradeValueTo(4M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cards = [];
        for (int i = 0; i < NumCards.BaseValue; i++)
        {
            var card = CombatState!.CreateCard<TwitchCard>(Owner);
            CardCmd.Upgrade(card);
            CardCmd.Enchant<Adroit>(card, AdroitAmount.BaseValue);
            cards.Add(card);
        }

        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardsToCombat(
                cards,
                PileType.Draw,
                Owner,
                CardPilePosition.Random
            )
        );
    }
}
