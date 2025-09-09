using DG.Tweening;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zorro.Core;

namespace OnePeak.Patches
{
    [HarmonyPatch(typeof(Item))]
    public class Item_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Item.Interact))]
        static void Interact_Postfix(ref Item __instance, ref Character interactor)
        {
            if (interactor.refs.afflictions.GetCurrentStatus(Plugin.GumGumStatus) > 0.0)
            {
                Vector3 itemPosition = __instance.transform.position;
                Rigidbody elbow = interactor.GetBodypartRig(BodypartType.Elbow_R);

                Tweener tween = elbow.DOMove(itemPosition, 1);
            }
            //tween.OnComplete(() => elbow.DOMove(itemPosition, 1));
            //elbow.DORestart();
            //DOTween.Play();


        }
    }
}
