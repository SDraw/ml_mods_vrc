using UnityEngine;

namespace ml_var
{
    public class VertexAnimationRemover : MelonLoader.MelonMod
    {
        [HarmonyLib.HarmonyPatch(typeof(SkinnedMeshRenderer), nameof(SkinnedMeshRenderer.BakeMesh))]
        class Patch_SkinnedMeshRenderer_BakeMesh
        {
            static void Postfix(ref SkinnedMeshRenderer __instance)
            {
                if(__instance != null && (__instance.sharedMesh != null))
                    __instance.sharedMesh.ClearBlendShapes();
            }
        }
    }
}
