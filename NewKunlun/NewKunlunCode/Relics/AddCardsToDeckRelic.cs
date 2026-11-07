using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Extensions;

namespace NewKunlun.NewKunlunCode.Relics;

public abstract class AddCardsToDeckRelic<T> : NewKunlunRelic
    where T : CardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<T>()];

    protected abstract int NumCards { get; }

    protected virtual bool Upgrade => false;

    public override bool HasUponPickupEffect => true;

    public override Task AfterObtained() =>
        Owner.AddGeneratedCardsToPile<T>(NumCards, PileType.Deck, Upgrade);
}
