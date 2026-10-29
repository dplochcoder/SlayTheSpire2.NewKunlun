using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Relics;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Medicinal Citrine",
    description: "Gain {MaxHp:diff()} max HP.\nPermanently gain 1 [gold]Toxin[/gold]."
)]
public partial class MedicinalCitrineCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Ancient, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(nameof(MaxHp), 5)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Toxin(), .. Tip.Toxic()];

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
