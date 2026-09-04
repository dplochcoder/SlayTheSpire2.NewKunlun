using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Parry",
    description: "Gain {Block:diff()} [gold]Block[/gold]. Gain {Parry:diff()} [gold]Parry[/gold]. Take {Imperfect:diff()} [gold]Imperfect[/gold]."
)]
public partial class ParryCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self),
        ITranscendenceCard
{
    public override bool GainsBlock => true;

    public override bool IsParryCard => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(9M, ValueProp.Move),
            new DynamicVar(nameof(Parry), 1M),
            new DynamicVar(nameof(Imperfect), 3M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Parry(), Tip.Imperfect()];

    protected override void OnUpgrade() => Block.UpgradeValueTo(13M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await ParryCmd.GainParry(
            choiceContext,
            Owner.Creature,
            Parry.BaseValue,
            Owner.Creature,
            cardPlay.Card
        );
        await PowerCmd.Apply<ImperfectPower>(
            choiceContext,
            Owner.Creature,
            Imperfect.BaseValue,
            Owner.Creature,
            cardPlay.Card
        );
    }

    public CardModel GetTranscendenceTransformedCard() => ModelDb.Get<PerfectParryCard>();
}
