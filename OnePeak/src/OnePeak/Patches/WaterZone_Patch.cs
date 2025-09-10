using HarmonyLib;
using OnePeak.DevilFruits;
using Peak.Afflictions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OnePeak.Patches;

[HarmonyPatch(typeof(WaterZone))]
public class WaterZone_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(WaterZone.Update))]
    static void UpdatePatch_Postfix(WaterZone __instance)
    {
        if (Character.localCharacter == null)
        {
            return;
        }
        if (__instance.characterInsideBounds && Character.observedCharacter == Character.localCharacter && GumGumFruit.IsOwnedBy(Character.localCharacter))
        {
            float drowsinessPerSecond = 0.05f;
            Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, drowsinessPerSecond * Time.deltaTime, false);
        }
    }
}
