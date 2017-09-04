using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SportsKinematics
{
    /// <summary>
    /// Handles collisions for Strikers.
    /// </summary>
    public class StrikerCollision : MonoBehaviour
    {
        /// <summary>
        /// Callback that sends action renderer bool when a
        /// collision occurs.
        /// </summary>
        /// <param name="co">Collision event</param>
        void OnCollisionEnter(Collision co)
        {
            if (co.gameObject.name == "HandRight" || co.gameObject.name == "HandLeft")
                return;

            if (co.gameObject.name == "Ball")
            {
                GameObject.Find("SportsKinematics/ActionRenderer").GetComponent<ActionRenderer>().m_hitOccured = true;
            }
        }
    }
}