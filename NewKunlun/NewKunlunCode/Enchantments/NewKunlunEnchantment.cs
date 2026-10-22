using BaseLib.Abstracts;
using BaseLib.Extensions;
using NewKunlun.NewKunlunCode.Extensions;

namespace NewKunlun.NewKunlunCode.Enchantments;

/// <summary>
/// Base class for New Kunlun enchantments, with icons loaded from the mod's resources.
/// </summary>
public abstract class NewKunlunEnchantment : CustomEnchantmentModel
{
    protected override string CustomIconPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentImagePath();
}
