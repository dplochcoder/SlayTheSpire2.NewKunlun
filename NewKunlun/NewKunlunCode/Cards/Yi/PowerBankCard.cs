using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace NewKunlun.NewKunlunCode.Cards.Yi;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Power Bank",
    description: "Draw {NumCards:diff()} cards.\nShuffle a [gold]Void[/gold] into your draw pile."
)]
public partial class PowerBankCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(NumCards), 3M)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Void>()];

    protected override void OnUpgrade() => NumCards.UpgradeValueTo(4M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, NumCards.BaseValue, Owner);
        await Owner.AddGeneratedCardToPile<Void>(PileType.Draw, position: CardPilePosition.Random);
    }
}
