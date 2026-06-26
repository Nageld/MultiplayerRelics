using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MultiplayerRelics.MultiplayerRelicsCode.RestSiteOptions;


namespace MultiplayerRelics.MultiplayerRelicsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class GoodieBag : MultiplayerRelicsRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    private ICollection<RestSiteOption>? _options;

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner)
            return false;
        _options = options;
        options.Add(new GiftOption(player));
        return true;
    }

    
}