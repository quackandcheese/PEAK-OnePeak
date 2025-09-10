using PEAKLib.Stats;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OnePeak.DevilFruits;

// IDEAS

// Increased grab range for pulling teammates
// Increased grab range for grabbing onto wall
// ✔️ When you fall, instead of taking fall damage you bounce uncontrollably
// ✔️ Increased interaction range
// ✔️ Increased jump height
// ✔️ When in water, you slowly get the drowsy affliction
// ✔️ add SFX for rubber stretching! RIP straight from the anime?
public class GumGumFruit : DevilFruit<GumGumFruit>
{
    public override GameObject Prefab => Plugin.Bundle.LoadAsset<GameObject>("GumGum Fruit.prefab");
    public override string StatusName => "Gum-Gum";
    public override Texture2D StatusTexture => Plugin.Bundle.LoadAsset<Texture2D>("IC_GumGum");
    public override Color StatusColor => new Color(0.462f, 0.424f, 0.729f);
    public SFX_Instance gumGumStretchSFX => Plugin.Bundle.LoadAsset<SFX_Instance>("SFXI GumGum Stretch");
    public SFX_Instance snapBackSFX => Plugin.Bundle.LoadAsset<SFX_Instance>("SFXI GumGum Snap Back");

    protected override void OnUpdateStatus(CharacterAfflictions self, Status status)
    {
        Interaction.instance.distance = Plugin.GumGumInteractDistance;
        self.character.refs.movement.jumpGravity = 45f;
        self.character.refs.movement.jumpImpulse = 750f;
        Plugin.Instance.GrabFriendDistance = 8f;
    }

    #region PATCHES
    #endregion
}
