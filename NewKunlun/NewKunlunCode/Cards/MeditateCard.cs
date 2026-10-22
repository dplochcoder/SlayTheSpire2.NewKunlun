using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(title: "Meditate", description: "Gain {Energy:energyIcons()}.\nDraw 1 card.")]
public partial class MeditateCard()
    : NewKunlunCard(
        0,
        CardType.Skill,
        CardRarity.Ancient,
        MegaCrit.Sts2.Core.Entities.Cards.TargetType.None
    )
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(Energy.IntValue, Owner);
        await CardPileCmd.Draw(choiceContext, Owner);
    }
}
