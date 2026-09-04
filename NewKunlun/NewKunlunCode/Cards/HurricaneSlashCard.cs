using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Hurricane Slash",
    description: "Deal {Damage:diff()} damage {HitCount:diff()} times to all enemies. Deals damage an additional time for each time you played {TalismanDash:cardName} this combat."
)]
public partial class HurricaneSlashCard()
    : NewKunlunCard(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(6M, ValueProp.Move),
            new CustomVar<HurricaneSlashCard>(
                nameof(HitCount),
                3,
                (card, _) => card.CalculateHitCount()
            ),
            new TalismanDashVar<HurricaneSlashCard>(card =>
                TalismanDashCard.IsUpgradedAnywhere(card.Owner)
            ),
        ];

    private decimal CalculateHitCount() =>
        3
        + CombatManager
            .Instance.History.Entries.OfType<CardPlayFinishedEntry>()
            .Count(e => e.CardPlay.Player == Owner && e.CardPlay.Card is TalismanDashCard);

    protected override void OnUpgrade() => Damage.UpgradeValueTo(8M);

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.TalismanDashCard(Owner)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithHitCount((int)HitCount.Calculate())
            .TargetingAllOpponents(Owner.Creature.CombatState!)
            .Execute(choiceContext);
    }
}
