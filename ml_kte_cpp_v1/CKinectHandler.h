#pragma once

class CJointFilter;
struct JointData
{
    float x, y, z;
    float rx, ry, rz, rw;
};
struct FrameData
{
    JointData m_joints[NUI_SKELETON_POSITION_COUNT];
    LONGLONG m_frameTime;
    ULONGLONG m_tick;
};

class CKinectHandler final
{
    INuiSensor* m_kinectSensor;
    CJointFilter *m_jointFilters[NUI_SKELETON_POSITION_COUNT];
    FrameData *m_frameData;

    std::atomic<bool> m_paused;

    CKinectHandler(const CKinectHandler &that) = delete;
    CKinectHandler& operator=(const CKinectHandler &that) = delete;

    void Cleanup();
public:
    explicit CKinectHandler();
    ~CKinectHandler();

    bool Initialize();
    void Terminate();

    const FrameData* GetFrameData() const;

    bool IsPaused() const;
    void SetPaused(bool p_state);

    void Update();
};
