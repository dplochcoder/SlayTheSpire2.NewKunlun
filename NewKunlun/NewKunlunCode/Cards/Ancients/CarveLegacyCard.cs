using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

public abstract class CarveLegacyCard()
    : NewKunlunCard(-1, CardType.Power, CardRarity.Ancient, TargetType.AllAllies)
{
    public static CardModel ForPlayer(Player player) =>
        player.Character switch
        {
            Ironclad => ModelDb.Card<CarveLegacyDemonFormCard>(),
            Silent => ModelDb.Card<CarveLegacySerpentFormCard>(),
            Regent => ModelDb.Card<CarveLegacyVoidFormCard>(),
            Necrobinder => ModelDb.Card<CarveLegacyReaperFormCard>(),
            Defect => ModelDb.Card<CarveLegacyEchoFormCard>(),
            Character.Yi => ModelDb.Card<CarveLegacyDemonFormCard>(), // TODO
            _ => ModelDb.Card<CarveLegacyDemonFormCard>(), // TODO
        };

    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    protected override int CanonicalEnergyCost => Owner != null ? Owner.RunState.Players.Count : -1;

    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    protected override bool HasEnergyCostX => Owner == null;

    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.MultiplayerOnly;
}

public abstract partial class CarveLegacyCard<TPower> : CarveLegacyCard
    where TPower : PowerModel
{
    protected abstract int BasePowerAmount { get; }

    protected virtual int UpgradedPowerAmount => BasePowerAmount;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<TPower>(nameof(PowerAmount), BasePowerAmount), new CarveLegacyCostVar()];

    protected override void OnUpgrade() => PowerAmount.UpgradeValueTo(UpgradedPowerAmount);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.CombatState == null)
            return;

        foreach (
            var ally in Owner.Creature.CombatState.Allies.Where(c =>
                c != Owner.Creature && c.IsAlive
            )
        )
            await PowerCmd.Apply<TPower>(
                choiceContext,
                ally,
                PowerAmount.IntValue,
                Owner.Creature,
                this
            );
    }
}

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Carve Legacy",
    description: "All allies gain {PowerAmount:diff()} [gold]Demon Form[/gold].{CarveLegacyCost:carveLegacyCost()}",
    skipValidation: true
)]
public class CarveLegacyDemonFormCard : CarveLegacyCard<DemonFormPower>
{
    protected override int BasePowerAmount => 3;

    protected override int UpgradedPowerAmount => 4;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DemonFormPower>(), HoverTipFactory.FromPower<StrengthPower>()];
}

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Carve Legacy",
    description: "All allies gain {PowerAmount:diff()} [gold]Serpent Form[/gold].{CarveLegacyCost:carveLegacyCost()}",
    skipValidation: true
)]
public class CarveLegacySerpentFormCard : CarveLegacyCard<SerpentFormPower>
{
    protected override int BasePowerAmount => 4;

    protected override int UpgradedPowerAmount => 6;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<SerpentFormPower>()];
}

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Carve Legacy",
    description: "All allies gain {PowerAmount:diff()} [gold]Void Form[/gold] and end their turns.\n{CarveLegacyCost:carveLegacyCost()}",
    skipValidation: true
)]
public class CarveLegacyVoidFormCard : CarveLegacyCard<VoidFormPower>
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override int BasePowerAmount => 2;

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Ethereal);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<VoidFormPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        if (Owner.Creature.CombatState == null)
            return;

        foreach (
            var ally in Owner.Creature.CombatState.Allies.Where(c =>
                c != Owner.Creature && c is { IsAlive: true, Player: not null }
            )
        )
            PlayerCmd.EndTurn(ally.Player!, false);
    }
}

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Carve Legacy",
    description: "All allies gain [gold]Reaper Form[/gold].\n{CarveLegacyCost:carveLegacyCost()}",
    skipValidation: true
)]
public class CarveLegacyReaperFormCard : CarveLegacyCard<ReaperFormPower>
{
    protected override int BasePowerAmount => 1;

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<ReaperFormPower>(), HoverTipFactory.FromPower<DoomPower>()];
}

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Carve Legacy",
    description: "All allies gain [gold]Echo Form[/gold].\n{CarveLegacyCost:carveLegacyCost()}",
    skipValidation: true
)]
public class CarveLegacyEchoFormCard : CarveLegacyCard<EchoFormPower>
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override int BasePowerAmount => 1;

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Ethereal);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<EchoFormPower>()];
}
