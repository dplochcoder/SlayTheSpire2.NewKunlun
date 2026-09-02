using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Schematics",
    description: "Whenever you inflict [gold]Internal Damage[/gold] on the enemy, inflict {InternalDamage:diff()} more."
)]
public partial class SchematicsCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(InternalDamage), 3M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.InternalDamage()];

    protected override void OnUpgrade() => InternalDamage.UpgradeValueTo(5M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SchematicsPower>(
            choiceContext,
            Owner.Creature,
            InternalDamage.BaseValue,
            Owner.Creature,
            this
        );
    }
}
