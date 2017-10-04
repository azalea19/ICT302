using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: Olesia Kochergina
/// Date: 19/09/2017
/// Custom range slider
/// </summary>

namespace SportsKinematics.UI
{
    public class RangeSliderScript : MonoBehaviour
    {

        public ActionRenderer m_actionRenderer;

        protected IntervalSlider m_slider;

        private bool start = false;
        private int m_prevFrame = 0;


        protected int m_prevLowerValue = 0;
        protected int m_prevUpperValue = 0;


        private Text m_lowerText = null;
        private Text m_upperText = null;



        void Start()
        {
            m_lowerText = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>();
            m_upperText = transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (m_prevFrame != m_actionRenderer.RenderFrame)
            {
                m_slider.lowerValue = m_actionRenderer.RenderFrame;
            }

            m_prevFrame = m_actionRenderer.RenderFrame;
        }


        public virtual void UpdateHandles()
        {
            // if (m_prevUpperValue != (int)m_slider.upperValue)
            {
                 m_actionRenderer.FrameMax = (int)m_slider.upperValue;
                m_prevUpperValue = (int)m_slider.upperValue;
               
            }

            if (m_prevLowerValue != (int)m_slider.lowerValue && m_actionRenderer.RenderFrame != (int)m_slider.lowerValue)
            {
                m_actionRenderer.RenderFrame = (int)m_slider.lowerValue;
                
                m_actionRenderer.UpdateBody();
                m_prevLowerValue = (int)m_slider.lowerValue;
                GameObject.Find("PlayButton").GetComponent<Pause>().TaskOnClick(false);
            }
            UpdateHandleText();
        }

        protected void UpdateHandleText()
        {
            if (!m_lowerText)
            {
                Start();
            }
            m_lowerText.text = ((int)m_slider.lowerValue).ToString();
            m_upperText.text = ((int)m_slider.upperValue).ToString();
        }

        public void Initialize()
        {
            m_slider = GetComponent<IntervalSlider>();
            m_slider.maxValue = m_actionRenderer.FrameMax;
            m_slider.lowerValue = 0;
            m_slider.upperValue = m_slider.maxValue;
            m_slider.minValue = 0;

            m_prevLowerValue = (int)m_slider.lowerValue;
            m_prevUpperValue = (int)m_slider.upperValue;
            if (!m_lowerText)
                Start();
            UpdateHandleText();
        }

    }
}
