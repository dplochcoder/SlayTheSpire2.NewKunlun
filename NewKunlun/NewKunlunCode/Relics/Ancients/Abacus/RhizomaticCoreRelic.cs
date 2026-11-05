using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Rhizomatic Core",
    description: "On pickup, gain 1 upgraded card reward.",
    flavor: ""
)]
public class RhizomaticCoreRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        List<CardModel> cards =
        [
            .. CardFactory
                .CreateForReward(
                    Owner,
                    3,
                    new CardCreationOptions(
                        [Owner.Character.CardPool],
                        CardCreationSource.Other,
                        CardRarityOddsType.RegularEncounter
                    ).WithFlags(CardCreationFlags.NoUpgradeRoll)
                )
                .Select(r => r.Card),
        ];
        foreach (var card in cards)
            card.UpgradeIfPossible(CardPreviewStyle.None);

        await ExtraCardSelectCmds.AddToDeckFromChooseACardRewardScreen(Owner, cards);
    }
}
