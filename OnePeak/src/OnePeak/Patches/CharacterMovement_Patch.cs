using HarmonyLib;
using OnePeak.DevilFruits;
using UnityEngine;

namespace OnePeak.Patches
{
    [HarmonyPatch(typeof(CharacterMovement))]
    internal class CharacterMovement_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CharacterMovement.Land))]
        static bool Land_Prefix(ref CharacterMovement __instance, ref CharacterMovement.PlayerGroundSample bestSample)
        {

            if (GumGumFruit.IsOwnedBy(__instance.character))
            {
                if (__instance.character.data.sinceGrounded > 0.5f)
                {
                    Bounce(ref __instance, ref bestSample);

                    if (__instance.character.IsLocal)
                    {
                        GUIManager.instance.ReticleLand();
                    }
                    __instance.character.OnLand(__instance.character.data.sinceGrounded);
                }

                return false;
            }

            return true;
        }

        private static void Bounce(ref CharacterMovement __instance, ref CharacterMovement.PlayerGroundSample bestSample)
        {
            if (__instance.FallTime() > __instance.fallDamageTime)
            {
                float fallDamageAmount = Mathf.Max(__instance.FallFactor(3f, 1.5f), 0.05f);
                fallDamageAmount = Mathf.Min(fallDamageAmount, __instance.MaxVelDmg());
                //fallDamageAmount *= Ascents.fallDamageMultiplier;
                if (fallDamageAmount >= 0.025f)
                {
                    Debug.Log("Fall damage: " + (fallDamageAmount * 100f).ToString("F0") + "%");
                    __instance.character.data.sinceGrounded = 0f;
                    __instance.character.data.sinceJump = 0f;
                    __instance.character.data.isJumping = true;
                    if (fallDamageAmount > 0.3f && __instance.character.IsLocal)
                    {
                        __instance.character.Fall(fallDamageAmount * 5f, 0f);
                        __instance.character.AddForce(bestSample.normal * 5000 * fallDamageAmount);
                    }
                    else
                    {
                        __instance.character.AddForce(bestSample.normal * 30000 * fallDamageAmount);
                    }
                }
            }
        }
    }
}

