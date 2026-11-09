using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Lear;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Tiger Effigy",
    description: "While you have no [gold]Block[/gold], your attacks deal {Percent}% more damage.",
    flavor: ""
)]
public partial class TigerEffigyRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Percent), 50)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.Static(StaticHoverTip.Block)];

    public override Task BeforeCombatStart()
    {
        Owner.Creature.BlockChanged += OnBlockChanged;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Owner.Creature.BlockChanged -= OnBlockChanged;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    private void OnBlockChanged(int prev, int next) =>
        Status = next > 0 ? RelicStatus.Normal : RelicStatus.Active;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        if (dealer != Owner.Creature || !props.IsPoweredAttack() || dealer.Block > 0)
            return amount;
        else
            return amount * (1 + Percent.IntValue / (decimal)100);
    }
}
