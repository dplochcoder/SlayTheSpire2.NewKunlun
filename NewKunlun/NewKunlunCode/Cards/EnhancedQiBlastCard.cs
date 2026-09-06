using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Enhanced Qi Blast",
    description: "Whenever you [gold]Discharge[/gold] 3 or more while [gold]Detonating[/gold], add 1 {AzureSand:cardName()} on top of your draw pile."
)]
public partial class EnhancedQiBlastCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardNameVar<AzureSandCard>(() => IsUpgraded)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.Discharge(), Tip.Detonate(), Tip.AzureSand(upgrade: IsUpgraded)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var existing = Owner
            .Creature.GetPowerInstances<EnhancedQiBlastPower>()
            .FirstOrDefault(p => p.IsUpgraded == IsUpgraded);
        if (existing != null)
            await PowerCmd.ModifyAmount(choiceContext, existing, 1M, Owner.Creature, this);
        else
        {
            var power = await PowerCmd.Apply<EnhancedQiBlastPower>(
                choiceContext,
                Owner.Creature,
                1M,
                Owner.Creature,
                this
            );
            power?.IsUpgraded = IsUpgraded;
        }
    }
}
