using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Cards.Ancients;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Security Camera",
    description: "On pickup, add two [gold]Security Guards[/gold] to your deck.",
    flavor: ""
)]
public class SecurityCameraRelic : AddCardsToDeckRelic<SecurityGuardCard>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int NumCards => 2;
}
