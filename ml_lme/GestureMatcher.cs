using UnityEngine;

namespace ml_lme
{
    static class GestureMatcher
    {

        readonly static Vector2[] ms_fingerLimits =
        {
            new Vector2(0f,15f),
            new Vector2(-20f,20f),
            new Vector2(-50f,50f),
            new Vector2(-7.5f,7.5f),
            new Vector2(-20f,20f)
        };

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
                            FilFingerSpreads(l_hand, ref p_data.m_leftFingersSpreads);
                        }
                        break;
                        case 1:
                        {
                            FillFingerBends(l_hand, ref p_data.m_rightFingersBends);
                            FilFingerSpreads(l_hand, ref p_data.m_rightFingersSpreads);
                        }
                        break;
                    }
                }
            }
        }

        static void FillHandPosition(Leap.Hand p_hand, ref Vector3 p_pos)
        {
            p_pos.x = p_hand.PalmPosition.x;
            p_pos.y = p_hand.PalmPosition.y;
            p_pos.z = p_hand.PalmPosition.z;
        }

        static void FillHandRotation(Leap.Hand p_hand, ref Quaternion p_rot)
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
                Quaternion l_prevSegment = Quaternion.identity;

                float l_angle = 0f;
                foreach(Leap.Bone l_bone in l_finger.bones)
                {
                    if(l_bone.Type == Leap.Bone.BoneType.TYPE_METACARPAL)
                    {
                        l_prevSegment = new Quaternion(l_bone.Rotation.x, l_bone.Rotation.y, l_bone.Rotation.z, l_bone.Rotation.w);
                        continue;
                    }

                    Quaternion l_curSegment = new Quaternion(l_bone.Rotation.x, l_bone.Rotation.y, l_bone.Rotation.z, l_bone.Rotation.w);
                    Quaternion l_diff = Quaternion.Inverse(l_prevSegment) * l_curSegment;
                    l_prevSegment = l_curSegment;

                    // Bend - local X rotation
                    float l_curAngle = 360f - l_diff.eulerAngles.x;
                    if(l_curAngle > 180f)
                        l_curAngle -= 360f;
                    l_angle += l_curAngle;
                }

                p_bends[(int)l_finger.Type] = Mathf.InverseLerp(0f, (l_finger.Type == Leap.Finger.FingerType.TYPE_THUMB) ? 90f : 180f, l_angle);
            }
        }

        static void FilFingerSpreads(Leap.Hand p_hand, ref float[] p_spreads)
        {

            foreach(Leap.Finger l_finger in p_hand.Fingers)
            {
                float l_angle = 0f;

                Leap.Bone l_parent = l_finger.Bone(Leap.Bone.BoneType.TYPE_METACARPAL);
                Leap.Bone l_child = l_finger.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL);

                Quaternion l_parentRot = new Quaternion(l_parent.Rotation.x, l_parent.Rotation.y, l_parent.Rotation.z, l_parent.Rotation.w);
                Quaternion l_childRot = new Quaternion(l_child.Rotation.x, l_child.Rotation.y, l_child.Rotation.z, l_child.Rotation.w);

                Quaternion l_diff = Quaternion.Inverse(l_parentRot) * l_childRot;

                // Spread - local Y rotation, but thumb is obnoxious
                l_angle = l_diff.eulerAngles.y;
                if(l_angle > 180f)
                    l_angle -= 360f;

                // Pain
                switch(l_finger.Type)
                {
                    case Leap.Finger.FingerType.TYPE_THUMB:
                    {
                        if(p_hand.IsRight)
                            l_angle *= -1f;
                        l_angle += ms_fingerLimits[(int)Leap.Finger.FingerType.TYPE_INDEX].y * 2f; // Magic value
                        l_angle *= 0.5f;
                    }
                    break;

                    case Leap.Finger.FingerType.TYPE_INDEX:
                    {
                        if(p_hand.IsLeft)
                            l_angle *= -1f;
                        l_angle += ms_fingerLimits[(int)Leap.Finger.FingerType.TYPE_INDEX].y;
                        l_angle *= 0.5f;
                    }
                    break;

                    case Leap.Finger.FingerType.TYPE_MIDDLE:
                    {
                        l_angle += (ms_fingerLimits[(int)Leap.Finger.FingerType.TYPE_MIDDLE].y * (p_hand.IsRight ? 0.125f : -0.125f));
                        l_angle *= (p_hand.IsLeft ? -4f : 4f);
                    }
                    break;

                    case Leap.Finger.FingerType.TYPE_RING:
                    {
                        if(p_hand.IsRight)
                            l_angle *= -1f;
                        l_angle += ms_fingerLimits[(int)Leap.Finger.FingerType.TYPE_RING].y;
                        l_angle *= 0.5f;
                    }
                    break;

                    case Leap.Finger.FingerType.TYPE_PINKY:
                    {
                        l_angle += (p_hand.IsRight ? ms_fingerLimits[(int)Leap.Finger.FingerType.TYPE_PINKY].x : ms_fingerLimits[(int)Leap.Finger.FingerType.TYPE_PINKY].y);
                        l_angle *= (p_hand.IsRight ? -0.5f : 0.5f);
                    }
                    break;

                }

                p_spreads[(int)l_finger.Type] = Mathf.InverseLerp(ms_fingerLimits[(int)l_finger.Type].x, ms_fingerLimits[(int)l_finger.Type].y, l_angle);
                if(l_finger.Type != Leap.Finger.FingerType.TYPE_THUMB)
                {
                    p_spreads[(int)l_finger.Type] *= 2f;
                    p_spreads[(int)l_finger.Type] -= 1f;
                }
            }
        }
    }
}
