
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Models.RelicPools;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MultiplayerRelics.MultiplayerRelicsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class OddlyResonatingStone : MultiplayerRelicsRelic
{

  public override RelicRarity Rarity => RelicRarity.Common;
  
  protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DexterityPower>(1)];

  public override async Task AfterRoomEntered(AbstractRoom room)
  {
    if (!(room is CombatRoom))
      return;
    Flash();
    ICombatState? combatState = Owner.Creature.CombatState;
    int owners = 0;
    
    if (combatState is not null)
    {
      foreach (Creature creature in combatState.PlayerCreatures
                 .Where(c => c.IsAlive).ToList())
      {
        if (creature.Player?.GetRelic<OddlyResonatingStone>() != null)
        {
          owners++;
        }
      }
    }
    
    MainFile.Logger.Info("Applying dexterity for " + owners + " stacks, player: " + Owner.NetId);
    await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.Dexterity.BaseValue * owners, Owner.Creature, null);
  }
}