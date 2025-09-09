/*using MonoDetour;
using pworld.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using MonoDetour.HookGen;
using On.Interaction;
using MonoDetour.Cil;
using MonoMod.Cil;
using UnityEngine;
using Mono.Cecil.Cil;
using HarmonyLib;
using OnePeak;

namespace OnePeak.Hooks;


[MonoDetourTargets(typeof(Interaction))]
static class InteractionHook
{

    [MonoDetourHookInitialize]
    static void Init()
    {

        DoInteractableRaycasts.ILHook(ILHook_DoInteractableRaycasts);
    }

    static void ILHook_DoInteractableRaycasts(ILManipulationInfo info)
    {
        ILWeaver weaver = new(info);

        weaver
            .MatchMultipleRelaxed(
                onMatch: matchWeaver =>
                {
                    matchWeaver.Current.Operand = Plugin.GumGumInteractDistance;
                },
                x => x.MatchLdloc(0),
                x => x.MatchLdcR4(10f) && weaver.SetCurrentTo(x),
                x => x.MatchBgtUn(out var _)
            )
            .ThrowIfFailure();
    }
}
*/