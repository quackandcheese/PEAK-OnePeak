/*using MonoDetour;
using pworld.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using MonoDetour.HookGen;
using On.CharacterMovement;
using MonoDetour.Cil;
using MonoMod.Cil;
using UnityEngine;
using Mono.Cecil.Cil;
using HarmonyLib;
using OnePeak;
using Zorro.Core;
using UnityEngine.TextCore.Text;

namespace OnePeak.Hooks;

[MonoDetourTargets(typeof(CharacterMovement))]
static class CharacterMovementHook
{

    [MonoDetourHookInitialize]
    static void Init()
    {

        CheckFallDamage.Prefix(Prefix_CheckFallDamage);
    }

    static void Prefix_CheckFallDamage(CharacterMovement self)
    {
        if (self.character.refs.afflictions.GetCurrentStatus(Plugin.GumGumStatus) > 0.0)
        {

            if (self.FallTime() > self.fallDamageTime)
            {
                float fallDamageAmount = Mathf.Max(self.FallFactor(3f, 1.5f), 0.05f);
                fallDamageAmount = Mathf.Min(fallDamageAmount, self.MaxVelDmg());
                //fallDamageAmount *= Ascents.fallDamageMultiplier;
                if (fallDamageAmount >= 0.025f)
                {
                    Debug.Log("Fall damage: " + (fallDamageAmount * 100f).ToString("F0") + "%");
                    if (fallDamageAmount > 0.3f && self.character.IsLocal)
                    {
                        self.character.Fall(fallDamageAmount * 5f, 0f);
                        self.character.AddForce((self.character.Center - self.character.data.groundPos).normalized * 5000 * fallDamageAmount);
                    }
                }
            }

            return false;
        }

        return true;
    }
}*/