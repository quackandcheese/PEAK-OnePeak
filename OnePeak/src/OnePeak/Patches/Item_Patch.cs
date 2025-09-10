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
            if (GumGumFruit.IsOwnedBy(interactor))
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
                // MOVE WITH HAND
                Debug.Log("Picking up " + __instance.gameObject.name);
                ItemBackpackVisuals itemBackpackVisuals;
                if (__instance.TryGetComponent<ItemBackpackVisuals>(out itemBackpackVisuals))
                {
                    itemBackpackVisuals.RemoveVisuals();
                }




                Bodypart arm = interactor.GetBodypart(BodypartType.Arm_R);
                Bodypart elbow = interactor.GetBodypart(BodypartType.Elbow_R);
                Bodypart hand = interactor.GetBodypart(BodypartType.Hand_R);

                Vector3 itemPosition = __instance.transform.position;
                Vector3 startStretchPos = elbow.transform.position;

                //SetJointMotion(arm.joint, ConfigurableJointMotion.Free);
                SetJointMotion(elbow.joint, ConfigurableJointMotion.Free);
                //SetJointMotion(hand.joint, ConfigurableJointMotion.Free);

                //elbow.rig.isKinematic = true;
                //hand.rig.isKinematic = true;
                __instance.rig.isKinematic = true;
                // TODO: add config for enabling self-collision
                __instance.rig.excludeLayers = LayerMask.GetMask("Character");

                Tweener tween = elbow.transform.DOMove(itemPosition, 0.3f);
                tween.OnComplete(() => OnStretchOutComplete(__instance, interactor, startStretchPos));
                return false;
                //elbow.GetComponent<ConfigurableJoint>().targetPosition = elbow.transform.InverseTransformPoint(itemPosition);
            }
            return true;
            //tween.OnComplete(() => elbow.DOMove(itemPosition, 1));
            //elbow.DORestart();
            //DOTween.Play();


        }

        private static void OnStretchOutComplete(Item __instance, Character interactor, Vector3 startStretchPos)
        {
            Bodypart arm = interactor.GetBodypart(BodypartType.Arm_R);
            Bodypart elbow = interactor.GetBodypart(BodypartType.Elbow_R);
            Bodypart hand = interactor.GetBodypart(BodypartType.Hand_R);

            Vector3 returnPos = interactor.refs.items.GetItemHoldPos(__instance);

            float pullBackJumpHeight = 0.5f;
            __instance.transform.DOJump(returnPos, pullBackJumpHeight, 1, 0.25f);
            elbow.transform.DOJump(returnPos, pullBackJumpHeight, 1, 0.25f).OnComplete(() => OnPullInComplete(__instance, interactor));


        }

        private static void OnPullInComplete(Item __instance, Character interactor)
        {
            Bodypart arm = interactor.GetBodypart(BodypartType.Arm_R);
            Bodypart elbow = interactor.GetBodypart(BodypartType.Elbow_R);
            Bodypart hand = interactor.GetBodypart(BodypartType.Hand_R);

            //SetJointMotion(arm.joint, ConfigurableJointMotion.Locked);
            SetJointMotion(elbow.joint, ConfigurableJointMotion.Locked);
            //SetJointMotion(hand.joint, ConfigurableJointMotion.Locked);

            //elbow.rig.isKinematic = false;
            //hand.rig.isKinematic = false;

            //elbow.joint.connectedBody.WakeUp();
            //hand.joint.connectedBody.WakeUp();

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
