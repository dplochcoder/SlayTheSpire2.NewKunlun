using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace NewKunlun.NewKunlunCode.Monsters;

public abstract class NewKunlunMonster : CustomMonsterModel
{
    public override int MinInitialHp => MaxInitialHp;
}
