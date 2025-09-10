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

                // turn off item physics
                __instance.rig.isKinematic = true;
                // TODO: add config for enabling self-collision
                __instance.rig.excludeLayers = LayerMask.GetMask("Character");

                // Stretch arm out!
                Vector3 itemPosition = __instance.transform.position;
                GumGumFruit.StretchArmTo(interactor, itemPosition, (returnPos, pullBackJumpHeight, pullBackDuration) =>
                {
                    __instance.transform.DOJump(returnPos, pullBackJumpHeight, 1, pullBackDuration);
                }, 
                () =>
                {
                    __instance.gameObject.SetActive(false);
                    __instance.view.RPC("RequestPickup", RpcTarget.MasterClient, new object[] { interactor.GetComponent<PhotonView>() });
                });

                return false;
            }
            return true;
        }
    }
}
