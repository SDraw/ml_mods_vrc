#include "stdafx.h"

#include "CJointFilter.h"

const float CJointFilter::ms_smoothing = 0.25f;
const float CJointFilter::ms_correction = 0.25f;
const float CJointFilter::ms_prediction = 0.25f;

CJointFilter::CJointFilter()
{
    m_maxDeviationRadius = 0.05f;
    m_jitterRadius = 0.03f;
    m_history.m_rawPosition = glm::vec3(0.f);
    m_history.m_filteredPosition = glm::vec3(0.f);
    m_history.m_trend = glm::vec3(0.f);
    m_history.m_frameCount = 0U;
}

CJointFilter::~CJointFilter()
{
}

const glm::vec3& CJointFilter::GetFiltered() const
{
    return m_filteredJoint;
}

void CJointFilter::Update(const Joint &f_joint)
{
    if(f_joint.TrackingState == TrackingState::TrackingState_Inferred)
    {
        m_jitterRadius = 0.06f;
        m_maxDeviationRadius = 0.1f;
    }
    else
    {
        m_jitterRadius = 0.03f;
        m_maxDeviationRadius = 0.05f;
    }

    const glm::vec3 l_rawPosition(f_joint.Position.X, f_joint.Position.Y, f_joint.Position.Z);
    glm::vec3 l_filteredPosition;
    glm::vec3 l_predictedPosition;
    glm::vec3 l_diff;
    glm::vec3 l_trend;
    float l_length;

    // If joint is invalid, reset the filter
    if(glm::length(l_rawPosition) == 0.f) m_history.m_frameCount = 0U;

    // Initial start values
    if(m_history.m_frameCount == 0U)
    {
        l_filteredPosition = l_rawPosition;
        l_trend = glm::vec3(0.f);
        m_history.m_frameCount++;
    }
    else if(m_history.m_frameCount == 1U)
    {
        l_filteredPosition = (l_rawPosition + m_history.m_rawPosition)*0.5f;
        l_diff = l_filteredPosition - m_history.m_filteredPosition;
        l_trend = l_diff*ms_correction + m_history.m_trend*(1.0f - ms_correction);
        m_history.m_frameCount++;
    }
    else
    {
        // First apply jitter filter
        l_diff = l_rawPosition - m_history.m_filteredPosition;
        l_length = glm::length(l_diff);

        if(l_length <= m_jitterRadius) l_filteredPosition = l_rawPosition * (l_length / m_jitterRadius) + m_history.m_filteredPosition *(1.0f - l_length / m_jitterRadius);
        else l_filteredPosition = l_rawPosition;

        // Now the double exponential smoothing filter
        l_filteredPosition = l_filteredPosition * (1.0f - ms_smoothing) + (m_history.m_filteredPosition + m_history.m_trend)*ms_smoothing;

        l_diff = l_filteredPosition - m_history.m_filteredPosition;
        l_trend = l_diff*ms_correction + m_history.m_trend*(1.0f - ms_correction);
    }

    // Predict into the future to reduce latency
    l_predictedPosition = l_filteredPosition + l_trend*ms_prediction;

    // Check that we are not too far away from raw data
    l_diff = l_predictedPosition - l_rawPosition;
    l_length = glm::length(l_diff);

    if(l_length > m_maxDeviationRadius)
    {
        l_predictedPosition = l_predictedPosition*(m_maxDeviationRadius / l_length) + l_rawPosition*(1.0f - m_maxDeviationRadius / l_length);
    }

    // Save the data from this frame
    m_history.m_rawPosition = l_rawPosition;
    m_history.m_filteredPosition = l_filteredPosition;
    m_history.m_trend = l_trend;

    // Output the data
    m_filteredJoint = l_predictedPosition;
}
