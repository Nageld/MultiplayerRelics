using System.Collections;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace MultiplayerRelics.MultiplayerRelicsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class DoubleBed() : MultiplayerRelicsRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    
    public override bool TryModifyRestSiteHealRewards(Player player, List<Reward> rewards, bool isMimicked)
    {
        if (player != Owner)
        {
            return false;
        }

        foreach (var ally in player._runState.Players)
        {
            if (ally != player)
            {
                var pool = ally.Character.CardPool;
                CardCreationOptions options = new CardCreationOptions(new List<CardPoolModel> { pool },
                    //Dream Catcher uses these
                    CardCreationSource.Encounter,
                    CardRarityOddsType.RegularEncounter);
                rewards.Add(new CardReward(options, 3, Owner));
            }
        }
        
        Flash();
        return true;
    }

    
}