using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Afflictions;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Medicinal Citrine",
    description: "Gain {MaxHp:diff()} max HP.\nPermanently gain 1 [gold]Toxin[/gold]."
)]
public partial class MedicinalCitrineCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Ancient, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(nameof(MaxHp), 5)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<ToxinPower>(),
            .. HoverTipFactory.FromAffliction<ToxicAffliction>(),
        ];

    protected override void OnUpgrade() => MaxHp.UpgradeValueTo(7);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, MaxHp.IntValue);

        var relic = Owner.Relics.OfType<OrpimentRelic>().FirstOrDefault();
        if (relic == null)
            return;

        relic.NumCitrineUses++;
        relic.Flash();
        await PowerCmd.Apply<ToxinPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }
}
