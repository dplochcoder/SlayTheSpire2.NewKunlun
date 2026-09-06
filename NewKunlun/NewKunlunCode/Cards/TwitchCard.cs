using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Twitch",
    description: "Gain 1 [gold]Parry[/gold].{IfUpgraded:show:\n[green]Draw 1 card.[/green]|}"
)]
public partial class TwitchCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool IsParryCard => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Parry()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ParryCmd.GainParry(choiceContext, Owner.Creature, 1M, Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, Owner);
    }
}
