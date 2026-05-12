using BaseLib.Abstracts;
using BaseLib.Extensions;
using MultiplayerRelics.MultiplayerRelicsCode.Extensions;
using MegaCrit.Sts2.Core.Runs;

namespace MultiplayerRelics.MultiplayerRelicsCode.Relics;

public abstract class MultiplayerRelicsRelic : CustomRelicModel
{
    public override bool IsAllowed(IRunState runState)
    {
        return runState.Players.Count > 1;
    }
    //MultiplayerRelics/images/relics
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}