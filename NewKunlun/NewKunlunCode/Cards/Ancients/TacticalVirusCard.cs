using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Tactical Virus",
    description: "All enemies lose {StrengthLoss} [gold]Strength[/gold] this turn.\nAll players gain {StrengthGain} [gold]Strength[/gold] this turn.\nShuffle 1 [gold]Void[/gold] into your draw pile."
)]
public partial class TacticalVirusCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Ancient, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<StrengthPower>(nameof(StrengthLoss), 8),
            new PowerVar<StrengthPower>(nameof(StrengthGain), 4),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromCard<Void>()];

    protected override void OnUpgrade()
    {
        StrengthLoss.UpgradeValueTo(11);
        StrengthGain.UpgradeValueTo(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TacticalVirusStrengthLossPower>(
            choiceContext,
            Owner.Creature.CombatState?.Enemies ?? [],
            StrengthLoss.IntValue,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<TacticalVirusStrengthGainPower>(
            choiceContext,
            Owner.Creature.CombatState?.Allies ?? [],
            StrengthGain.IntValue,
            Owner.Creature,
            this
        );
        await Owner.AddGeneratedCardToPile<Void>(PileType.Draw, position: CardPilePosition.Random);
    }
}
