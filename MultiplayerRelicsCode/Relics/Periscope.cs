using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;

namespace MultiplayerRelics.MultiplayerRelicsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class Periscope() : MultiplayerRelicsRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        var players = RunManager.Instance.DebugOnlyGetState()?.Players;
        if (players == null) return;

        var cards = players.SelectMany(p => p.Deck.Cards).ToList();
        if (cards.Count == 0) return;

        var chosen = await CardSelectCmd.FromSimpleGrid(
            new BlockingPlayerChoiceContext(), cards, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1));

        var picked = chosen.FirstOrDefault();
        if (picked == null) return;

        CardModel card = Owner.RunState.CloneCard(picked);
        Owner.RunState.RemoveCard(card);
        Owner.RunState.AddCard(card, Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
    }
}