using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Charge Strike",
    description: "Deal {BaseDamage:diff()} damage. Spend 1 [gold]Qi Charge[/gold] to deal {ChargeDamage:diff()} instead."
)]
public partial class ChargeStrikeCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(nameof(BaseDamage), 8M, ValueProp.Move),
            new DamageVar(nameof(ChargeDamage), 20M, ValueProp.Move),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.QiCharge()];

    protected override void OnUpgrade()
    {
        BaseDamage.UpgradeValueTo(11M);
        ChargeDamage.UpgradeValueTo(26M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var charges = await QiChargeCmd.ConsumeQiCharges(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );

        var attack = DamageCmd
            .Attack(charges > 0 ? ChargeDamage.BaseValue : BaseDamage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!);
        attack = charges > 0 ? attack.WithHeavySlashVfx() : attack.WithSlashVfx();
        await attack.Execute(choiceContext);
    }
}
