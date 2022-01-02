using System.Reflection;

namespace ml_alg
{
    public static class IKTweaksHelper
    {
        static bool ms_present = false;
        static MethodInfo ms_preSetupVrIk = null;

        public static bool Present
        {
            get => ms_present;
        }

        public static MethodInfo PreSetupVRIK
        {
            get => ms_preSetupVrIk;
        }

        public static void Resolve()
        {
            foreach(MelonLoader.MelonMod l_mod in MelonLoader.MelonHandler.Mods)
            {
                if(l_mod.Info.Name == "IKTweaks")
                {
                    ResovleMethods();
                    ms_present = true;
                    break;
                }
            }
        }

        // Those CSharp references are weird
        static void ResovleMethods()
        {
            ms_preSetupVrIk = typeof(IKTweaks.FullBodyHandling).GetMethod("PreSetupVrIk", BindingFlags.Static | BindingFlags.NonPublic);
        }

        // Hips
        public static void SetHipsIK(UnityEngine.MonoBehaviour p_component, UnityEngine.Vector3 p_pos, UnityEngine.Quaternion p_rot)
        {
            RootMotionNew.FinalIK.VRIK_New l_component = p_component.TryCast<RootMotionNew.FinalIK.VRIK_New>();
            if(l_component != null)
            {
                var l_spine = l_component.solver?.spine;
                if(l_spine != null)
                {
                    l_spine.IKPositionPelvis = p_pos;
                    l_spine.IKRotationPelvis = p_rot;

                    UnityEngine.Transform l_target = l_spine.pelvisTarget;
                    if((l_target != null) && (l_target.parent != null))
                    {
                        l_target.parent.position = p_pos;
                        l_target.parent.rotation = p_rot;
                    }
                }
            }
        }

        public static UnityEngine.Transform GetHipsTarget(UnityEngine.MonoBehaviour p_component)
        {
            UnityEngine.Transform l_target = null;
            RootMotionNew.FinalIK.VRIK_New l_component = p_component.TryCast<RootMotionNew.FinalIK.VRIK_New>();
            if((l_component != null) && (l_component.solver?.spine != null))
                l_target = l_component.solver.spine.pelvisTarget;
            return l_target;
        }

        // Legs
        public static void SetLegIK(UnityEngine.MonoBehaviour p_component, UnityEngine.HumanBodyBones p_legFoot, UnityEngine.Vector3 p_pos, UnityEngine.Quaternion p_rot)
        {
            RootMotionNew.FinalIK.VRIK_New l_component = p_component.TryCast<RootMotionNew.FinalIK.VRIK_New>();
            if(l_component != null)
            {
                var l_leg = ((p_legFoot == UnityEngine.HumanBodyBones.LeftFoot) ? l_component.solver?.leftLeg : l_component.solver?.rightLeg);
                if(l_leg != null)
                {
                    l_leg.IKPosition = p_pos;
                    l_leg.IKRotation = p_rot;

                    UnityEngine.Transform l_target = l_leg.target;
                    if((l_target != null) && (l_target.parent != null))
                    {
                        l_target.parent.position = p_pos;
                        l_target.parent.rotation = p_rot;
                    }
                }
            }
        }

        public static UnityEngine.Transform GetLegTarget(UnityEngine.MonoBehaviour p_component, UnityEngine.HumanBodyBones p_legFoot)
        {
            UnityEngine.Transform l_target = null;
            RootMotionNew.FinalIK.VRIK_New l_component = p_component.TryCast<RootMotionNew.FinalIK.VRIK_New>();
            if(l_component != null)
                l_target = ((p_legFoot == UnityEngine.HumanBodyBones.LeftFoot) ? l_component.solver?.leftLeg?.target : l_component.solver?.rightLeg?.target);
            return l_target;
        }
    }
}
