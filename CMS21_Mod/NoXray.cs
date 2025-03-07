using System;
using UnityEngine;
using MelonLoader;
using HarmonyLib;
using CMS.Managers;
using System.Reflection;

namespace CMS21_Mod
{
    public class NoXray : MelonMod
    {
        private static bool state = true;
        private static CarLoader[] carLoaders;

        public void CheckForUpdates()
        {

        }

        public override void OnApplicationStart()
        {
            
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            carLoaders = UnityEngine.Object.FindObjectsOfType<CarLoader>();
            if (carLoaders == null)
            {
                MelonLogger.Warning("No CarLoader instances found in the current workspace.");
                return;
            }
            else
            {
                MelonLogger.Msg($"(i) CarLoader instances found in the current scence: {sceneName}");
            }
        }

        public static void ApplyShowHideBodyGlobally(bool show, float XRayAlpha)
        {
            foreach (var loader in carLoaders)
            {
                var method = typeof(CarLoader).GetMethod("ShowHideBody", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(bool), typeof(float) }, null);

                if (method != null)
                {
                    method.Invoke(loader, new object[] { show, XRayAlpha });
                    MelonLogger.Msg($"Applied Patch on {loader.name} with toggle: {show}, XRayAlpha: {XRayAlpha}");
                }
                else
                {
                    MelonLogger.Warning("ShowHideBody() with (bool, float) not found on CarLoader.");
                }
            }
        }


        [HarmonyPatch(typeof(CarLoader))]
        [HarmonyPatch("ShowHideBody", new Type[] { typeof(bool), typeof(float) })]
        public class Patch
        {
            public static bool Prefix(ref bool show, ref float XRayAlpha)
            {
                show = state;
                MelonLogger.Msg("Patched ShowHideBody, force to: " + show);
                return true;
            }
        }

        [HarmonyPatch(typeof(CarLoader))]
        [HarmonyPatch("ShowCarConditions")]
        public class Patch2
        {
            public static bool Prefix()
            {
                state = false;
                ApplyShowHideBodyGlobally(state, 0.5f);
                MelonLogger.Msg("Patched Examination XRay to forced.");
                return true;
            }
        }

        [HarmonyPatch(typeof(CarLoader))]
        [HarmonyPatch("HideCarConditions")]
        public class Patch3
        {
            public static bool Prefix()
            {
                state = true;
                MelonLogger.Msg("Reverting patch to hide examination mode.");
                return true;
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                state = !state;
                ApplyShowHideBodyGlobally(state, 0.5f);
                MelonLogger.Msg("Changed XRay state: " + state);
            }
        }
    }
}
