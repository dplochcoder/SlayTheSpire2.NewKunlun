using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Cards.Ancients;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Floatation Device",
    description: "At the start of combat, add a [gold]Life Preserver[/gold] into your hand.",
    flavor: ""
)]
public class FloatationDeviceRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState
    )
    {
        if (
            player != Owner
            || Owner.PlayerCombatState?.TurnNumber != 1
            || Owner.Creature.CombatState == null
        )
            return;

        await CardPileCmd.AddGeneratedCardsToCombat(
            [Owner.Creature.CombatState.CreateCard<LifePreserverCard>(Owner)],
            PileType.Hand,
            Owner
        );
    }
}
