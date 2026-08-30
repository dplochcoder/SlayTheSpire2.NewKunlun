using BaseLib.Abstracts;
using Godot;
using NewKunlun.NewKunlunCode.Extensions;

namespace NewKunlun.NewKunlunCode.Character;

public class YiRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Yi.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
