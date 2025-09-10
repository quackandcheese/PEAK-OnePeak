using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using PEAKLib.Core;
using PEAKLib.Stats;
using PEAKLib.Items;
using MonoDetour;
using HarmonyLib;

namespace OnePeak;

// IDEAS

// When you fall, instead of taking fall damage you bounce uncontrollably
// Increased interaction range
// Increased grab range for pulling teammates
// Increased jump height
// When in water, you slowly get the drowsy affliction

[BepInAutoPlugin]
[BepInDependency("com.github.PEAKModding.PEAKLib.Core", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.github.PEAKModding.PEAKLib.Items", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.github.PEAKModding.PEAKLib.Stats", BepInDependency.DependencyFlags.HardDependency)]
public partial class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; } = null!;
    public static ModDefinition Definition { get; set; } = null!;
    internal static ManualLogSource Log { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    internal static CharacterAfflictions.STATUSTYPE GumGumStatus;

    // Config
    internal static float GumGumInteractDistance { get; set; }

    private void Awake()
    {
        Instance = this;
        Log = Logger;
        Definition = ModDefinition.GetOrCreate(Info.Metadata);

        InitConfig();

        this.LoadBundleWithName(
            "onepeak.peakbundle",
            statusBundle =>
            {
                InitGumGumStatus(statusBundle);
                InitGumGumFruit(statusBundle);
            }
        );

        Patch();

        Log.LogInfo($"Plugin {Name} is loaded!");
    }

    private void InitConfig()
    {
        GumGumInteractDistance = Config.Bind("General", "Gum-Gum Interact Distance", 25f, "How far away you can interact with items from when you've eaten the Gum-Gum Fruit.").Value;
    }
    internal static void Patch()
    {
        Harmony ??= new Harmony(Plugin.Instance.Info.Metadata.GUID);

        Log.LogDebug("Patching...");

        Harmony.PatchAll();

        Log.LogDebug("Finished patching!");
    }

    private void InitGumGumFruit(PeakBundle bundle)
    {
        var gumGumFruitPrefab = bundle.LoadAsset<GameObject>("GumGum Fruit.prefab");
        // attach behavior
        var action = gumGumFruitPrefab.AddComponent<Action_GumGum>();
        action.OnCastFinished = true;

        //bundle.Mod.RegisterContent();
        new ItemContent(gumGumFruitPrefab.GetComponent<Item>()).Register(Definition);
    }

    private void InitGumGumStatus(PeakBundle bundle)
    {
        var gumGumTex = bundle.LoadAsset<Texture2D>("IC_GumGum");
        Status gumGumStatus = new Status()
        {
            Name = "Gum-Gum",
            Color = new Color(0.462f, 0.424f, 0.729f),
            MaxAmount = 0.1f,
            AllowClear = false,

            // these are ignored because we use Update
            ReductionCooldown = 1f,
            ReductionPerSecond = 0f,

            Icon = Sprite.Create(
                gumGumTex,
                new Rect(0, 0, gumGumTex.width, gumGumTex.height),
                new Vector2(0.5f, 0.5f)
            ),

            Update = OnUpdateGumGumStatus,
        };
        new StatusContent(gumGumStatus).Register(Definition);
        GumGumStatus = gumGumStatus.Type;
    }

    private void OnUpdateGumGumStatus(CharacterAfflictions self, Status status)
    {
        if (self.GetCurrentStatus(GumGumStatus) > 0.0)
        {
            Interaction.instance.distance = 6f;
            self.character.refs.movement.jumpGravity = 45f;
            self.character.refs.movement.jumpImpulse = 750f;
        }
    }
}
