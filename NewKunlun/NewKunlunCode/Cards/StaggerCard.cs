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
    title: "Stagger",
    description: "Deal {Damage:diff()} damage. If the target is afflicted with [gold]Talisman[/gold], hit {IfUpgraded:show:three times|twice}."
)]
public partial class StaggerCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8M, ValueProp.Move), new DynamicVar(nameof(Repeat), 2M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Power<TalismanPower>()];

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(9M);
        Repeat.UpgradeValueTo(3M);
    }

    protected override bool ShouldGlowGoldInternal =>
        CombatState?.Enemies.Any(e => e.IsHittable && e.HasTalismanFor(Owner)) ?? false;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var times = cardPlay.Target?.HasTalismanFor(Owner) ?? false ? (int)Repeat.BaseValue : 1;
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .WithHitCount(times)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }
}
