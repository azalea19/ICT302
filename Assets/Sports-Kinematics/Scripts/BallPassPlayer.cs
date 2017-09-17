using UnityEngine;

namespace SportsKinematics
{
    /// <summary>
    /// Calls to determine if ball has passed the player
    /// </summary>
    public class BallPassPlayer : MonoBehaviour
    {
        /// <summary>
        /// distance of ball from player hand
        /// </summary>
        private Vector3 m_displacement;

        /// <summary>
        /// scalar distance of ball from player hand
        /// </summary>
        private float m_distance;

        /// <summary>
        /// determines if player was leaning the correct way to hit ball
        /// </summary>
        private bool m_corrLean;

        /// <summary>
        /// used to determine leaning
        /// </summary>
        private GameObject m_spine;

        /// <summary>
        /// Acccesor for displacement
        /// </summary>
        public Vector3 Displacement
        {
            get { return m_displacement; }
        }

        /// <summary>
        /// Accessor for for distance
        /// </summary>
        public float Distance
        {
            get { return m_distance; }
        }

        /// <summary>
        /// accessor for correct leaning
        /// </summary>
        public bool SwingCorrect
        {
            get { return m_corrLean; }
        }

        /// <summary>
        /// Accessor method for spine shoulder
        /// </summary>
        public GameObject SpineShoulder
        {
            set { m_spine = value; }
        }

        /// <summary>
        /// event handler for when ball passes player
        /// </summary>
        /// <param name="co">collider for ball</param>
        void OnTriggerEnter(Collider co)
        {
            Debug.Log(co.name);
            if (co.name.Equals("Ball"))
            {
                m_distance = Vector3.Distance(co.transform.position, transform.position);
                m_displacement = co.transform.position - transform.position;

                float spineDist = Vector3.Distance(m_spine.transform.position, co.transform.position);

                if (m_distance > spineDist)
                {
                    m_corrLean = false;
                    return;
                }
                else
                {
                    m_corrLean = true;
                }

                GameObject.Find("Retriever").GetComponent<PlayerTracker>().HitOccured = true;
            }
            else
            {
                Debug.Log(co.name);
            }
        }
    }
}