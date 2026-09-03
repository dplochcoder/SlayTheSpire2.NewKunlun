using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "One with the Tao",
    description: "At the end of your turn, gain {Block:diff()} block and 1 [gold]Parry[/gold]."
)]
public partial class OneWithTheTaoCard()
    : NewKunlunCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(Block), 12M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Parry()];

    protected override void OnUpgrade() => Block.UpgradeValueTo(16M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<OneWithTheTaoPower>(
            choiceContext,
            Owner.Creature,
            Block.BaseValue,
            Owner.Creature,
            this
        );
        power?.Parry.BaseValue += 1;
    }
}
