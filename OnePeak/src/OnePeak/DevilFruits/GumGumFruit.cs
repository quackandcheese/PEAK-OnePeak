using DG.Tweening;
using PEAKLib.Stats;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OnePeak.DevilFruits;

// IDEAS

// Increased grab range for pulling teammates
// Increased grab range for grabbing onto wall
// Add networking for stretchy arm
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
    public static Tweener armTweener = null!;

    protected override void OnUpdateStatus(CharacterAfflictions self, Status status)
    {
        Interaction.instance.distance = Plugin.GumGumInteractDistance;
        self.character.refs.movement.jumpGravity = 45f;
        self.character.refs.movement.jumpImpulse = 750f;
        Plugin.Instance.GrabFriendDistance = 8f;
    }

    #region STRETCH ARM
    public static void StretchArmTo(Character interactor, Vector3 targetPosition, Action<Vector3, float, float> onExtendComplete = null!, Action onPullBackComplete = null!)
    {
        if (!IsOwnedBy(interactor) || (armTweener != null && armTweener.active)) return;

        Bodypart elbow = interactor.GetBodypart(BodypartType.Elbow_R);

        // set joint motion to free so it can be moved around without pulling the body
        SetJointMotion(elbow.joint, ConfigurableJointMotion.Free);

        float distance = Vector3.Distance(targetPosition, elbow.transform.position);
        float distanceDivisor = 12f; // arbitrary number to make the stretch speed reasonable
        float extendDuration = distance / distanceDivisor;

        // stretch hand to target position
        armTweener = elbow.transform.DOMove(targetPosition, extendDuration);
        armTweener.OnComplete(() =>
        {
            OnStretchOutComplete(interactor, extendDuration, onExtendComplete, onPullBackComplete);
        });

        // play stretch sound effect
        var sfxSettings = new SFX_Settings();
        sfxSettings.pitch = Instance.gumGumStretchSFX.clips[0].length / extendDuration;
        sfxSettings.volume = Instance.gumGumStretchSFX.settings.volume;
        sfxSettings.pitch_Variation = 0f;
        SFX_Player.instance.PlaySFX(Instance.gumGumStretchSFX, interactor.Center, null, sfxSettings, 1f, false);
    }

    private static void OnStretchOutComplete(Character interactor, float extendDuration, Action<Vector3, float, float> onExtendComplete = null!, Action onPullBackComplete = null!)
    {
        Bodypart elbow = interactor.GetBodypart(BodypartType.Elbow_R);

        Vector3 returnPos = interactor.refs.items.GetItemHoldPos(null);

        float pullBackJumpHeight = 0.3f;
        float pullBackDuration = 0.5f;

        onExtendComplete?.Invoke(returnPos, pullBackJumpHeight, pullBackDuration);

        elbow.transform.DOJump(returnPos, pullBackJumpHeight, 1, pullBackDuration).OnComplete(() =>
        {
            OnPullInComplete(interactor, onPullBackComplete);
        });

        SFX_Player.instance.PlaySFX(GumGumFruit.Instance.snapBackSFX, interactor.Center, null, null, 1f, false);
    }

    private static void OnPullInComplete(Character interactor, Action onPullBackComplete = null!)
    {
        onPullBackComplete?.Invoke();

        Bodypart elbow = interactor.GetBodypart(BodypartType.Elbow_R);

        SetJointMotion(elbow.joint, ConfigurableJointMotion.Locked);
    }

    private static void SetJointMotion(ConfigurableJoint joint, ConfigurableJointMotion motion)
    {
        joint.xMotion = motion;
        joint.yMotion = motion;
        joint.zMotion = motion;
    }
    #endregion

    #region PATCHES
    #endregion
}
