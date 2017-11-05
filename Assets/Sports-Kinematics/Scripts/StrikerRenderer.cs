using System;
using UnityEngine;
using System.Collections;

namespace SportsKinematics
{
    /// <summary>
    /// Renders the striker object.
    /// </summary>
    public class StrikerRenderer : MonoBehaviour//FR1 - Virtual opponent modelling from captured data.
    {
        /// <summary>
        /// logs the first time the striker is rendered
        /// </summary>
        private float m_startTime;

        /// <summary>
        /// length in which the ball must travel to his the paddle
        /// </summary>
        private float m_journeyLength;
        /// <summary>
        /// initial and final orientations for the striking object
        /// </summary>
        public Quaternion m_start, m_final;
        /// <summary>
        /// paddle mesh to be drawn
        /// </summary>
        private GameObject m_strikerObject;
        /// <summary>
        /// rendering method for the paddle
        /// </summary>
        private MeshRenderer m_Renderer;
        /// <summary>
        /// ilter to hold the mesh
        /// </summary>
        private MeshFilter m_Filter;
        /// <summary>
        /// collider for physics
        /// </summary>
        private Collider m_Collider;
        /// <summary>
        /// paddle parent, allowing for paddle transforms
        /// </summary>
        private GameObject m_strikerObjectParent;
        /// <summary>
        /// Handles collisions for the paddle mesh
        /// </summary>
        private StrikerCollision m_collision;
        /// <summary>
        /// Array of models to be loaded
        /// </summary>
        public Mesh[] m_strikers;
        /// <summary>
        /// Array of textures or materials for models
        /// </summary>
        public Material[] m_strikerMaterials;
        /// <summary>
        /// speed at which the ball shall travel
        /// </summary>
        public float m_speed = 1.0f;

        /// <summary>
        /// Type of striker object to render
        /// </summary>
        [Serializable]
        public enum StrikerSelection
        {
            //catch renderes no striker object
            Catch = 0,
            //renders a ping pong paddle
            TableTennis = 1,
            /*ADD MORE AS NEEDED*/
        }//selection method or the striking object to be drawn. Adds scalability to other sports.
        public StrikerSelection m_StrikerSelection; //Selected sport, given enum above

        /// <summary>
        /// Initializes striker objects to null
        /// </summary>
        void Start()
        {
            m_strikerObject = null;
            m_Renderer = null;
            m_Filter = null;
            m_strikerObjectParent = null;
            m_collision = null;
            m_Collider = null;
        }

        /// <summary>
        /// If striker is set, gets strikers current transform
        /// </summary>
        /// <returns>Strikers Transform</returns>
        public Transform GetStrikerLocation()
        {
            if (m_strikerObject != null)
            {
                return m_strikerObject.transform;
            }
            Debug.Log("StrikerObject.transform was null");
            return null;
        }

        /// <summary>
        /// Draw a designated striking tool attached to a hand
        /// </summary>
        /// <param name="jointObj">Position of joint to affix paddle to.</param>
        /// <author>Justin Johnston, Aiden Triffitt</author>
        /// <date>29/04/2017</date>
        public void CreateStriker(Transform jointTran, bool occ)//FR1 - Virtual opponent modelling from captured data.
        {
            if (!m_strikerObject)
            {
                m_startTime = Time.time;

                m_strikerObject = new GameObject("StrikerMesh");
                m_strikerObjectParent = new GameObject("Striker");

                m_strikerObject.transform.position = jointTran.position;

                m_strikerObjectParent.transform.rotation = m_start;

                m_journeyLength = Quaternion.Angle(m_start, m_final);

                switch (m_StrikerSelection)
                {
                    case StrikerSelection.TableTennis:
                        m_Renderer = m_strikerObject.AddComponent<MeshRenderer>();
                        m_Filter = m_strikerObject.AddComponent<MeshFilter>();
                        m_Filter.mesh = m_strikers[(int)m_StrikerSelection];
                        m_Collider = m_strikerObject.AddComponent<MeshCollider>();
                        m_strikerObject.GetComponent<MeshCollider>().sharedMesh = m_Filter.mesh;
                        m_strikerObject.GetComponent<MeshCollider>().skinWidth = 2f;
                        m_Renderer.material = m_strikerMaterials[(int)m_StrikerSelection];
                        m_collision = m_strikerObject.AddComponent<StrikerCollision>();
                        break;
                    case StrikerSelection.Catch:
                        m_Collider = m_strikerObject.AddComponent<SphereCollider>();
                        break;
                }
                
                if (!occ)
                {
                    m_strikerObject.layer = LayerMask.NameToLayer("Occluded");
                }
            }

            m_strikerObjectParent.transform.parent = jointTran;
            m_strikerObject.transform.parent = m_strikerObjectParent.transform;

            GameObject.Find("PaddleParent").transform.parent = jointTran;
            GameObject.Find("PaddleParent").transform.localPosition = new Vector3(-0.59f, 3.83f, 3.59f);
        }

        /// <summary>
        /// Converts a float array to a quaternion
        /// </summary>
        /// <param name="f">Converted quaternion</param>
        /// <returns></returns>
        private Quaternion FloatToQuat(float[] f)
        {
            return new Quaternion(f[0], f[1], f[2], f[3]);
        }

        /// <summary>
        /// Conerts quaternion to float array
        /// </summary>
        /// <param name="q">Converted float array</param>
        /// <returns></returns>
        private float[] QuatToFloat(Quaternion q)
        {
            float[] f = new float[4];
            f[0] = q.x;
            f[1] = q.y;
            f[2] = q.z;
            f[3] = q.w;

            return f;
        }

        /// <summary>
        /// Refresh the striker transform on another specific transform.
        /// If occluded is true wont render.
        /// </summary>
        /// <param name="jointTran">Transform to render striker on</param>
        /// <param name="nextRot">To remove</param>
        /// <param name="occ">Determines whether or not to render striker</param>
        public void RefreshStriker(Transform jointTran, Quaternion nextRot, bool occ)//FR1 - Virtual opponent modelling from captured data.
        {
            float distCovered = (Time.time - m_startTime) * m_speed;
            float fracJourney = distCovered / m_journeyLength;

            if (!m_strikerObject)
            {
                m_strikerObject = new GameObject("StrikerMesh");
                m_strikerObjectParent = new GameObject("Striker");
            }

            if (!occ)
            {
                m_strikerObject.layer = LayerMask.NameToLayer("Occluded");
            }

            m_strikerObject.transform.rotation = jointTran.rotation;
            m_strikerObject.transform.position = jointTran.position;

            m_strikerObject.transform.parent = m_strikerObjectParent.transform;
        }
    }
}
