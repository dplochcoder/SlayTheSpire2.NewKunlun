using BaseLib.Abstracts;
using Godot;
using SlayTheSpire2.NewKunlun.SlayTheSpire2.NewKunlunCode.Extensions;

namespace SlayTheSpire2.NewKunlun.SlayTheSpire2.NewKunlunCode.Character;

public class YiRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Yi.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
