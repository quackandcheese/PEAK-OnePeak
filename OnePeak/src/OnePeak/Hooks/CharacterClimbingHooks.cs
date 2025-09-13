using MonoDetour;
using pworld.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using MonoDetour.HookGen;
using MonoDetour.Cil;
using MonoMod.Cil;
using UnityEngine;
using Mono.Cecil.Cil;
using HarmonyLib;
using OnePeak.DevilFruits;
using OnDetour.CharacterClimbing;

namespace OnePeak.Hooks;

[MonoDetourTargets(typeof(CharacterClimbing))]
static class CharacterClimbingHooksHooks
{

    [MonoDetourHookInitialize]
    static void Init()
    {
        TryToStartWallClimb.ILHook(ILHook_TryToStartWallClimb);
    }

    static void ILHook_TryToStartWallClimb(ILManipulationInfo info)
    {
        ILWeaver weaver = new(info);

        // Change the maximum grab distance based on property GrabFriendDistance in Plugin

        weaver
            .MatchMultipleRelaxed(
                onMatch: matchWeaver =>
                {
                    matchWeaver.InsertBefore(matchWeaver.Current,
                        matchWeaver.Create(
                            OpCodes.Call,
                            AccessTools.DeclaredPropertyGetter(typeof(Plugin),
                            nameof(Plugin.Instance))));

                    matchWeaver.ReplaceCurrent(matchWeaver.Create(
                        OpCodes.Callvirt,
                        AccessTools.DeclaredPropertyGetter(typeof(Plugin),
                        nameof(Plugin.GrabWallDistanceMultiplier))));
                },
                x => x.MatchLdloc(0),
                x => x.MatchLdloc(1),
                x => x.MatchLdcR4(1.25f) && weaver.SetCurrentTo(x)
            )
            .ThrowIfFailure();

        Plugin.Log.LogInfo(info.ToString());
    }
}