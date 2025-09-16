using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using PEAKLib.Core;
using PEAKLib.Stats;
using PEAKLib.Items;
using MonoDetour;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System;
using Zorro.Core.CLI;
using Zorro.Core;
using OnePeak.DevilFruits;
using MoreBadges;

namespace OnePeak;

[BepInAutoPlugin]
[BepInDependency("com.github.PEAKModding.PEAKLib.Core", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.github.PEAKModding.PEAKLib.Items", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.github.PEAKModding.PEAKLib.Stats", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("MoreCustomizations", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.snosz.morebadges", BepInDependency.DependencyFlags.HardDependency)]
public partial class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; } = null!;
    public static ModDefinition Definition { get; set; } = null!;
    internal static ManualLogSource Log { get; private set; } = null!;
    internal static PeakBundle Bundle { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    public float GrabFriendDistance { get; set; } = 4f;
    public float GrabWallDistanceMultiplier { get; set; } = 1.25f;

    // Config
    internal static float GumGumInteractDistance { get; set; }

    private void Awake()
    {
        Instance = this;
        Log = Logger;
        Definition = ModDefinition.GetOrCreate(Info.Metadata);

        InitConfig();
        MonoDetourManager.InvokeHookInitializers(typeof(Plugin).Assembly);
        Patch();

        this.LoadBundleWithName(
            "onepeak.peakbundle",
            bundle =>
            {
                Bundle = bundle;
                InitDevilFruits();
                InitBadges();
            }
        );


        Log.LogInfo($"Plugin {Name} is loaded!");
    }

    private void InitBadges()
    {
        // This adds a badge that will trigger only if the conditions are met during a single expedition.
        MoreBadgesPlugin.CustomBadge strawHatBadge = new MoreBadgesPlugin.CustomBadge(
            name: BadgeNames.StrawHatBadge,
            displayName: "STRAW HAT BADGE",
            description: "Reach the peak while having eaten the Gum-Gum Fruit.",
            icon: Plugin.Bundle.LoadAsset<Texture2D>("IC_StrawHatBadge"), //256x256 Texture2D
            progressRequired: 1, // How many times the condition needs to be met to unlock the badge. Defaults to 1
            runBasedProgress: true // This determines if the badge progress is reset every expedition. Defaults to false,
                                   //nameLocalizations: nameLocalizations // If you want to support more than just english, you can pass a list containing translated strings in the appropriate order
                                   //descriptionLocalizations: descriptionLocalizations 
            );
        MoreBadgesPlugin.RegisterBadge(strawHatBadge, "-252561889_Hat_OnePeak_StrawHat");
    }

    private void InitDevilFruits()
    {
        new GumGumFruit();
    }

    private void InitConfig()
    {
        GumGumInteractDistance = Config.Bind("General", "Gum-Gum Interact Distance", 8f, "How far away you can interact with items from when you've eaten the Gum-Gum Fruit.").Value;
    }
    internal static void Patch()
    {
        Harmony ??= new Harmony(Plugin.Instance.Info.Metadata.GUID);

        Log.LogDebug("Patching...");

        Harmony.PatchAll();

        Log.LogDebug("Finished patching!");
    }
}
