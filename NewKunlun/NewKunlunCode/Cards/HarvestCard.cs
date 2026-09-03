using BaseLib.Utils;
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

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Harvest",
    description: "Deal {Damage:diff()} damage. Shuffle an {IfUpgraded:show:[green]Azure Sand+[/green]|[gold]Azure Sand[/gold]} into your discard pile."
)]
public partial class HarvestCard()
    : NewKunlunCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(15M, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.AzureSandCard(upgrade: IsUpgraded)];

    protected override void OnUpgrade() => Damage.UpgradeValueTo(20M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithHeavySlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        await this.AddGeneratedCardToPile<AzureSandCard>(PileType.Discard, upgrade: IsUpgraded);
    }
}
