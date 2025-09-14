using PEAKLib.Core;
using PEAKLib.Items;
using PEAKLib.Stats;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Zorro.ControllerSupport.Rumble.RumbleClip;

namespace OnePeak;

public abstract class DevilFruit
{
    public static List<DevilFruit> AllFruits { get; } = new List<DevilFruit>();
    public CharacterAfflictions.STATUSTYPE Status { get; private set; }
    public abstract GameObject Prefab { get; }
    public abstract string StatusName { get; }
    public abstract Texture2D StatusTexture { get; }
    public abstract Color StatusColor { get; }

    public DevilFruit()
    {
        AllFruits.Add(this);
        InitStatus();
        InitItem();
        InitHooks();
    }

    protected virtual void InitStatus()
    {
        Status devilFruitStatus = new Status()
        {
            Name = StatusName,
            Color = StatusColor,
            MaxAmount = 0.1f,
            AllowClear = false,

            // these are ignored because we use Update
            ReductionCooldown = 1f,
            ReductionPerSecond = 0f,

            Icon = Sprite.Create(
                StatusTexture,
                new Rect(0, 0, StatusTexture.width, StatusTexture.height),
                new Vector2(0.5f, 0.5f)
            ),

            Update = (self, status) => { if (IsOwnedBy(Character.localCharacter)) OnUpdateStatus(self, status); },
        };
        new StatusContent(devilFruitStatus).Register(Plugin.Definition);
        Status = devilFruitStatus.Type;
    }

    protected virtual void InitItem()
    {
        var action = Prefab.AddComponent<Action_ApplyDevilFruitStatus>();
        action.devilFruitStatus = Status;
        action.OnCastFinished = true;
        new ItemContent(Prefab.GetComponent<Item>()).Register(Plugin.Definition);
    }

    protected virtual void InitHooks()
    {

    }

    protected abstract void OnUpdateStatus(CharacterAfflictions self, Status status);

    public bool IsOwnedBy(Character character)
    {
        return character.refs.afflictions.GetCurrentStatus(Status) > 0.0;
    }
}

public abstract class DevilFruit<T> : DevilFruit where T : DevilFruit<T>
{
    public static T Instance { get; private set; }

    public DevilFruit() : base()
    {
        Instance = (T)this;
    }
}