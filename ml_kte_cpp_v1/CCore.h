#pragma once

class CKinectHandler;
struct FrameData;

class CCore
{
    static const char* const ms_interfaces[];

    CKinectHandler *m_kinectHandler;
    std::thread *m_kinectThread;
    std::mutex m_kinectLock;
    std::atomic<bool> m_kinectActive;
    std::vector<FrameData*> m_frameHistory;
    size_t m_frameHistoryCount;

    CCore(const CCore &that) = delete;
    CCore& operator=(const CCore &that) = delete;

    void KinectProcess();
public:
    CCore();
    ~CCore();

    void Initialize();
    void Terminate();

    void GetTrackingData(float *p_positions, float *p_rotations);
};

