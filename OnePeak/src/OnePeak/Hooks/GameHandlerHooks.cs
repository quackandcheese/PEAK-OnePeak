// WIP
// turns out the Reach method is run on every client for every player, and the client BEING grabbed drags itself towards the grabber

/*using MonoDetour;
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
using OnePeak.DevilFruits;

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
                        nameof(Plugin.GrabFriendDistance))));
                },
                x => x.MatchLdloc(2),
                x => x.MatchLdcR4(4f) && weaver.SetCurrentTo(x),
                x => x.MatchBgt(out var _)
            )
            .ThrowIfFailure();

        // After the player successfully grabs another player, call GumGumFruit.StretchLimbTo to stretch the arm towards the grabbed player
        // TODO: add a check if character.data.isReaching is false so it only runs once
        weaver
            .MatchMultipleRelaxed(
                onMatch: matchWeaver =>
                {
                    matchWeaver.InsertAfterCurrent(
                        matchWeaver.Create(OpCodes.Ldarg_0), // player that is grabbing
                        matchWeaver.Create(OpCodes.Ldfld, AccessTools.DeclaredField(typeof(CharacterGrabbing), nameof(CharacterGrabbing.character))),
                        matchWeaver.Create(OpCodes.Ldloc_1), // player that is being grabbed
                        matchWeaver.Create(OpCodes.Callvirt, AccessTools.DeclaredPropertyGetter(typeof(Character), nameof(Character.Center))), // player that is being grabbed
                        matchWeaver.Create(OpCodes.Ldnull),
                        matchWeaver.Create(OpCodes.Ldnull),
                        matchWeaver.Create(OpCodes.Call, AccessTools.DeclaredMethod(typeof(GumGumFruit), nameof(GumGumFruit.StretchLimbTo)))
                    );
                },
                x => x.MatchCallvirt(AccessTools.DeclaredMethod(typeof(Character), nameof(Character.LimitFalling))),
                x => x.MatchLdsfld(AccessTools.DeclaredField(typeof(GUIManager), nameof(GUIManager.instance))),
                x => x.MatchCallvirt(AccessTools.DeclaredMethod(typeof(GUIManager), nameof(GUIManager.Grasp))) && weaver.SetCurrentTo(x)
            )
            .ThrowIfFailure();

        Plugin.Log.LogInfo(info.ToString());
    }
}*/