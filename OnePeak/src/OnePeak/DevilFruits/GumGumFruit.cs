using PEAKLib.Stats;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OnePeak.DevilFruits;

public class GumGumFruit : DevilFruit<GumGumFruit>
{
    public override GameObject Prefab => Plugin.Bundle.LoadAsset<GameObject>("GumGum Fruit.prefab");
    public override string StatusName => "Gum-Gum";
    public override Texture2D StatusTexture => Plugin.Bundle.LoadAsset<Texture2D>("IC_GumGum");
    public override Color StatusColor => new Color(0.462f, 0.424f, 0.729f);

    protected override void OnUpdateStatus(CharacterAfflictions self, Status status)
    {
        Interaction.instance.distance = Plugin.GumGumInteractDistance;
        self.character.refs.movement.jumpGravity = 45f;
        self.character.refs.movement.jumpImpulse = 750f;
    }

    #region PATCHES
    #endregion
}
