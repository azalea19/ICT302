using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SportsKinematics
{
    public class BallSlider : MonoBehaviour
    {

        public ActionRenderer m_renderer;
        private  Slider m_slider;
        private Text m_text = null;

        void Start()
        {
            m_slider = GetComponent<Slider>();
            m_text = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>();
            m_slider.value = m_renderer.m_exp.FrameBall-m_renderer.m_exp.FrameStart;
            m_slider.minValue = 0;
            m_slider.maxValue = m_renderer.m_exp.FrameEnd - m_renderer.m_exp.FrameStart;
        }

        public void BallPlacement()
        {
            m_renderer.m_exp.FrameBall = (int)m_slider.value + m_renderer.m_exp.FrameStart;
            m_renderer.RenderFrame = m_renderer.m_exp.FrameBall;
            m_renderer.UpdateBody();
            m_text.text = (m_renderer.m_exp.FrameBall - m_renderer.m_exp.FrameStart).ToString();
        }
    }
}