using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
namespace SportsKinematics
{
    public class Occlusion
    {

        /// <summary>
        /// material for colouring bones that are being modified
        /// </summary>
        public static Material m_activeJoint;

        /// <summary>
        /// material for colouring bones that are not being modified
        /// </summary>
        public static Material m_inactiveJoint;

        /// <summary>
        /// true - if the skeleton toggle is being modified
        /// </summary>
        private bool m_skeletonChange = false;
        public GameObject m_occlusionPanel;

        private Experiment m_exp;
        private GameObject m_body;


        // Use this for initialization
        public void Initialize()
        {
            m_skeletonChange = false;
            
            m_activeJoint = (Material)Resources.Load("BoneActiveMaterial");
            m_inactiveJoint = (Material)Resources.Load("BoneInactiveMaterial");
            Debug.Log(m_activeJoint);
        }
        
        public void Start(Experiment exp, GameObject body)
        {
            m_exp = exp;
            m_body = body;
        }

        /// <summary>
        /// Author: Olesia Kochergina
        /// Should only be used in the editor
        /// </summary>
        public void AddSkeletonOcclusionOptions(GameObject body)
        {

            Vector3 position = new Vector3();
            GameObject content = m_occlusionPanel.transform.GetChild(0).GetChild(0).gameObject;
            float yPos = content.transform.GetChild(Experiment.m_jointOffset - 1).position.y;
            float yDif = content.transform.GetChild(Experiment.m_jointOffset - 2).position.y- content.transform.GetChild(Experiment.m_jointOffset - 1).position.y;
            content.transform.GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener(delegate { Occlude(0); });
            content.transform.GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener(delegate { Occlude(1); });
            content.transform.GetChild(2).GetComponent<Toggle>().onValueChanged.AddListener(delegate { Occlude(2); });
            for (int i = 0; i < body.transform.childCount; i++)
            {
                GameObject option = GameObject.Instantiate(content.transform.GetChild(Experiment.m_jointOffset - 1).gameObject);
                option.name = body.transform.GetChild(i).name;
                position = content.transform.GetChild(Experiment.m_jointOffset - 1).position;
                Debug.Log(content.transform.GetChild(Experiment.m_jointOffset - 1).position.y);
                yPos -= 30;
                position.y = yPos;
                option.transform.localPosition = position;
                option.GetComponent<Toggle>().isOn = m_exp.m_occBoolArr[i + Experiment.m_jointOffset];
                //otherwise delegate increments i in all Occlude functions, so th etemporary variable j needs to be created
                int j = i;
                option.GetComponent<Toggle>().onValueChanged.AddListener(delegate { Occlude(j + Experiment.m_jointOffset); });
                option.transform.parent = content.transform;

                option.transform.localScale = content.transform.GetChild(1).localScale;

                option.transform.GetChild(1).GetComponent<Text>().text = body.transform.GetChild(i).name;

            }
        }

        /// <summary>
        /// Author: Olesia Kochergina
        /// </summary>
        /// <param name="index">0 - paddle, 1 - ball, 2..26 - skeleton</param>
        public void Occlude(int index)
        {
            if (index == 0)
            {
                Debug.Log("Paddle Occlusion");
                m_exp.m_occBoolArr[0] = !m_exp.m_occBoolArr[0];
            }
            else if (index == 1)
            {
                Debug.Log("Ball Occlusion");
                m_exp.m_occBoolArr[1] = !m_exp.m_occBoolArr[1];
            }
            else if (index == 2)
            {
                m_exp.m_occBoolArr[2] = !m_exp.m_occBoolArr[2];
                GameObject.Find("Action Renderer").GetComponent<ActionRenderer>().OccludeAllJoints(m_exp.m_occBoolArr[2]);
                //prevents modification of single joints using Occlude(index) in option.GetComponent<Toggle>().isOn callback
                m_skeletonChange = true;

                for (int i = Experiment.m_jointOffset; i < m_exp.m_occBoolArr.Length; i++)
                {
                    m_occlusionPanel.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<Toggle>().isOn = m_exp.m_occBoolArr[i];
                }
                m_skeletonChange = false;
            }
            else
            {
                GameObject temp = GameObject.Find("Opponent");
                if (!m_skeletonChange)
                    m_exp.m_occBoolArr[index] = !m_exp.m_occBoolArr[index];

                if (m_body)
                {
                    if (m_exp.m_occBoolArr[index])
                    {
                        if (m_body.transform.GetChild(index - Experiment.m_jointOffset).GetComponent<LineRenderer>())
                            m_body.transform.GetChild(index - Experiment.m_jointOffset).GetComponent<LineRenderer>().material = m_activeJoint;
                        m_body.transform.GetChild(index - Experiment.m_jointOffset).GetComponent<MeshRenderer>().material = m_activeJoint;

                        //occluded layer is not visible by the main camera
                        //temp.transform.GetChild(index - 2).gameObject.layer = LayerMask.NameToLayer("Occluded");//.SetActive(!temp.transform.GetChild(index - 2).gameObject.activeSelf);
                        //m_occFrameMin[index] = m_renderFrame;
                    }
                    else
                    {
                        if (m_body.transform.GetChild(index - Experiment.m_jointOffset).GetComponent<LineRenderer>())
                            m_body.transform.GetChild(index - Experiment.m_jointOffset).GetComponent<LineRenderer>().material = m_inactiveJoint;
                        m_body.transform.GetChild(index - Experiment.m_jointOffset).GetComponent<MeshRenderer>().material = m_inactiveJoint;
                        //temp.transform.GetChild(index - 2).gameObject.layer = LayerMask.NameToLayer("Default");
                        //m_occFrameMin[index] = m_renderFrame;
                    }
                }
            }
        }


    }
}