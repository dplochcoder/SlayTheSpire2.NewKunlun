using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using NewKunlun.NewKunlunCode.Extensions;

namespace NewKunlun.NewKunlunCode.Monsters.AgriculturalZone;

public class ShiqiuMonster : NewKunlunMonster
{
    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        throw new NotImplementedException();
    }

    public override int MaxInitialHp => AscensionLevel.ToughEnemies.Get(163, 145);
}
