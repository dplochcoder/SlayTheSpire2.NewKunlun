using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Antique Record",
    description: "Gain {Energy:energyIcons()}, draw {Cards} cards.\nIf played more than {MaxPlays:diff()} times in one combat, permanently becomes [gold]Broken Record[/gold]."
)]
public partial class AntiqueRecordCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new EnergyVar(2), new(nameof(Cards), 2), new(nameof(MaxPlays), 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<BrokenRecordCard>()];

    protected override void OnUpgrade() => MaxPlays.UpgradeValueBy(1);

    public int PlaysThisCombat
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    protected override void AfterClonedInternal() => PlaysThisCombat = 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(Energy.IntValue, Owner);
        await CardPileCmd.Draw(choiceContext, Cards.IntValue, Owner);

        if (++PlaysThisCombat > MaxPlays.IntValue)
        {
            var deck = DeckVersion;
            if (deck != null)
                await CardCmd.TransformTo<BrokenRecordCard>(deck);
            await CardCmd.TransformTo<BrokenRecordCard>(
                this,
                deck != null ? CardPreviewStyle.None : CardPreviewStyle.HorizontalLayout
            );
        }
    }
}
