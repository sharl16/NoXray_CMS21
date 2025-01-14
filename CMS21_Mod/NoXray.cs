using System;
using UnityEngine;
using MelonLoader;
using HarmonyLib;

namespace CMS21_Mod
{
    public class NoXray : MelonMod
    {
        private static bool state = true;

        [HarmonyPatch(typeof(CarLoader))]
        [HarmonyPatch("ShowHideBody", new Type[] { typeof(bool), typeof(float)})]
        public class Patch
        {
            public static bool Prefix(ref bool show, ref float XRayAlpha)
            {
                show = state; //Hide XRay?
                MelonLogger.Msg("Hide Xray? " + show);
                return true;
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                state = !state;
                MelonLogger.Msg("Changed XRay state: " + state);
            }
        }
    }
}
