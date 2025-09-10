using MonoDetour;
using pworld.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using MonoDetour.HookGen;
using On.CharacterGrabbing;
using MonoDetour.Cil;
using MonoMod.Cil;
using UnityEngine;
using Mono.Cecil.Cil;
using HarmonyLib;

namespace OnePeak.Hooks;

[MonoDetourTargets(typeof(CharacterGrabbing))]
static class GameHandlerHooks
{

    [MonoDetourHookInitialize]
    static void Init()
    {
        Reach.ILHook(ILHook_Reach);
    }

    static void ILHook_Reach(ILManipulationInfo info)
    {
        ILWeaver weaver = new(info);

        // Change the maximum grab distance to the value set in the config

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
                        nameof(Plugin.GrabFriendDistance))));
                },
                x => x.MatchLdloc(2),
                x => x.MatchLdcR4(4f) && weaver.SetCurrentTo(x),
                x => x.MatchBgt(out var _)
            )
            .ThrowIfFailure();

        Plugin.Log.LogInfo(info.ToString());
    }
}