using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Twitch",
    description: "Gain {Parry:diff()} [gold]Parry[/gold].{IfUpgraded:show: Draw {DrawCards:plural:card|cards}.|}"
)]
public partial class TwitchCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool IsParryCard => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(Parry), 1M), new DynamicVar(nameof(DrawCards), 0M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Power<ParryPower>()];

    protected override void OnUpgrade() => DrawCards.UpgradeValueTo(1M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ParryCmd.GainParry(
            choiceContext,
            Owner.Creature,
            Parry.BaseValue,
            Owner.Creature,
            this
        );
        await CardPileCmd.Draw(choiceContext, DrawCards.BaseValue, Owner);
    }
}
