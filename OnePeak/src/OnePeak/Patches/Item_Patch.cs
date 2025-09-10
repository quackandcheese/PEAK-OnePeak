using DG.Tweening;
using HarmonyLib;
using OnePeak.DevilFruits;
using Photon.Pun;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.XR;

namespace OnePeak.Patches
{
    [HarmonyPatch(typeof(Item))]
    public class Item_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Item.Interact))]
        static bool Interact_Prefix(Item __instance, Character interactor)
        {
            float distance = Vector3.Distance(__instance.transform.position, interactor.Center);
            if (distance > 2f && GumGumFruit.IsOwnedBy(interactor))
            {
                if (!interactor.player.HasEmptySlot(__instance.itemID))
                {
                    return false;
                }
                if (interactor.refs.items.lastEquippedSlotTime + 0.25f > Time.time)
                {
                    return false;
                }
                if (interactor.data.isClimbing && !__instance.UIData.canPocket)
                {
                    __instance.SetKinematicNetworked(false);
                    return false;
                }
                GlobalEvents.TriggerItemRequested(__instance, interactor);
                //__instance.gameObject.SetActive(false);
                Debug.Log("Picking up " + __instance.gameObject.name);
                ItemBackpackVisuals itemBackpackVisuals;
                if (__instance.TryGetComponent<ItemBackpackVisuals>(out itemBackpackVisuals))
                {
                    itemBackpackVisuals.RemoveVisuals();
                }

                Bodypart elbow = interactor.GetBodypart(BodypartType.Elbow_R);

                Vector3 itemPosition = __instance.transform.position;
                Vector3 startStretchPos = elbow.transform.position;

                SetJointMotion(elbow.joint, ConfigurableJointMotion.Free);

                __instance.rig.isKinematic = true;
                // TODO: add config for enabling self-collision
                __instance.rig.excludeLayers = LayerMask.GetMask("Character");

                float distanceDivisor = 24f;
                float extendDuration = distance / distanceDivisor;
                Tweener tween = elbow.transform.DOMove(itemPosition, extendDuration);
                tween.OnComplete(() => OnStretchOutComplete(__instance, interactor, startStretchPos, extendDuration));
                return false;
            }
            return true;
        }

        private static void OnStretchOutComplete(Item __instance, Character interactor, Vector3 startStretchPos, float extendDuration)
        {
            Bodypart elbow = interactor.GetBodypart(BodypartType.Elbow_R);

            Vector3 returnPos = interactor.refs.items.GetItemHoldPos(__instance);

            float pullBackJumpHeight = 0.3f;
            float pullDuration = extendDuration * 0.75f;
            __instance.transform.DOJump(returnPos, pullBackJumpHeight, 1, pullDuration);
            elbow.transform.DOJump(returnPos, pullBackJumpHeight, 1, pullDuration).OnComplete(() => OnPullInComplete(__instance, interactor));
        }

        private static void OnPullInComplete(Item __instance, Character interactor)
        {
            Bodypart elbow = interactor.GetBodypart(BodypartType.Elbow_R);

            SetJointMotion(elbow.joint, ConfigurableJointMotion.Locked);

            __instance.gameObject.SetActive(false);
            __instance.view.RPC("RequestPickup", RpcTarget.MasterClient, new object[] { interactor.GetComponent<PhotonView>() });
        }

        private static void SetJointMotion(ConfigurableJoint joint, ConfigurableJointMotion motion)
        {
            joint.xMotion = motion;
            joint.yMotion = motion;
            joint.zMotion = motion;
        }
    }
}
