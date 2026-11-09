using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Cards.Ancients;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu;

public abstract class WorkbenchRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public static RelicModel? ForPlayer(Player player) =>
        player.Character switch
        {
            Ironclad => ModelDb.Relic<WorkbenchIroncladRelic>(),
            Silent => ModelDb.Relic<WorkbenchSilentRelic>(),
            Regent => ModelDb.Relic<WorkbenchRegentRelic>(),
            Necrobinder => ModelDb.Relic<WorkbenchNecrobinderRelic>(),
            Defect => ModelDb.Relic<WorkbenchDefectRelic>(),
            _ => null,
        };
}

public abstract class WorkbenchRelic<TOld, TNew> : WorkbenchRelic
    where TOld : CardModel
    where TNew : CardModel
{
    private static bool IsOldCard(CardModel card) => card is TOld && card.IsTransformable;

    public override bool IsAllowedForPlayer(Player player) => player.Deck.Cards.Any(IsOldCard);

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<TNew>()];

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        ExtraCardSelectCmds.TransformFromDeckPreservingMutations<TNew>(
            Owner,
            new(CardSelectorPrefs.TransformSelectionPrompt, 1),
            IsOldCard
        );
}

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Workbench",
    description: "On pickup, transform [gold]Bash[/gold] into [gold]Obliterate[/gold].",
    flavor: ""
)]
public class WorkbenchIroncladRelic : WorkbenchRelic<Bash, ObliterateCard> { }

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Workbench",
    description: "On pickup, transform [gold]Survivor[/gold] into [gold]Fortify[/gold].",
    flavor: ""
)]
public class WorkbenchSilentRelic : WorkbenchRelic<Survivor, FortifyCard> { }

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Workbench",
    description: "On pickup, transform [gold]Venerate[/gold] into [gold]Convert[/gold].",
    flavor: ""
)]
public class WorkbenchRegentRelic : WorkbenchRelic<Venerate, ConvertCard> { }

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Workbench",
    description: "On pickup, transform [gold]Bodyguard[/gold] into [gold]Growth[/gold].",
    flavor: ""
)]
public class WorkbenchNecrobinderRelic : WorkbenchRelic<Bodyguard, GrowthCard> { }

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Workbench",
    description: "On pickup, transform [gold]Zap[/gold] into [gold]Fork Bomb[/gold].",
    flavor: ""
)]
public class WorkbenchDefectRelic : WorkbenchRelic<Zap, ForkBombCard> { }
