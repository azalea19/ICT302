using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SportsKinematics.UI
{
    public class SaveAsDataScript : MonoBehaviour
    {
        public ActionRenderer m_renderer;
        public InputField m_text;


        public void Save()
        {
            m_renderer.SaveAsData(m_text.text);
        }

    }
}
