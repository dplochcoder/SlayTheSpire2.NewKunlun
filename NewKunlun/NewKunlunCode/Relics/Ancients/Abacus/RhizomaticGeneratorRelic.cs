using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Rhizomatic Generator",
    description: "Whenever you shuffle your deck, gain {Energy:energyIcons()}.",
    flavor: ""
)]
public partial class RhizomaticGeneratorRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        Flash();
        await PlayerCmd.GainEnergy(Energy.IntValue, Owner);
    }
}
