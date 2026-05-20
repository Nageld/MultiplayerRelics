using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace MultiplayerRelics.MultiplayerRelicsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class Bonds() : MultiplayerRelicsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;


    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber != 1)
            return;
        var allies = combatState.PlayerCreatures.Where(c => c.IsAlive).ToList();
        if (allies.Count == 0)
            return;

        var multiplayerCards = Owner.UnlockState.CardPools
            .SelectMany(c => c.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint))
            .Where(c => c.MultiplayerConstraint == CardMultiplayerConstraint.MultiplayerOnly)
            .ToList();

        Flash();
        foreach (var ally in allies)
        {
            if (ally.Player == null) continue;
            
            CardModel? cardModel = CardFactory.GetDistinctForCombat(
                ally.Player,
                multiplayerCards,
                1,
                Owner.RunState.Rng.CombatCardGeneration
            ).FirstOrDefault();
            
            if (cardModel == null) continue; 
            
            var added = await CardPileCmd.AddGeneratedCardToCombat(
                cardModel, PileType.Draw, ally.Player, CardPilePosition.Random);
            
            if (LocalContext.IsMe(ally))
            {
                CardCmd.PreviewCardPileAdd(added);
            }
        }
    }
}