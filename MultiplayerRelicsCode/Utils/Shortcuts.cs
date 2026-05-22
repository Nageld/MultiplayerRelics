using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace MultiplayerRelics.MultiplayerRelicsCode.Utils;

public class Shortcuts
{
    public static List<Creature>? GetAllies(Player Owner, bool IncludeSelf)
    {
        ICombatState? combatState = Owner.Creature.CombatState;
        if (IncludeSelf)
        {
            return combatState?.PlayerCreatures.Where(c => c.IsAlive).ToList();
        }
        return combatState?.PlayerCreatures.Where(c => c.IsAlive && c != Owner.Creature).ToList();
    }
}