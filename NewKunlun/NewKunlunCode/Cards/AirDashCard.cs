using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Air Dash",
    description: "Gain {Block:diff()} block. Gain {Dexterity:diff()} [gold]Dexterity[/gold] this turn."
)]
public partial class AirDashCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(7M, ValueProp.Move), new DynamicVar(nameof(Dexterity), 3M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Dexterity()];

    protected override void OnUpgrade()
    {
        Block.UpgradeValueTo(10M);
        Dexterity.UpgradeValueTo(5M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await PowerCmd.Apply<AirDashPower>(
            choiceContext,
            Owner.Creature,
            Dexterity.BaseValue,
            Owner.Creature,
            this
        );
    }
}
