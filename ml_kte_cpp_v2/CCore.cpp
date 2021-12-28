#include "stdafx.h"
#include "CCore.h"

#include "CKinectHandler.h"

enum HistoryIndex : size_t
{
    HI_Previous = 0U,
    HI_Last,

    HI_Count
};

CCore::CCore()
{
    m_kinectHandler = nullptr;
    m_kinectThread = nullptr;
    m_kinectActive = false;
    m_frameHistoryCount = 0U;
}

CCore::~CCore()
{
}

void CCore::Initialize()
{
    m_kinectHandler = new CKinectHandler();
    m_kinectActive = true;
    m_kinectThread = new std::thread(&CCore::KinectProcess, this);
}

void CCore::Terminate()
{
    if (m_kinectThread)
    {
        m_kinectActive = false;
        m_kinectThread->join();
        m_kinectThread = nullptr;
    }

    delete m_kinectHandler;
    m_kinectHandler = nullptr;

    for (auto l_frame : m_frameHistory) delete l_frame;
    m_frameHistory.clear();
    m_frameHistoryCount = 0U;
}

void CCore::GetTrackingData(float *p_positions, float *p_rotations)
{
    if (m_kinectLock.try_lock())
    {
        const FrameData *l_frameData = m_kinectHandler->GetFrameData();
        if (m_frameHistoryCount == HI_Count)
        {
            if (m_frameHistory[HI_Last]->m_frameTime != l_frameData->m_frameTime)
            {
                // Copy 'Last' to 'Previous', current to 'Last'
                std::memcpy(m_frameHistory[HI_Previous], m_frameHistory[HI_Last], sizeof(FrameData));
                std::memcpy(m_frameHistory[HI_Last], l_frameData, sizeof(FrameData));
            }
        }
        else
        {
            FrameData *l_frameSnapshot = new FrameData();
            std::memcpy(l_frameSnapshot, l_frameData, sizeof(FrameData));
            m_frameHistory.push_back(l_frameSnapshot); // Frame time is irrelevant
            m_frameHistoryCount++;
        }
        m_kinectLock.unlock();
    }

    // Smooth history data
    if (m_frameHistoryCount == HI_Count)
    {
        const ULONGLONG l_diff = (GetTickCount64() - m_frameHistory[HI_Last]->m_tick);
        float l_smooth = static_cast<float>(l_diff) / 33.333333f;
        l_smooth = glm::clamp(l_smooth, 0.f, 1.f);

        for (size_t i = 0U; i < _JointType::JointType_Count; i++)
        {
            const JointData &l_jointA = m_frameHistory[HI_Previous]->m_joints[i];
            const JointData &l_jointB = m_frameHistory[HI_Last]->m_joints[i];

            const glm::vec3 l_jointPosA(-l_jointA.x, l_jointA.y, -l_jointA.z);
            const glm::vec3 l_jointPosB(-l_jointB.x, l_jointB.y, -l_jointB.z);

            const glm::quat l_jointRotA(l_jointA.rw, -l_jointA.rx, l_jointA.ry, -l_jointA.rz);
            const glm::quat l_jointRotB(l_jointB.rw, -l_jointB.rx, l_jointB.ry, -l_jointB.rz);

            const glm::vec3 l_linearPos = glm::mix(l_jointPosA, l_jointPosB, l_smooth);
            const glm::quat l_linearRot = glm::slerp(l_jointRotA, l_jointRotB, l_smooth);

            p_positions[i * 3] = l_linearPos.x;
            p_positions[i * 3 + 1] = l_linearPos.y;
            p_positions[i * 3 + 2] = l_linearPos.z;

            p_rotations[i * 4] = l_linearRot.x;
            p_rotations[i * 4 + 1] = l_linearRot.y;
            p_rotations[i * 4 + 2] = l_linearRot.z;
            p_rotations[i * 4 + 3] = l_linearRot.w;
        }
    }
}

void CCore::KinectProcess()
{
    const std::chrono::milliseconds l_threadDelay(33U);

    bool l_initialized = m_kinectHandler->Initialize();
    while (m_kinectActive)
    {
        if (l_initialized)
        {
            m_kinectLock.lock();
            m_kinectHandler->Update();
            m_kinectLock.unlock();
        }
        else l_initialized = m_kinectHandler->Initialize();

        std::this_thread::sleep_for(l_threadDelay);
    }

    if (l_initialized) m_kinectHandler->Terminate();
}
