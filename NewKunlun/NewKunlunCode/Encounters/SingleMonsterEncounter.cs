using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace NewKunlun.NewKunlunCode.Encounters;

public abstract class SingleMonsterEncounter<TAct, TMonster>(RoomType roomType)
    : NewKunlunEncounter<TAct>(roomType, autoAdd: true)
    where TAct : ActModel
    where TMonster : MonsterModel
{
    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        [(ModelDb.Monster<TMonster>().ToMutable(), null)];

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<TMonster>()];
}
