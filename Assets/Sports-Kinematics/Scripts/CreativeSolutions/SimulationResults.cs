using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

namespace SportsKinematics
{
    public class SimulationResults : MonoBehaviour
    {
        private float m_smallestDistToStriker; //The smallest  distance from the paddle to the ball, used for the results at the end if the ball was not hit
        private float m_smallestDistTime;      //The frame in which the smallest distance occurred, this is so there is potential for the exact frame to be simulated to show the setup of the scene at that point.
        public bool m_isHitByPlayer { get; set; }            //If the ball hit theplayers paddle, used for the result
        public float m_hitTime;     //The time when the players paddle hit the ball
        private Vector3 m_directionToBall;   //direction to the ball from striker

        public GameObject m_testResult; //text output for the player using the Vive
        public GameObject m_playerPaddle;   //the players paddle
        public GameObject m_ball;   //the ball

        SimulationManager m_simManager;

        // Use this for initialization
        void Start()
        { //Setup starting data for some variables
            m_isHitByPlayer = false;
            m_smallestDistToStriker = GetDistanceToStriker();   //the smallest distance is the initial distance (should be the largest possible distance)
            m_simManager = this.GetComponent<SimulationManager>();
        }

        void Update()
        {
            //NOTE: the only takes the origins of the objects into account, in order to make it more accurate a mesh-mesh distance must be used
            if (GetDistanceToStriker() < m_smallestDistToStriker)   //If the ball and paddle are closer than they have ever been
            {
                //update the data for the smallest distance (time and the vec3)
                m_smallestDistToStriker = GetDistanceToStriker();
                m_directionToBall = GetDirectionToBall();
                m_smallestDistTime = GetCurrentTime();
            }

            if (m_isHitByPlayer)
            {
                m_testResult.GetComponent<TextMesh>().text = "Smallest Distance: " + m_smallestDistToStriker.ToString("F2") + "   HIT THE BALL!";
            }
            else
            {
                m_testResult.GetComponent<TextMesh>().text = "Smallest Distance: " + m_smallestDistToStriker.ToString("F2") + "  Distance to ball: " + GetDistanceToStriker().ToString("F2");
            }
        }

        float GetCurrentTime()       //Gets the current total time for this recording
        {
            return m_simManager.GetComponent<SimulationManager>().m_time;
        }

        public string GetResultsString()
        {
            string results = "Outcome: ";

            if (m_isHitByPlayer)
            {
                results += "Ball hit.\n";
                results += "Time hit: " + m_hitTime + "\n";
            }
            else
            {
                results += "Ball missed.";
                results += "Striker was closest to the paddle at: " + m_smallestDistTime + "\n";
                results += "The distance between the ball and paddle was: " + m_smallestDistToStriker + "\n";
                results += "The direction from the paddle to the ball was:\n";
                results += "X axis (Left/Right): " + m_directionToBall.x + "\n";
                results += "Y axis (Up/Down): " + m_directionToBall.y + "\n";
                results += "Z axis (Forward/Back): " + m_directionToBall.z + "\n";
            }

            return results;
        }

        float GetDistanceToStriker()              //Gets the distance between the origin of the paddle and ball
        {
            float distance = Vector3.Distance(m_ball.transform.position, m_playerPaddle.transform.position);
            return distance;
        }

        Vector3 GetDirectionToBall()              //Gets the distance between the origin of the paddle and ball
        {
            Vector3 direction = m_playerPaddle.transform.position - m_ball.transform.position;
            return direction;
        }
    }
}
