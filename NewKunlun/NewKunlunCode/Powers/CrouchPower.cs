using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Crouch",
    description: "Next turn, pull {TalismanDash:cardName()} into your hand."
)]
public partial class CrouchPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new TalismanDashVar<CrouchPower>(card =>
                TalismanDashCard.IsUpgradedAnywhere(card.Owner.Player)
            ),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.TalismanDashCard(Owner.Player)];

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (!participants.Contains(Owner) || Owner.Player == null)
            return;

        var card = Owner.Player.FindCard<TalismanDashCard>([
            PileType.Hand,
            PileType.Deck,
            PileType.Discard,
        ]);
        if (card != null)
            await CardPileCmd.Add(card, PileType.Hand.GetPile(Owner.Player), CardPilePosition.Top);
        else
            TalkCmd.Play(Yi.BanterNoTalismanDash(), Owner, VfxColor.Red, VfxDuration.Standard);

        await PowerCmd.Remove(this);
    }
}
