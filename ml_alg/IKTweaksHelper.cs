using System;
using System.Reflection;
using HarmonyLib;

namespace ml_alg
{
    public static class IKTweaksHelper
    {
        static bool ms_present = false;

        static Type ms_VRIKNew = null;
        static FieldInfo ms_VRIKNew_solver = null;

        static Type ms_IKSolverVR = null;
        static FieldInfo ms_IKSolverVR_spine = null;
        static FieldInfo ms_IKSolverVR_leftLeg = null;
        static FieldInfo ms_IKSolverVR_rightLeg = null;

        static Type ms_IKSolverVR_Spine = null;
        static FieldInfo ms_IKSolverVR_Spine_IKPositionPelvis = null;
        static FieldInfo ms_IKSolverVR_Spine_IKRotationPelvis = null;
        static FieldInfo ms_IKSolverVR_Spine_pelvisTarget = null;


        static Type ms_IKSolverVR_Leg = null;
        static FieldInfo ms_IKSolverVR_Leg_IKPosition = null;
        static FieldInfo ms_IKSolverVR_Leg_IKRotation = null;
        static FieldInfo ms_IKSolverVR_Leg_target = null;

        public static bool Present
        {
            get => ms_present;
        }

        public static void ResolveTypes()
        {
            foreach(MelonLoader.MelonMod l_mod in MelonLoader.MelonHandler.Mods)
            {
                if(l_mod.Info.Name == "IKTweaks")
                {
                    ms_present = true;

                    l_mod.Assembly.GetTypes().DoIf(t => t.Name == "VRIK_New", t => ms_VRIKNew = t);
                    ms_VRIKNew?.GetFields().DoIf(f => f.Name == "solver", f => ms_VRIKNew_solver = f);

                    l_mod.Assembly.GetTypes().DoIf(t => t.Name == "IKSolverVR", t => ms_IKSolverVR = t);
                    ms_IKSolverVR?.GetFields().DoIf(f => f.Name == "spine", f => ms_IKSolverVR_spine = f);
                    ms_IKSolverVR?.GetFields().DoIf(f => f.Name == "leftLeg", f => ms_IKSolverVR_leftLeg = f);
                    ms_IKSolverVR?.GetFields().DoIf(f => f.Name == "rightLeg", f => ms_IKSolverVR_rightLeg = f);

                    l_mod.Assembly.GetTypes().DoIf(t => t.Name == "Spine", t => ms_IKSolverVR_Spine = t);
                    ms_IKSolverVR_Spine?.GetFields().DoIf(f => f.Name == "IKPositionPelvis", f => ms_IKSolverVR_Spine_IKPositionPelvis = f);
                    ms_IKSolverVR_Spine?.GetFields().DoIf(f => f.Name == "IKRotationPelvis", f => ms_IKSolverVR_Spine_IKRotationPelvis = f);
                    ms_IKSolverVR_Spine?.GetFields().DoIf(f => f.Name == "pelvisTarget", f => ms_IKSolverVR_Spine_pelvisTarget = f);

                    l_mod.Assembly.GetTypes().DoIf(t => t.Name == "Leg", t => ms_IKSolverVR_Leg = t);
                    ms_IKSolverVR_Leg?.GetFields().DoIf(f => f.Name == "IKPosition", f => ms_IKSolverVR_Leg_IKPosition = f);
                    ms_IKSolverVR_Leg?.GetFields().DoIf(f => f.Name == "IKRotation", f => ms_IKSolverVR_Leg_IKRotation = f);
                    ms_IKSolverVR_Leg?.GetFields().DoIf(f => f.Name == "target", f => ms_IKSolverVR_Leg_target = f);

                    break;
                }
            }
        }

        // Hips
        public static void SetHipsIK(UnityEngine.MonoBehaviour p_component, UnityEngine.Vector3 p_pos, UnityEngine.Quaternion p_rot, bool p_rotate = true)
        {
            object l_solver = ms_VRIKNew_solver?.GetValue(p_component.ConvertToRuntimeType(ms_VRIKNew));
            if(l_solver != null)
            {
                object l_spine = ms_IKSolverVR_spine?.GetValue(l_solver);
                if(l_spine != null)
                {
                    ms_IKSolverVR_Spine_IKPositionPelvis?.SetValue(l_spine, p_pos);
                    if(p_rotate)
                        ms_IKSolverVR_Spine_IKRotationPelvis?.SetValue(l_spine, p_rot);

                    UnityEngine.Transform l_target = (UnityEngine.Transform)ms_IKSolverVR_Spine_pelvisTarget?.GetValue(l_spine);
                    if((l_target != null) && (l_target.parent != null))
                    {
                        l_target.parent.position = p_pos;
                        if(p_rotate)
                            l_target.parent.rotation = p_rot;
                    }
                }
            }
        }

        public static UnityEngine.Transform GetHipsTarget(UnityEngine.MonoBehaviour p_component)
        {
            UnityEngine.Transform l_target = null;
            object l_solver = ms_VRIKNew_solver?.GetValue(p_component.ConvertToRuntimeType(ms_VRIKNew));
            if(l_solver != null)
            {
                object l_spine = ms_IKSolverVR_spine?.GetValue(l_solver);
                if(l_spine != null)
                    l_target = (UnityEngine.Transform)ms_IKSolverVR_Spine_pelvisTarget?.GetValue(l_spine);
            }
            return l_target;
        }

        // Left leg
        public static void SetLeftLegIK(UnityEngine.MonoBehaviour p_component, UnityEngine.Vector3 p_pos, UnityEngine.Quaternion p_rot, bool p_rotate = true)
        {
            object l_solver = ms_VRIKNew_solver?.GetValue(p_component.ConvertToRuntimeType(ms_VRIKNew));
            if(l_solver != null)
            {
                object l_leg = ms_IKSolverVR_leftLeg?.GetValue(l_solver);
                if(l_leg != null)
                {
                    ms_IKSolverVR_Leg_IKPosition?.SetValue(l_leg, p_pos);
                    if(p_rotate)
                        ms_IKSolverVR_Leg_IKRotation?.SetValue(l_leg, p_rot);

                    UnityEngine.Transform l_target = (UnityEngine.Transform)ms_IKSolverVR_Leg_target?.GetValue(l_leg);
                    if((l_target != null) && (l_target.parent != null))
                    {
                        l_target.parent.position = p_pos;
                        if(p_rotate)
                            l_target.parent.rotation = p_rot;
                    }
                }
            }
        }

        public static UnityEngine.Transform GetLeftLegTarget(UnityEngine.MonoBehaviour p_component)
        {
            UnityEngine.Transform l_target = null;
            object l_solver = ms_VRIKNew_solver?.GetValue(p_component.ConvertToRuntimeType(ms_VRIKNew));
            if(l_solver != null)
            {
                object l_leg = ms_IKSolverVR_leftLeg?.GetValue(l_solver);
                if(l_leg != null)
                    l_target = (UnityEngine.Transform)ms_IKSolverVR_Leg_target?.GetValue(l_leg);
            }
            return l_target;
        }

        // Right leg
        public static void SetRightLegIK(UnityEngine.MonoBehaviour p_component, UnityEngine.Vector3 p_pos, UnityEngine.Quaternion p_rot, bool p_rotate = true)
        {
            object l_solver = ms_VRIKNew_solver?.GetValue(p_component.ConvertToRuntimeType(ms_VRIKNew));
            if(l_solver != null)
            {
                object l_leg = ms_IKSolverVR_rightLeg?.GetValue(l_solver);
                if(l_leg != null)
                {
                    ms_IKSolverVR_Leg_IKPosition?.SetValue(l_leg, p_pos);
                    if(p_rotate)
                        ms_IKSolverVR_Leg_IKRotation?.SetValue(l_leg, p_rot);

                    UnityEngine.Transform l_target = (UnityEngine.Transform)ms_IKSolverVR_Leg_target?.GetValue(l_leg);
                    if((l_target != null) && (l_target.parent != null))
                    {
                        l_target.parent.position = p_pos;
                        if(p_rotate)
                            l_target.parent.rotation = p_rot;
                    }
                }
            }
        }

        public static UnityEngine.Transform GetRightLegTarget(UnityEngine.MonoBehaviour p_component)
        {
            UnityEngine.Transform l_target = null;
            object l_solver = ms_VRIKNew_solver?.GetValue(p_component.ConvertToRuntimeType(ms_VRIKNew));
            if(l_solver != null)
            {
                object l_leg = ms_IKSolverVR_rightLeg?.GetValue(l_solver);
                if(l_leg != null)
                    l_target = (UnityEngine.Transform)ms_IKSolverVR_Leg_target?.GetValue(l_leg);
            }
            return l_target;
        }
    }
}
