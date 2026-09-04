using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Avarice Jade",
    description: "Gain {Gold:diff()} gold whenever a fatal blow is dealt."
)]
public partial class AvariceJadeCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(9)];

    protected override void OnUpgrade() => Gold.UpgradeValueTo(15M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AvariceJadePower>(
            choiceContext,
            Owner.Creature,
            Gold.BaseValue,
            Owner.Creature,
            this
        );
    }
}
