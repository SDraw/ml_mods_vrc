namespace ml_lme
{
    static class GestureMatcher
    {
        public class GesturesData
        {
            readonly public static int ms_handsCount = 2;
            readonly public static int ms_fingersCount = 5;

            public bool[] m_handsPresenses = null;
            public UnityEngine.Vector3[] m_handsPositons = null;
            public UnityEngine.Quaternion[] m_handsRotations = null;
            public float[] m_leftFingersBends = null;
            public float[] m_leftFingersSpreads = null;
            public float[] m_rightFingersBends = null;
            public float[] m_rightFingersSpreads = null;

            public GesturesData()
            {
                m_handsPresenses = new bool[ms_handsCount];
                m_handsPositons = new UnityEngine.Vector3[ms_handsCount];
                m_handsRotations = new UnityEngine.Quaternion[ms_handsCount];
                m_leftFingersBends = new float[ms_fingersCount];
                m_leftFingersSpreads = new float[ms_fingersCount];
                m_rightFingersBends = new float[ms_fingersCount];
                m_rightFingersSpreads = new float[ms_fingersCount];
            }
        }

        public static void GetGestures(Leap.Frame p_frame, ref GesturesData p_data)
        {
            // Fill as default
            for(int i = 0; i < GesturesData.ms_handsCount; i++)
                p_data.m_handsPresenses[i] = false;
            for(int i = 0; i < GesturesData.ms_fingersCount; i++)
            {
                p_data.m_leftFingersBends[i] = 0f;
                p_data.m_leftFingersSpreads[i] = 0f;
                p_data.m_rightFingersBends[i] = 0f;
                p_data.m_leftFingersSpreads[i] = 0f;
            }

            // Fill hands data
            foreach(Leap.Hand l_hand in p_frame.Hands)
            {
                int l_sideID = (l_hand.IsLeft ? 0 : 1);
                if(!p_data.m_handsPresenses[l_sideID])
                {
                    p_data.m_handsPresenses[l_sideID] = true;
                    FillHandPosition(l_hand, ref p_data.m_handsPositons[l_sideID]);
                    FillHandRotation(l_hand, ref p_data.m_handsRotations[l_sideID]);
                    switch(l_sideID)
                    {
                        case 0:
                        {
                            FillFingerBends(l_hand, ref p_data.m_leftFingersBends);
                            FilFingerSpreads(l_hand, ref p_data.m_leftFingersBends, ref p_data.m_leftFingersSpreads);
                        }
                        break;
                        case 1:
                        {
                            FillFingerBends(l_hand, ref p_data.m_rightFingersBends);
                            FilFingerSpreads(l_hand, ref p_data.m_rightFingersBends, ref p_data.m_rightFingersSpreads);
                        }
                        break;
                    }
                }
            }
        }

        static void FillHandPosition(Leap.Hand p_hand, ref UnityEngine.Vector3 p_pos)
        {
            p_pos.x = p_hand.PalmPosition.x;
            p_pos.y = p_hand.PalmPosition.y;
            p_pos.z = p_hand.PalmPosition.z;
        }

        static void FillHandRotation(Leap.Hand p_hand, ref UnityEngine.Quaternion p_rot)
        {
            p_rot.x = p_hand.Rotation.x;
            p_rot.y = p_hand.Rotation.y;
            p_rot.z = p_hand.Rotation.z;
            p_rot.w = p_hand.Rotation.w;
        }

        static void FillFingerBends(Leap.Hand p_hand, ref float[] p_bends)
        {
            foreach(Leap.Finger l_finger in p_hand.Fingers)
            {
                int l_startBoneID = ((l_finger.Type == Leap.Finger.FingerType.TYPE_THUMB) ? 1 : 0);
                UnityEngine.Vector3 l_prevDirection = new UnityEngine.Vector3(0f, 0f, 0f);

                for(int i = l_startBoneID; i < 4; i++)
                {
                    Leap.Bone l_bone = l_finger.Bone((Leap.Bone.BoneType)i);
                    if(l_bone != null)
                    {
                        Leap.Vector l_leapDir = l_bone.NextJoint - l_bone.PrevJoint;
                        UnityEngine.Vector3 l_dir = new UnityEngine.Vector3(l_leapDir.x, l_leapDir.y, l_leapDir.z);
                        l_dir.Normalize();
                        if(i > l_startBoneID)
                            p_bends[(int)l_finger.Type] += UnityEngine.Mathf.Acos(UnityEngine.Vector3.Dot(l_dir, l_prevDirection));
                        l_prevDirection = l_dir;
                    }
                }

                p_bends[(int)l_finger.Type] = UnityEngine.Mathf.InverseLerp((l_finger.Type == Leap.Finger.FingerType.TYPE_THUMB) ? 0f : (float)System.Math.PI * 0.125f, (l_finger.Type == Leap.Finger.FingerType.TYPE_THUMB) ? (float)System.Math.PI * 0.25f : (float)System.Math.PI, p_bends[(int)l_finger.Type]);
            }
        }

        static void FilFingerSpreads(Leap.Hand p_hand, ref float[] p_bends, ref float[] p_spreads)
        {
            UnityEngine.Quaternion l_palmRotation = new UnityEngine.Quaternion(p_hand.Rotation.x, p_hand.Rotation.y, p_hand.Rotation.z, p_hand.Rotation.w);
            UnityEngine.Vector3 l_sideDir = l_palmRotation * (p_hand.IsLeft ? UnityEngine.Vector3.right : UnityEngine.Vector3.left);
            UnityEngine.Vector3 l_handPos = new UnityEngine.Vector3(p_hand.PalmPosition.x, p_hand.PalmPosition.y, p_hand.PalmPosition.z);

            foreach(Leap.Finger l_finger in p_hand.Fingers)
            {
                Leap.Bone l_bone = l_finger.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL);
                if(l_bone != null)
                {
                    Leap.Vector l_leapDir = l_bone.NextJoint - l_bone.PrevJoint;
                    UnityEngine.Vector3 l_dir = new UnityEngine.Vector3(l_leapDir.x, l_leapDir.y, l_leapDir.z);
                    l_dir.Normalize();

                    if(l_finger.Type != Leap.Finger.FingerType.TYPE_THUMB)
                        p_spreads[(int)l_finger.Type] = UnityEngine.Vector3.Dot(l_dir, l_sideDir) * 2f;
                    else
                    {
                        UnityEngine.Vector3 l_boneNext = new UnityEngine.Vector3(l_bone.NextJoint.x, l_bone.NextJoint.y, l_bone.NextJoint.z);
                        p_spreads[(int)l_finger.Type] = 1f - UnityEngine.Mathf.InverseLerp(40f, 70f, UnityEngine.Vector3.Distance(l_boneNext, l_handPos));
                    }

                    switch(l_finger.Type)
                    {
                        case Leap.Finger.FingerType.TYPE_INDEX:
                            p_spreads[(int)l_finger.Type] += 0.25f;
                            break;
                        case Leap.Finger.FingerType.TYPE_MIDDLE:
                            p_spreads[(int)l_finger.Type] = p_spreads[(int)l_finger.Type] * 2.5f + 0.5f;
                            break;
                        case Leap.Finger.FingerType.TYPE_RING:
                            p_spreads[(int)l_finger.Type] = -p_spreads[(int)l_finger.Type] * 3f - 0.75f;
                            break;
                        case Leap.Finger.FingerType.TYPE_PINKY:
                            p_spreads[(int)l_finger.Type] = -p_spreads[(int)l_finger.Type] - 0.15f;
                            break;
                    }

                    if(p_bends[(int)l_finger.Type] > 0.5f)
                        p_spreads[(int)l_finger.Type] = UnityEngine.Mathf.Lerp(p_spreads[(int)l_finger.Type], 0f, (p_bends[(int)l_finger.Type] - 0.5f) * 2f);
                }
            }
        }
    }
}
