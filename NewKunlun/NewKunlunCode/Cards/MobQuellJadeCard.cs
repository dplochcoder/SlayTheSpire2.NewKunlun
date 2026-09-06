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
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Mob Quell Jade",
    description: "{TalismanDash:cardName()} targets all enemies.\nYour next {IfUpgraded:show:2 |}{TalismanDetonate:cardName()}{IfUpgraded:show:s deal| deals} 50% more damage."
)]
public partial class MobQuellJadeCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(DoubleDamages), 1M),
            new CardNameVar<TalismanDashCard>(() => TalismanDashCard.IsUpgradedAnywhere(Owner)),
            new CardNameVar<TalismanDetonateCard>(() =>
                TalismanDetonateCard.IsUpgradedAnywhere(Owner)
            ),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.TalismanDashCard(Owner), Tip.TalismanDetonateCard(Owner)];

    protected override void OnUpgrade() => DoubleDamages.UpgradeValueTo(2M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MobQuellJadePower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<MobQuellJadeDamageMultiplierPower>(
            choiceContext,
            Owner.Creature,
            DoubleDamages.BaseValue,
            Owner.Creature,
            this
        );
    }
}
