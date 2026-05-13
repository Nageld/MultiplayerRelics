using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace MultiplayerRelics.MultiplayerRelicsCode.Patches;

[HarmonyPatch(typeof(RelicModel), nameof(RelicModel.IsAllowed))]
public static class IsAllowedPatch
{
    static void Postfix(RelicModel __instance, IRunState runState, ref bool __result)
    {
        //TODO
        //If we change more relics for multiplayer make this iterate through a list of the classes
        if (__instance is OddlySmoothStone)
            __result = runState.Players.Count < 2;
    }
}