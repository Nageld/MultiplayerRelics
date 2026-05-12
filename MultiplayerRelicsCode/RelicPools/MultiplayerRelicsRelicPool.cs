using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace MultiplayerRelics.MultiplayerRelicsCode.RelicPools;

public class MultiplayerRelicsRelicPool : CustomRelicPoolModel
{
    public override bool IsShared => true;
    private IEnumerable<RelicModel>? _memRelics;

    public override Task BeforeRoomEntered(AbstractRoom room)
    {
        if (_memRelics is null)
        {
            _memRelics = _relics;
            if (_relics is null)
            {
                return Task.FromException(new NullReferenceException("Multiplayer relics not found"));
            }

            bool isMult = false;
            foreach (RelicModel relic in _relics)
            {
                if (relic.Owner.RunState.CardMultiplayerConstraint == CardMultiplayerConstraint.MultiplayerOnly)
                {
                    isMult = true;
                    break;
                }
            }
            
            if (!isMult)
            {
                MainFile.Logger.Info("Not in multiplayer, removing relics");
                _relics = (IEnumerable<RelicModel>) Array.Empty<RelicModel>();
            }
            else
            {
                MainFile.Logger.Info("In multiplayer, keeping relics");
            }
        }
        return Task.CompletedTask;
    }
}
