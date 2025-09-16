using HarmonyLib;
using MoreBadges;
using OnePeak.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnePeak.Patches;

[HarmonyPatch(typeof(EndScreen), "EndSequenceRoutine")]
public static class EndSequenceRoutine_Patch
{
    static void Prefix(EndScreen __instance)
    {
        foreach (DevilFruit fruit in DevilFruit.AllFruits)
        {
            if (Character.localCharacter.HasEatenDevilFruit(fruit) && fruit.BadgeName != "")
                MoreBadgesPlugin.AddProgress(fruit.BadgeName, 1);
        }
    }
}