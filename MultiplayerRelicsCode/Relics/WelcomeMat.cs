using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MultiplayerRelics.MultiplayerRelicsCode.Utils;

namespace MultiplayerRelics.MultiplayerRelicsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class WelcomeMat() : MultiplayerRelicsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    private bool _usedThisCombat;
    
    private bool UsedThisCombat
    {
        get => _usedThisCombat;
        set
        {
            AssertMutable();
            _usedThisCombat = value;
        }
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
        {
            return Task.CompletedTask;
        }
        UsedThisCombat = false;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }
    
    public override Task AfterCombatEnd(CombatRoom _)
    {
        UsedThisCombat = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var card = cardPlay.Card;

        if (UsedThisCombat) return;
        if (card.IsDupe) return;
        if (card.Owner != Owner) return;

        UsedThisCombat = true;
        Flash();
        Status = RelicStatus.Normal;

        var allies = Shortcuts.GetAllies(Owner, false);
        var target = cardPlay.Target;
        foreach (var ally in allies)
        {
            var combatCopy = card.CreateCloneForPlayer(ally.Player);

            combatCopy.EnergyCost.SetThisCombat(0);

            await CardPileCmd.AddGeneratedCardToCombat(combatCopy, PileType.Hand, ally.Player, CardPilePosition.Random);

        }
    }
}