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

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Surge",
    description: "Deal {Damage:diff()} damage. Add 1 [gold]Malfunction[/gold] to your discard pile."
)]
public partial class SurgeCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(13M, ValueProp.Move)];

    protected override void OnUpgrade() => Damage.UpgradeValueTo(19M);

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Card<MalfunctionCard>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .WithHeavySlashVfx()
            .Execute(choiceContext);
        await this.AddGeneratedStatusToPile<MalfunctionCard>();
    }
}
