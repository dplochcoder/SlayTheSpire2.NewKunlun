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

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Short Circuit",
    description: "Deal {Damage:diff()} damage. If the target has [gold]Talisman[/gold], play [gold]Talisman Detonate[/gold]."
)]
public partial class ShortCircuitCard()
    : NewKunlunCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(15M, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tips.Talisman(), Tips.TalismanDetonateCard(Owner)];

    protected override void OnUpgrade() => Damage.UpgradeValueTo(20M);

    protected override bool ShouldGlowGoldInternal =>
        CombatState?.Enemies.Any(e => e.HasTalismanFor(Owner)) ?? false;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx(heavy: cardPlay.Target?.HasTalismanFor(Owner) ?? false)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        if (cardPlay.Target!.IsHittable && cardPlay.Target!.HasTalismanFor(Owner))
            await TalismanDetonateCard.AutoPlay(choiceContext, Owner, CombatState!);
    }
}
