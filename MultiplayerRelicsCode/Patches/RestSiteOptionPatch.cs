using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace MultiplayerRelics.MultiplayerRelicsCode.Patches;

[HarmonyPatch(typeof(RestSiteOption))]
[HarmonyPatch("IconPath", MethodType.Getter)]

public static class RestSiteOptionPatch
{
    static void Postfix(RestSiteOption __instance, ref string __result)
    {
        if (__instance.OptionId == "MULTIPLAYERRELICS-GIFT_OPTION")
            __result = "res://MultiplayerRelics/images/rest_site/gift.png";

    }
}