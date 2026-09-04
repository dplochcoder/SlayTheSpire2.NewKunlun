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
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Take You With Me",
    description: "Deal {Damage:diff()} damage. Deals {ExtraDamage:diff()} additional damage for each [gold]Internal Damage[/gold] you have."
)]
public partial class TakeYouWithMeCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(BaseDamage), 10M),
            new DynamicVar(nameof(ExtraDamage), 3M),
            new CustomDamageVar<TakeYouWithMeCard>(
                nameof(Damage),
                10M,
                ValueProp.Move,
                (card, _) =>
                    BaseDamage.BaseValue
                    + ExtraDamage.BaseValue
                        * card.Owner.Creature.GetPowerAmount<InternalDamagePower>()
            ),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.InternalDamage()];

    protected override void OnUpgrade() => ExtraDamage.UpgradeValueTo(4M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.Calculate(cardPlay.Target!))
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }
}
