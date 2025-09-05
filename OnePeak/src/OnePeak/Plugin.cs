using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using PEAKLib.Core;
using PEAKLib.Stats;
using PEAKLib.Items;

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
    public static ModDefinition Definition { get; set; } = null!;
    internal static ManualLogSource Log { get; private set; } = null!;

    internal static CharacterAfflictions.STATUSTYPE GumGumStatus;

    private void Awake()
    {
        Log = Logger;
        Definition = ModDefinition.GetOrCreate(Info.Metadata);

        this.LoadBundleWithName(
            "onepeak.peakbundle",
            statusBundle =>
            {
                InitGumGumStatus(statusBundle);
                InitGumGumFruit(statusBundle);
            }
        );

        Log.LogInfo($"Plugin {Name} is loaded!");
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

            Update = (self, status) =>
            {

            },
        };
        new StatusContent(gumGumStatus).Register(Definition);
        GumGumStatus = gumGumStatus.Type;
    }
}
