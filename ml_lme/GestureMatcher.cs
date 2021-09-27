namespace ml_lme
{
    static class GestureMatcher
    {
        public class GesturesData
        {
            readonly public static int gc_handCount = 2;
            readonly public static int gc_fingersCount = 5;

            public bool[] m_handsPresenses = null;
            public UnityEngine.Vector3[] m_handsPositons = null;
            public UnityEngine.Quaternion[] m_handsRotations = null;
            public float[] m_leftFingersBends = null;
            public float[] m_leftFingersSpreads = null;
            public float[] m_rightFingersBends = null;
            public float[] m_rightFingersSpreads = null;

            public GesturesData()
            {
                m_handsPresenses = new bool[gc_handCount];
                m_handsPositons = new UnityEngine.Vector3[gc_handCount];
                m_handsRotations = new UnityEngine.Quaternion[gc_handCount];
                m_leftFingersBends = new float[gc_fingersCount];
                m_leftFingersSpreads = new float[gc_fingersCount];
                m_rightFingersBends = new float[gc_fingersCount];
                m_rightFingersSpreads = new float[gc_fingersCount];
            }
        }

        readonly static UnityEngine.Vector3 gc_axisX = new UnityEngine.Vector3(1f, 0f, 0f);
        readonly static UnityEngine.Vector3 gc_axisXN = new UnityEngine.Vector3(-1f, 0f, 0f);

        public static void GetGestures(Leap.Frame f_frame, ref GesturesData f_data)
        {
            // Fill as default
            for(int i = 0; i < GesturesData.gc_handCount; i++)
                f_data.m_handsPresenses[i] = false;
            for(int i = 0; i < GesturesData.gc_fingersCount; i++)
            {
                f_data.m_leftFingersBends[i] = 0f;
                f_data.m_leftFingersSpreads[i] = 0f;
                f_data.m_rightFingersBends[i] = 0f;
                f_data.m_leftFingersSpreads[i] = 0f;
            }

            // Fill hands data
            foreach(var l_hand in f_frame.Hands)
            {
                int l_sideID = (l_hand.IsLeft ? 0 : 1);
                if(!f_data.m_handsPresenses[l_sideID])
                {
                    f_data.m_handsPresenses[l_sideID] = true;
                    FillHandPosition(l_hand, ref f_data.m_handsPositons[l_sideID]);
                    FillHandRotation(l_hand, ref f_data.m_handsRotations[l_sideID]);
                    switch(l_sideID)
                    {
                        case 0:
                        {
                            FillHandBends(l_hand, ref f_data.m_leftFingersBends);
                            FillHandSpreads(l_hand, ref f_data.m_leftFingersBends, ref f_data.m_leftFingersSpreads);
                        }
                        break;
                        case 1:
                        {
                            FillHandBends(l_hand, ref f_data.m_rightFingersBends);
                            FillHandSpreads(l_hand, ref f_data.m_rightFingersBends, ref f_data.m_rightFingersSpreads);
                        }
                        break;
                    }
                }
            }
        }

        static void FillHandPosition(Leap.Hand f_hand, ref UnityEngine.Vector3 f_pos)
        {
            f_pos.x = f_hand.PalmPosition.x;
            f_pos.y = f_hand.PalmPosition.y;
            f_pos.z = f_hand.PalmPosition.z;
        }

        static void FillHandRotation(Leap.Hand f_hand, ref UnityEngine.Quaternion f_rot)
        {
            f_rot.x = f_hand.Rotation.x;
            f_rot.y = f_hand.Rotation.y;
            f_rot.z = f_hand.Rotation.z;
            f_rot.w = f_hand.Rotation.w;
        }

        static void FillHandBends(Leap.Hand f_hand, ref float[] f_bends)
        {
            foreach(var l_finger in f_hand.Fingers)
            {
                int l_startBoneID = ((l_finger.Type == Leap.Finger.FingerType.TYPE_THUMB) ? 1 : 0);
                UnityEngine.Vector3 l_prevDirection = new UnityEngine.Vector3(0f, 0f, 0f);

                for(int i = l_startBoneID; i < 4; i++)
                {
                    var l_bone = l_finger.Bone((Leap.Bone.BoneType)i);
                    if(l_bone != null)
                    {
                        var l_leapDir = l_bone.NextJoint - l_bone.PrevJoint;
                        var l_dir = new UnityEngine.Vector3(l_leapDir.x, l_leapDir.y, l_leapDir.z);
                        l_dir.Normalize();
                        if(i > l_startBoneID)
                            f_bends[(int)l_finger.Type] += UnityEngine.Mathf.Acos(UnityEngine.Vector3.Dot(l_dir, l_prevDirection));
                        l_prevDirection = l_dir;
                    }
                }

                f_bends[(int)l_finger.Type] = NormalizeRange(f_bends[(int)l_finger.Type], (l_finger.Type == Leap.Finger.FingerType.TYPE_THUMB) ? 0f : (float)System.Math.PI * 0.125f, (l_finger.Type == Leap.Finger.FingerType.TYPE_THUMB) ? (float)System.Math.PI * 0.25f : (float)System.Math.PI);
            }
        }

        static void FillHandSpreads(Leap.Hand f_hand, ref float[] f_bends, ref float[] f_spreads)
        {
            UnityEngine.Quaternion l_palmRotation = new UnityEngine.Quaternion(f_hand.Rotation.x, f_hand.Rotation.y, f_hand.Rotation.z, f_hand.Rotation.w);
            UnityEngine.Vector3 l_sideDir = l_palmRotation * (f_hand.IsLeft ? gc_axisX : gc_axisXN);
            UnityEngine.Vector3 l_handPos = new UnityEngine.Vector3(f_hand.PalmPosition.x, f_hand.PalmPosition.y, f_hand.PalmPosition.z);

            foreach(var l_finger in f_hand.Fingers)
            {
                var l_bone = l_finger.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL);
                if(l_bone != null)
                {
                    var l_leapDir = l_bone.NextJoint - l_bone.PrevJoint;
                    var l_dir = new UnityEngine.Vector3(l_leapDir.x, l_leapDir.y, l_leapDir.z);
                    l_dir.Normalize();

                    if(l_finger.Type != Leap.Finger.FingerType.TYPE_THUMB)
                        f_spreads[(int)l_finger.Type] = UnityEngine.Vector3.Dot(l_dir, l_sideDir) * 2f;
                    else
                    {
                        UnityEngine.Vector3 l_boneNext = new UnityEngine.Vector3(l_bone.NextJoint.x, l_bone.NextJoint.y, l_bone.NextJoint.z);
                        f_spreads[(int)l_finger.Type] = 1f - NormalizeRange(UnityEngine.Vector3.Distance(l_boneNext, l_handPos), 40f, 70f);
                    }

                    switch(l_finger.Type)
                    {
                        case Leap.Finger.FingerType.TYPE_INDEX:
                            f_spreads[(int)l_finger.Type] += 0.25f;
                            break;
                        case Leap.Finger.FingerType.TYPE_MIDDLE:
                            f_spreads[(int)l_finger.Type] = f_spreads[(int)l_finger.Type] * 2.5f + 0.5f;
                            break;
                        case Leap.Finger.FingerType.TYPE_RING:
                            f_spreads[(int)l_finger.Type] = -f_spreads[(int)l_finger.Type] * 3f - 0.75f;
                            break;
                        case Leap.Finger.FingerType.TYPE_PINKY:
                            f_spreads[(int)l_finger.Type] = -f_spreads[(int)l_finger.Type] - 0.15f;
                            break;
                    }

                    if(f_bends[(int)l_finger.Type] > 0.5f)
                        f_spreads[(int)l_finger.Type] = UnityEngine.Mathf.Lerp(f_spreads[(int)l_finger.Type], 0f, (f_bends[(int)l_finger.Type] - 0.5f) * 2f);
                }
            }
        }

        public static float NormalizeRange(float f_val, float f_min, float f_max)
        {
            float l_mapped = (f_val - f_min) / (f_max - f_min);
            return UnityEngine.Mathf.Clamp(l_mapped, 0f, 1f);
        }
    }
}
