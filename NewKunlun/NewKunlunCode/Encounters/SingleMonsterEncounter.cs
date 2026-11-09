using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace NewKunlun.NewKunlunCode.Encounters;

public class SingleMonsterEncounter<TAct, TMonster>(RoomType roomType, bool autoAdd = true)
    : NewKunlunEncounter<TAct>(roomType, autoAdd)
    where TAct : ActModel
    where TMonster : MonsterModel
{
    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        [(ModelDb.Monster<TMonster>().ToMutable(), null)];

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<TMonster>()];
}
