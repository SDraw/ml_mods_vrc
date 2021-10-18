#pragma once
// Modified joint smoothing code from https://social.msdn.microsoft.com/Forums/en-US/045b058a-ae3a-4d01-beb6-b756631b4b42/joint-smoothing-code?forum=kinectv2sdk

class CJointFilter final
{
    struct FilterData
    {
        glm::vec3 m_rawPosition;
        glm::vec3 m_filteredPosition;
        glm::vec3 m_trend;
        unsigned long m_frameCount;
    };

    static const float ms_smoothing;
    static const float ms_correction;
    static const float ms_prediction;
    float m_jitterRadius;
    float m_maxDeviationRadius;

    FilterData m_history;
    glm::vec3 m_filteredJoint;
public:
    CJointFilter();
    ~CJointFilter();

    const glm::vec3& GetFiltered() const;

    void Update(const Joint &f_joint);
};
