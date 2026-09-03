using BaseLib.Extensions;
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
    title: "You're Next",
    description: "Deal {Damage:diff()} damage. If this attack kills an enemy, add one [green]Azure Sand+[/green] on top of your draw pile."
)]
public partial class YoureNextCard()
    : NewKunlunCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(20M, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.AzureSandPower()];

    protected override void OnUpgrade() => Damage.UpgradeValueTo(30M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attack = await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithHeavySlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        var kills = attack
            .Results.SelectMany(list => list)
            .Where(result => result.WasTargetKilled)
            .Select(result => result.Receiver)
            .Distinct()
            .Count();

        for (var i = 0; i < kills; i++)
            await this.AddGeneratedCardToPile<AzureSandCard>(
                PileType.Draw,
                upgrade: true,
                CardPilePosition.Top
            );
    }
}
