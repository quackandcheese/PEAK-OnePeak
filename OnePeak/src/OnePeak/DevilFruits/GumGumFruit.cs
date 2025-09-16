using DG.Tweening;
using MoreBadges;
using PEAKLib.Stats;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OnePeak.DevilFruits;

// IDEAS

// ☐ Increased grab range for pulling teammates
// ☐ Increased grab range for grabbing onto wall
// ➢ ☐ Change line 414 in CharacterClimbing to multiply by a config value if you have the gumgum fruit
// ➢ ☐ Postfix patch CharacterClimbing.StartClimbRpc to stretch arm to character.data.climbPos
// ☐ Add networking for stretchy arm
// ☑ When you fall, instead of taking fall damage you bounce uncontrollably
// ☑ Increased interaction range
// ☑ Increased jump height
// ☑ When in water, you slowly get the drowsy affliction
// ☑ add SFX for rubber stretching! RIP straight from the anime?

public class GumGumFruit : DevilFruit<GumGumFruit>
{
    public override GameObject Prefab => Plugin.Bundle.LoadAsset<GameObject>("GumGum Fruit.prefab");
    public override string StatusName => "Gum-Gum";
    public override Texture2D StatusTexture => Plugin.Bundle.LoadAsset<Texture2D>("IC_GumGum");
    public override Color StatusColor => new Color(0.462f, 0.424f, 0.729f);
    public SFX_Instance gumGumStretchSFX => Plugin.Bundle.LoadAsset<SFX_Instance>("SFXI GumGum Stretch");
    public SFX_Instance snapBackSFX => Plugin.Bundle.LoadAsset<SFX_Instance>("SFXI GumGum Snap Back");
    public static bool pullingToClimb = false;

    protected override void OnUpdateStatus(CharacterAfflictions self, Status status)
    {
        Interaction.instance.distance = Plugin.GumGumInteractDistance;
        self.character.refs.movement.jumpGravity = 45f;
        self.character.refs.movement.jumpImpulse = 750f;
        Plugin.Instance.GrabFriendDistance = 8f;
        Plugin.Instance.GrabWallDistanceMultiplier = 5f;
    }

    protected override void InitHooks()
    {
        On.CharacterClimbing.StartClimbRpc += CharacterClimbing_StartClimbRpc;
        On.CharacterClimbing.StopClimbingRpc += CharacterClimbing_StopClimbingRpc;
    }

    private void CharacterClimbing_StopClimbingRpc(On.CharacterClimbing.orig_StopClimbingRpc orig, CharacterClimbing self, float setFall)
    {
        if (IsOwnedBy(self.character) && pullingToClimb)
            return;
        orig(self, setFall);
    }

    private void CharacterClimbing_StartClimbRpc(On.CharacterClimbing.orig_StartClimbRpc orig, CharacterClimbing self, Vector3 climbPos, Vector3 climbNormal)
    {
        Vector3 cameraPos = MainCamera.instance.transform.position;
        float defaultClimbDistance = 2.5f;
        if (IsOwnedBy(self.character) && Vector3.Distance(cameraPos, climbPos) > defaultClimbDistance)
        {
            if (!pullingToClimb)
            {
                pullingToClimb = true;
                Plugin.Log.LogInfo("StartClimbRpc called");

                // Calculate a vector perpendicular to the climbNormal and up direction to get a "sideways" offset
                Vector3 up = Vector3.up;
                Vector3 side = Vector3.Cross(climbNormal, up).normalized * 0.15f;
                Vector3 cameraToClimb = (MainCamera.instance.transform.position - climbPos).normalized * 0.2f; // Move slightly toward camera
                Vector3 rightPos = climbPos + side + cameraToClimb;
                Vector3 leftPos = climbPos - side + cameraToClimb;

                StretchLimbTo(self.character, rightPos, BodypartType.Elbow_R, false);
                StretchLimbTo(self.character, leftPos, BodypartType.Elbow_L, false, null, () =>
                {
                    self.character.data.sinceCanClimb = 0f;
                    self.sinceLastClimbStarted = 0f;
                    Plugin.Log.LogInfo("Stretch complete, calling original StartClimbRpc");
                    pullingToClimb = false;
                    orig(self, climbPos, climbNormal);
                });
            }

            return;
        }

        orig(self, climbPos, climbNormal);
    }

    #region STRETCH ARM
    public static void StretchLimbTo(Character interactor, Vector3 targetPosition, BodypartType limbType, bool tweenPullBack = true, Action<Vector3, float, float> onExtendComplete = null!, Action onPullBackComplete = null!)
    {
        if (!Instance.IsOwnedBy(interactor)) return;

        Bodypart limb = interactor.GetBodypart(limbType);

        // set joint motion to free so it can be moved around without pulling the body
        SetJointMotion(limb.joint, ConfigurableJointMotion.Free);
        limb.rig.isKinematic = true;

        float distance = Vector3.Distance(targetPosition, limb.transform.position);
        float distanceDivisor = 12f; // arbitrary number to make the stretch speed reasonable
        float extendDuration = distance / distanceDivisor;

        // stretch hand to target position
        var limbTweener = limb.transform.DOMove(targetPosition, extendDuration);
        limbTweener.OnComplete(() =>
        {
            OnStretchOutComplete(interactor, limbType, extendDuration, tweenPullBack, onExtendComplete, onPullBackComplete);
        });

        // play stretch sound effect
        var sfxSettings = new SFX_Settings();
        sfxSettings.pitch = Instance.gumGumStretchSFX.clips[0].length / extendDuration;
        sfxSettings.volume = Instance.gumGumStretchSFX.settings.volume;
        sfxSettings.pitch_Variation = 0f;
        SFX_Player.instance.PlaySFX(Instance.gumGumStretchSFX, interactor.Center, null, sfxSettings, 1f, false);
    }

    private static void OnStretchOutComplete(Character interactor, BodypartType limbType, float extendDuration, bool tweenPullBack = true, Action<Vector3, float, float> onExtendComplete = null!, Action onPullBackComplete = null!)
    {
        Bodypart limb = interactor.GetBodypart(limbType);

        Vector3 returnPos = interactor.refs.items.GetItemHoldPos(null);

        float pullBackJumpHeight = 0.3f;
        float pullBackDuration = 0.5f;

        onExtendComplete?.Invoke(returnPos, pullBackJumpHeight, pullBackDuration);

        if (tweenPullBack)
        {
            limb.transform.DOJump(returnPos, pullBackJumpHeight, 1, pullBackDuration).OnComplete(() =>
            {
                OnPullInComplete(interactor, limbType, onPullBackComplete);
            });
        }
        else
        {
            OnPullInComplete(interactor, limbType, onPullBackComplete);
        }

        SFX_Player.instance.PlaySFX(GumGumFruit.Instance.snapBackSFX, interactor.Center, null, null, 1f, false);
    }

    private static void OnPullInComplete(Character interactor, BodypartType limbType, Action onPullBackComplete = null!)
    {

        Bodypart limb = interactor.GetBodypart(limbType);

        limb.rig.isKinematic = false;
        SetJointMotion(limb.joint, ConfigurableJointMotion.Locked);

        onPullBackComplete?.Invoke();
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
