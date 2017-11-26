using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SportsKinematics.UI
{
    public class RangeSliderOcclusionScript : RangeSliderScript
    {

        protected override void Update()
        {
            //if (m_prevFrame != m_actionRenderer.RenderFrame)
            //{
            //    m_slider.lowerValue = m_actionRenderer.RenderFrame;
            //}

            //m_prevFrame = m_actionRenderer.RenderFrame;
        }

        public override void UpdateHandles()
        {
            // if (m_prevUpperValue != (int)m_slider.upperValue)
            {
                // m_actionRenderer.FrameMax = (int)m_slider.upperValue;
                m_prevUpperValue = (int)m_slider.upperValue;
                for (int i = 0; i < m_actionRenderer.m_exp.m_occBoolArr.Length; i++)
                {
                    if (m_actionRenderer.m_exp.m_occBoolArr[i])
                        m_actionRenderer.m_exp.m_occFrameMax[i] = (int)m_slider.upperValue + m_actionRenderer.m_exp.FrameStart;
                }
            }

            if (m_prevLowerValue != (int)m_slider.lowerValue && CorrectedFrameStart() != (int)m_slider.lowerValue)
            {
                //m_actionRenderer.RenderFrame = (int)m_slider.lowerValue;
                for (int i = 0; i < m_actionRenderer.m_exp.m_occBoolArr.Length; i++)
                {
                    if (m_actionRenderer.m_exp.m_occBoolArr[i])
                        m_actionRenderer.m_exp.m_occFrameMin[i] = (int)m_slider.lowerValue + m_actionRenderer.m_exp.FrameStart ;
                }
                m_actionRenderer.UpdateBody();
                m_prevLowerValue = (int)m_slider.lowerValue;
                GameObject.Find("PlayButton").GetComponent<Pause>().TaskOnClick(false);
            }
            UpdateHandleText();
        }
    }
}