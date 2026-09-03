using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace NewKunlun.NewKunlunCode.Cards;

public interface IAzureBowArrow
{
    Task OnPlayArrow(PlayerChoiceContext context, CardPlay cardPlay);
}
