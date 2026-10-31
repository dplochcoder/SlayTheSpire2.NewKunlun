using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Tao Fruit",
    description: "At the start of your turn, gain {Vigor} [gold]Vigor[/gold].",
    flavor: ""
)]
public partial class TaoFruitRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Vigor), 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<VigorPower>()];

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (!participants.Contains(Owner.Creature))
            return;

        Flash();
        await PowerCmd.Apply<VigorPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            Vigor.IntValue,
            Owner.Creature,
            null
        );
    }
}
