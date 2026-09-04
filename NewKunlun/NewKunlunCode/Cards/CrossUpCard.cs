using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Cross Up",
    description: "Deal {Damage:diff()} damage.\nInflict {Vulnerable:diff()} [gold]Vulnerable[/gold].\nPull {TalismanDash:cardName} into your hand."
)]
public partial class CrossUpCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(7M, ValueProp.Move),
            new DynamicVar(nameof(Vulnerable), 1M),
            new TalismanDashVar<CrossUpCard>(card =>
                TalismanDashCard.IsUpgradedAnywhere(card.Owner)
            ),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.Vulnerable(), Tip.TalismanDashCard(Owner)];

    protected override void OnUpgrade() => Damage.UpgradeValueTo(11M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        if (cardPlay.Target?.IsHittable is true)
            await PowerCmd.Apply<VulnerablePower>(
                choiceContext,
                cardPlay.Target,
                Vulnerable.BaseValue,
                Owner.Creature,
                this
            );

        var card = Owner.FindCard<TalismanDashCard>([PileType.Draw, PileType.Discard]);
        if (card != null)
            await CardPileCmd.Add(card, PileType.Hand.GetPile(Owner), CardPilePosition.Top);
    }
}
