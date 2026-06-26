namespace MultiplayerRelics.MultiplayerRelicsCode.RestSiteOptions;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;



public class GiftOption(Player owner) : RestSiteOption(owner), ICustomModel
{
    private bool _used;

    public override bool IsEnabled => !_used && GetRemovableCardCount(Owner) >= 1;

    private static int GetRemovableCardCount(Player player)
    {
        return PileType.Deck.GetPile(player).Cards.Count(c => c.IsRemovable);
    }
    
    public override string OptionId => "MULTIPLAYERRELICS-GIFT_OPTION";

    private LocString? _description;

    public override LocString Description
    {
        get
        {
            if (!IsEnabled)
            {
                return new LocString("rest_site_ui", $"OPTION_{OptionId}.descriptionDisabled");
            }
            if (_description == null)
            {
                _description = new LocString("rest_site_ui", $"OPTION_{OptionId}.description");
                _description.Add("HasTarget", variable: false);
                _description.Add("Name", "");
            }
            return _description;
        }
    }
    
    private Player? NodeToPlayer(Node? node)
    {
	    if (node == null)
	    {
		    return null;
	    }
	    if (!(node is NMultiplayerPlayerState nMultiplayerPlayerState))
	    {
		    if (node is NRestSiteCharacter nRestSiteCharacter)
		    {
			    return nRestSiteCharacter.Player;
		    }
		    return null;
	    }
	    return nMultiplayerPlayerState.Player;
    }
    
    
    private void OnNodeHovered(Node node)
    {
	    Player player = NodeToPlayer(node);
	    if (player != null)
	    {
		    Description.Add("HasTarget", variable: true);
		    Description.Add("Name", PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, player.NetId));
		    NRestSiteRoom.Instance?.GetButtonForOption(this)?.RefreshTextState();
	    }
    }
    
    
    private void OnNodeUnhovered(Node _)
    {
	    Description.Add("HasTarget", variable: false);
	    NRestSiteRoom.Instance?.GetButtonForOption(this)?.RefreshTextState();
    }

    private bool ShouldCancelTargeting()
    {
	    if (NOverlayStack.Instance.ScreenCount <= 0)
	    {
		    return NCapstoneContainer.Instance.InUse;
	    }
	    return true;
    }
    
    private bool AllowHoveringNode(Node node)
    {
	    return !LocalContext.IsMe(NodeToPlayer(node));
    }
    
    public override async Task<bool> OnSelect()
	{
		uint choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(Owner);
		Player? target = null;
		if (LocalContext.IsMe(Owner))
		{
			NRestSiteRoom.Instance?.AnimateDescriptionDown();
			NRestSiteButton? buttonForOption = NRestSiteRoom.Instance?.GetButtonForOption(this);
			Vector2 startPosition = buttonForOption.GlobalPosition + buttonForOption.Size / 2f;
			bool usingController = NControllerManager.Instance.IsUsingController;
			NTargetManager targetManager = NTargetManager.Instance;
			targetManager.StartTargeting(TargetType.AnyPlayer, startPosition, usingController ? TargetMode.Controller : TargetMode.ClickMouseToTarget, ShouldCancelTargeting, AllowHoveringNode);
			if (usingController)
			{
				List<NRestSiteCharacter> list = NRestSiteRoom.Instance?.characterAnims.Where( c => c.Player != Owner).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					list[num].Hitbox.SetFocusMode(Control.FocusModeEnum.All);
					list[num].Hitbox.FocusNeighborTop = list[num].Hitbox.GetPath();
					list[num].Hitbox.FocusNeighborBottom = list[num].Hitbox.GetPath();
					Control hitbox = list[num].Hitbox;
					NodePath path;
					if (num <= 0)
					{
						path = list[^1].Hitbox.GetPath();
					}
					else
					{
						path = list[num - 1].Hitbox.GetPath();
					}
					hitbox.FocusNeighborLeft = path;
					list[num].Hitbox.FocusNeighborRight = (num < list.Count - 1) ? list[num + 1].Hitbox.GetPath() : list[0].Hitbox.GetPath();
				}
				list.FirstOrDefault()?.Hitbox.TryGrabFocus();
			}
			targetManager.Connect(NTargetManager.SignalName.NodeHovered, Callable.From<Node>(OnNodeHovered));
			targetManager.Connect(NTargetManager.SignalName.NodeUnhovered, Callable.From<Node>(OnNodeUnhovered));
			try
			{
				target = NodeToPlayer(await targetManager.SelectionFinished());
				RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(Owner, choiceId, PlayerChoiceResult.FromPlayerId(target?.NetId));
			}
			finally
			{
				targetManager.Disconnect(NTargetManager.SignalName.NodeHovered, Callable.From<Node>(OnNodeHovered));
				targetManager.Disconnect(NTargetManager.SignalName.NodeUnhovered, Callable.From<Node>(OnNodeUnhovered));
				if (usingController)
				{
					foreach (NRestSiteCharacter characterAnim in NRestSiteRoom.Instance.characterAnims)
					{
						characterAnim.Hitbox.SetFocusMode(Control.FocusModeEnum.None);
					}
				}
			}
		}
		else
		{
			ulong? num2 = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(Owner, choiceId)).AsPlayerId();
			if (num2.HasValue)
			{
				target = Owner.RunState.GetPlayer(num2.Value);
			}
		}
		NRestSiteRoom.Instance?.AnimateDescriptionUp();
		Description.Add("HasTarget", variable: false);
		NRestSiteRoom.Instance?.GetButtonForOption(this)?.RefreshTextState();
		if (target == null)
		{
			return false;
		}

		CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1)
		{
			Cancelable = true,
			RequireManualConfirmation = true,
		};
		CardModel? chosen = (await CardSelectCmd.FromDeckForRemoval(Owner, prefs)).FirstOrDefault();
		if (chosen == null)
		{
			return false;
		}

		CardModel gift = Owner.RunState.CloneCard(chosen);
		Owner.RunState.RemoveCard(gift);
		Owner.RunState.AddCard(gift, target);
		await CardPileCmd.Add(gift, PileType.Deck.GetPile(target));
		await CardPileCmd.RemoveFromDeck(chosen);
		_used = true;
		return true;
	}
}