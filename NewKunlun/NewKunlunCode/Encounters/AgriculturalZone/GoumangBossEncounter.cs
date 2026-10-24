using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Acts;
using NewKunlun.NewKunlunCode.Monsters.AgriculturalZone;

namespace NewKunlun.NewKunlunCode.Encounters.AgriculturalZone;

public class GoumangBossEncounter() : NewKunlunEncounter<AgriculturalZoneAct>(RoomType.Boss)
{
    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        [
            (ModelDb.Monster<ShinueMonster>(), nameof(ShinueMonster)),
            (ModelDb.Monster<ShiqiuMonster>(), nameof(ShiqiuMonster)),
            (ModelDb.Monster<GoumangMonster>(), nameof(GoumangMonster)),
        ];

    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
        [
            ModelDb.Monster<GoumangMonster>(),
            ModelDb.Monster<ShinueMonster>(),
            ModelDb.Monster<ShiqiuMonster>(),
        ];

    public override bool IsValidForAct(ActModel act) => act is AgriculturalZoneAct;
}
