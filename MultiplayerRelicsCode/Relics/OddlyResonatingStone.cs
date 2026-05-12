
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using BaseLib.Utils;
using System.Threading;

namespace MultiplayerRelics.MultiplayerRelicsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class OddlyResonatingStone : MultiplayerRelicsRelic
{


  public override List<(string, string)>? Localization => new RelicLoc(
    "Oddly Resonating Stone",
    "At the start of each combat, foo.",
    "The stone resonates with the energy of the world around it, creating a soothing hum that can be heard by those who are attuned to it."
  );

  public override RelicRarity Rarity => RelicRarity.Common;

  private static volatile int ownedCount = 0;

  public override Task AfterObtained() => Task.Run(() =>
  {
    MainFile.Logger.Info("Owned count: " + ownedCount + " player who obtained it: " + this.Owner.NetId);
    Interlocked.Increment(ref ownedCount);
  });

  public override Task AfterRemoved() => Task.Run(() =>
  {
    MainFile.Logger.Info("Owned count: " + ownedCount + " player who removed it: " + this.Owner.NetId);
    Interlocked.Decrement(ref ownedCount);
  });

  public override bool ShowCounter => true;
  public override int DisplayAmount => ownedCount;

  public override async Task AfterRoomEntered(AbstractRoom room)
  {
    OddlyResonatingStone oddlyResonatingStone = this;
    if (!(room is CombatRoom))
      return;
    oddlyResonatingStone.Flash();
    MainFile.Logger.Info("Applying dexterity for " + ownedCount + " stacks, player: " + oddlyResonatingStone.Owner.NetId);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    DexterityPower dexterityPower = await PowerCmd.Apply<DexterityPower>((PlayerChoiceContext)new ThrowingPlayerChoiceContext(), oddlyResonatingStone.Owner.Creature, oddlyResonatingStone.DynamicVars.Dexterity.BaseValue * ownedCount, oddlyResonatingStone.Owner.Creature, (CardModel)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
  }
}