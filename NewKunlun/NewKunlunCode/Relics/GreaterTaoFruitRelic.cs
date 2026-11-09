using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Greater Tao Fruit",
    description: "Every time you shuffle your deck, gain {Strength} [gold]Strength[/gold].",
    flavor: ""
)]
public partial class GreaterTaoFruitRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<StrengthPower>(nameof(Strength), 2)];

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        Flash();
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner.Creature,
            Strength.IntValue,
            Owner.Creature,
            null
        );
    }
}
