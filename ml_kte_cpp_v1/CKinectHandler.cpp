#include "stdafx.h"

#include "CKinectHandler.h"
#include "CJointFilter.h"

CKinectHandler::CKinectHandler()
{
    m_kinectSensor = nullptr;

    for(auto &l_filter : m_jointFilters) l_filter = new CJointFilter();

    m_frameData = new FrameData();
    m_frameData->m_frameTime = 0;
    m_frameData->m_tick = 0U;

    m_paused = false;
}

CKinectHandler::~CKinectHandler()
{
    Cleanup();

    delete m_frameData;
    for(auto &l_filter : m_jointFilters) delete l_filter;
}

bool CKinectHandler::Initialize()
{
    if(!m_kinectSensor)
    {
        int l_count = 0;
        NuiGetSensorCount(&l_count);

        for(int i = 0; i < l_count; i++)
        {
            if(NuiCreateSensorByIndex(i, &m_kinectSensor) >= S_OK)
            {
                if(m_kinectSensor->NuiStatus() == S_OK) break;
                else
                {
                    m_kinectSensor->Release();
                    m_kinectSensor = nullptr;
                }
            }
            else m_kinectSensor = nullptr;
        }

        if(m_kinectSensor)
        {
            if(m_kinectSensor->NuiInitialize(NUI_INITIALIZE_FLAG_USES_SKELETON) >= S_OK) m_paused = false;
            else
            {
                m_kinectSensor->Release();
                m_kinectSensor = nullptr;
            }
        }
    }

    if(!m_kinectSensor) Cleanup();
    return (m_kinectSensor != nullptr);
}
void CKinectHandler::Terminate()
{
    Cleanup();
}

void CKinectHandler::Cleanup()
{
    if(m_kinectSensor)
    {
        m_kinectSensor->NuiShutdown();
        m_kinectSensor->Release();
        m_kinectSensor = nullptr;
    }

    m_paused = false;
}

const FrameData* CKinectHandler::GetFrameData() const
{
    return m_frameData;
}

bool CKinectHandler::IsPaused() const
{
    return m_paused;
}

void CKinectHandler::SetPaused(bool f_state)
{
    m_paused = f_state;
}

void CKinectHandler::Update()
{
    if(m_kinectSensor && !m_paused)
    {
        NUI_SKELETON_FRAME l_frame = { 0 };
        if(m_kinectSensor->NuiSkeletonGetNextFrame(0, &l_frame) >= S_OK)
        {
            for(size_t i = 0U; i < NUI_SKELETON_COUNT; i++)
            {
                const NUI_SKELETON_DATA &l_skeleton = l_frame.SkeletonData[i];
                if(l_skeleton.eTrackingState == NUI_SKELETON_TRACKED)
                {
                    m_frameData->m_tick = GetTickCount64();
                    m_frameData->m_frameTime = l_frame.liTimeStamp.QuadPart;

                    for(size_t j = 0U; j < NUI_SKELETON_POSITION_COUNT; j++)
                    {
                        m_jointFilters[j]->Update(l_skeleton.SkeletonPositions[j], l_skeleton.eSkeletonPositionTrackingState[j]);
                        const glm::vec3 &l_filtered = m_jointFilters[j]->GetFiltered();

                        JointData &l_jointData = m_frameData->m_joints[j];
                        l_jointData.x = l_filtered.x;
                        l_jointData.y = l_filtered.y;
                        l_jointData.z = l_filtered.z;
                    }

                    NUI_SKELETON_BONE_ORIENTATION l_orientations[NUI_SKELETON_POSITION_COUNT];
                    if(NuiSkeletonCalculateBoneOrientations(&l_skeleton, l_orientations) >= S_OK)
                    {
                        for(size_t j = 0U; j < NUI_SKELETON_POSITION_COUNT; j++)
                        {
                            JointData &l_jointData = m_frameData->m_joints[j];

                            const Vector4 &l_kinectRotation = l_orientations[j].absoluteRotation.rotationQuaternion;
                            const glm::quat l_newRotation(l_kinectRotation.w, l_kinectRotation.x, l_kinectRotation.y, l_kinectRotation.z);
                            const glm::quat l_oldRotation(l_jointData.rw, l_jointData.rx, l_jointData.ry, l_jointData.rz);
                            const glm::quat l_smoothedRotation = glm::slerp(l_oldRotation, l_newRotation, 0.75f);
                            l_jointData.rx = l_smoothedRotation.x;
                            l_jointData.ry = l_smoothedRotation.y;
                            l_jointData.rz = l_smoothedRotation.z;
                            l_jointData.rw = l_smoothedRotation.w;
                        }
                    }

                    break; // Only first skeleton
                }
            }
        }
    }
}
