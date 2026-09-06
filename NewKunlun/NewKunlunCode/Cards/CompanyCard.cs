using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Company",
    description: "All players [gold]Reload[/gold] {ReloadCount:diff()}.\nCosts 1 less with 2 players."
)]
public partial class CompanyCard()
    : NewKunlunCard(3, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(ReloadCount), 1M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Reload()];

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override void OnUpgrade() => ReloadCount.UpgradeValueTo(2M);

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost
    )
    {
        modifiedCost = originalCost;
        if (
            card != this
            || card.CombatState == null
            || card.CombatState.Players.Count(p => p.Creature.IsAlive) <= 2
        )
            return false;

        --modifiedCost;
        return true;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (
            var player in Owner.Creature.CombatState?.Players.Where(p => p.Creature.IsAlive) ?? []
        )
            await ReloadCmd.Reload(choiceContext, player, ReloadCount.IntValue, this);
    }
}
