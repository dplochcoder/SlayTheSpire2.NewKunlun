using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Inebriated",
    description: "Whenever you draw a card, randomize its cost.\nWhenever you play a card, randomize the cost of a random card in your hand."
)]
public class InebriatedPower : NewKunlunPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // TODO
}
