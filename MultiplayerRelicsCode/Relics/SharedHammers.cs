using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace MultiplayerRelics.MultiplayerRelicsCode.Relics;


[Pool(typeof(SharedRelicPool))]
public class SharedHammers() : MultiplayerRelicsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    public override Task AfterRestSiteSmith(Player player)    {
        if (player != Owner)  return Task.CompletedTask;
        var players = RunManager.Instance.State?.Players;
        if (players != null)
            foreach (var character in players)
            {
                var deck = PileType.Deck.GetPile(character).Cards.Where(c => c.IsUpgradable);
                CardModel? card = Owner.RunState.Rng.Niche.NextItem(deck);
                if (card != null)
                    CardCmd.Upgrade(card);
            }
        return Task.CompletedTask;
    }
    
}