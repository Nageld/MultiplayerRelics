using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MultiplayerRelics.MultiplayerRelicsCode.Extensions;

namespace MultiplayerRelics.MultiplayerRelicsCode.Enchantments;

public class MultiplayerEnchantment : CustomEnchantmentModel
{
    protected override string? CustomIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentImagePath();
            return ResourceLoader.Exists(path) ? path : null;
        }
    }
}