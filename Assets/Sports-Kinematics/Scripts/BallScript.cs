using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SportsKinematics
{
    public class BallScript : MonoBehaviour
    {
        /// <summary>
        /// DEtermine if ball has hit opponent paddle
        /// </summary>
        private bool m_collidedPaddle = false;

        /// <summary>
        /// determine if ball has hit table
        /// </summary>
        private bool m_collidedTable = false;

        /// <summary>
        /// bounce speed of ball
        /// </summary>
        private float m_bounceSpeed;

        /// <summary>
        /// ball mass
        /// </summary>
        private float m_mass;

        /// <summary>
        /// decay amount for bouncing against table.
        /// </summary>
        private float m_bounceDecay;

        /// <summary>
        /// MAx amount ball can turn to track paddle
        /// </summary>
        private float m_maxTurnRate = 20f;

        /// <summary>
        /// rigid body of ball
        /// </summary>
        private Rigidbody m_rb;

        /// <summary>
        /// paddle to hit transform
        /// </summary>
        private Transform m_targetPaddle;

        /// <summary>
        /// target position
        /// </summary>
        private Vector3 m_myTarget;

        /// <summary>
        /// force to apply against ball
        /// </summary>
        private Vector3 m_forceToApply;

        /// <summary>
        /// start position of bal
        /// </summary>
        private Vector3 m_ballStartPosition;

        /// <summary>
        /// used to prevent application of force to ball
        /// </summary>
        private bool m_addNoForce;

        /// <summary>
        /// time to collide with opponent paddle
        /// </summary>
        private float m_timeToCollide = 0.5f;

        /// <summary>
        /// velocity of ball before freezing
        /// </summary>
        private Vector3 m_VelocityBeforeFreeze/* = Vector3.zero*/;

        /// <summary>
        /// start firing of ball
        /// </summary>
        public bool m_triggerBall = false;

        /// <summary>
        /// target to be hit as GameObject
        /// </summary>
        public GameObject m_targetManager;

        /// <summary>
        /// speed for ball to travel at
        /// </summary>
        public float m_speed;

        /// <summary>
        /// boolean to mobe ball after trigger
        /// </summary>
        public bool m_play;

        /// <summary>
        /// array fo possible materials for scalability
        /// </summary>
        public Material[] m_materials;

        /// <summary>
        /// array of possible meshes for scalability
        /// </summary>
        public Mesh[] m_meshes;

        /// <summary>
        /// enum of sports that can be used, to determine which meshe and material to apply
        /// </summary>
        public StrikerRenderer.StrikerSelection m_ballType;
        

        /// <summary>
        /// GameObject start function. Initates scene
        /// </summary>
        void Start()
        {
            m_ballStartPosition = transform.position;
            m_rb = GetComponent<Rigidbody>();
            m_mass = GetComponent<Rigidbody>().mass;
            m_play = false;

            GetComponent<MeshRenderer>().material = m_materials[(int)m_ballType];
            GetComponent<MeshFilter>().mesh = m_meshes[(int)m_ballType];
            GetComponent<MeshCollider>().sharedMesh = m_meshes[(int)m_ballType];

            switch (m_ballType)
            {
                case StrikerRenderer.StrikerSelection.Catch:
                    break;
                case StrikerRenderer.StrikerSelection.TableTennis:
                    m_bounceDecay = 0.766f;
                    GetComponent<Transform>().localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    break;
            }
        }

        /// <summary>
        /// GameObject FixedUpdate. Applies all physics and ball movement
        /// </summary>
        void FixedUpdate()
        {
            if (m_play)
            {
                //unfreeze velocity, need to be reset
                m_rb.constraints = RigidbodyConstraints.None;
                //Velocity restored after unfreezing if not Vector3.zero
                if(m_VelocityBeforeFreeze != Vector3.zero)
                {
                    m_rb.velocity = m_VelocityBeforeFreeze;
                }
                    
                if (!m_collidedPaddle)
                {
                    if (m_triggerBall)
                    {
                        if (!m_addNoForce)
                        {
                            Vector3 direction = m_myTarget - transform.position;

                            m_speed = Vector3.Distance(m_myTarget, transform.position) / m_timeToCollide;

                            m_forceToApply = m_mass * m_speed * direction.normalized;
                            //m_forceToApply *= m_speed.normalized;
                            //m_forceToApply *= ((float)Math.Sqrt(m_speed.magnitude));
                            //Debug.Log("force to apply: " + m_forceToApply);
                            AddForce(m_forceToApply.x, m_forceToApply.y, m_forceToApply.z);
                            m_addNoForce = true;
                        }
                    }

                    //If the ball has hit the table, bounce
                    if (m_collidedTable)
                    {
                        //Debug.Log("bounceForce:" + m_bounceSpeed);
                        m_rb.AddForce(0f, -m_bounceSpeed * m_bounceDecay * m_mass, 0f, ForceMode.Impulse);
                        m_collidedTable = false;
                    }
                    m_bounceSpeed = GetComponent<Rigidbody>().velocity.y;
                }

                if (m_collidedPaddle)
                {
                    //m_rb.AddForce(-m_targetPaddle.position * m_speed);
                    m_rb.useGravity = true;
                }

                //Needed to prevent freezing in place by restore call at start of function, no need to check for zero
                m_VelocityBeforeFreeze = m_rb.velocity;
            }
            else
            {
                //Preserve velocity before freezing when m_rb.velocity is not zero
                if (m_rb.velocity != Vector3.zero)
                {
                    m_VelocityBeforeFreeze = m_rb.velocity;
                }
                //Freeze ball on pause, kills velocity
                m_rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        /// <summary>
        /// Collision event for ball
        /// </summary>
        /// <param name="collision">collide for ball collision</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == "StrikerMesh")
            {
                m_VelocityBeforeFreeze = Vector3.zero;
                Debug.Log("We hit 'er broadside Capt'n!");
                m_collidedPaddle = true;
            }
            else if (collision.gameObject.name == "Top1" || collision.gameObject.name == "Top2")
            {
                m_collidedTable = true;
            }
            else if(collision.gameObject.name == "Net")
            {//do nothing
            }
        }

        /// <summary>
        /// Draw debug lines for ball flight path
        /// </summary>
        /// <param name="targetVec"></param>
        /// <param name="col"></param>
        private void DrawDebugLines(Vector3 targetVec, Color col)
        {
            GameObject debugger = new GameObject("DebugLine");

            debugger.transform.position = transform.position;

            if(!debugger.GetComponent<LineRenderer>())
                debugger.AddComponent<LineRenderer>();

            LineRenderer lr = debugger.GetComponent<LineRenderer>();

            Vector3[] arr = new Vector3[2];

            arr[0] = transform.position;
            arr[1] = targetVec;

            lr.SetPositions(arr);
            lr.startColor = lr.endColor = col;
            lr.widthMultiplier = 0.1f;
        }

        /// <summary>
        /// prepare to move ball
        /// </summary>
        /// <param name="targetVec"></param>
        public void PrepareToFire(Vector3 targetVec)//, int targetFrame, uint sampleRate
        {          
            m_myTarget = targetVec;
           // DrawDebugLines(targetVec, new Color(1,1,1));
           // DrawDebugLines(m_myTarget, new Color(0, 1, 1));
        }

        /// <summary>
        /// add force to ball
        /// </summary>
        /// <param name="forceToApply">vector of force to apply</param>
        public void AddForce(Vector3 forceToApply)
        {
            m_rb.AddForce(forceToApply, ForceMode.Impulse);
        }

        /// <summary>
        /// add force to ball
        /// </summary>
        /// <param name="forceToApplyX">force being applied in x</param>
        /// <param name="forceToApplyY">force being applied in y</param>
        /// <param name="forceToApplyZ">force being applied in z</param>
        public void AddForce(float forceToApplyX, float forceToApplyY, float forceToApplyZ)
        {
            m_rb.AddForce(forceToApplyX, forceToApplyY, forceToApplyZ, ForceMode.Impulse);
        }

        /// <summary>
        /// Sets the layer of the ball to occluded or default
        /// </summary>
        /// <param name="layer"></param>
        public void SetLayer(string layer)
        {
            gameObject.layer = LayerMask.NameToLayer(layer);
        }

        /// <summary>
        /// reset the ball in scene
        /// </summary>
        public void ResetBall()
        {
            this.transform.position = m_ballStartPosition;
            m_rb.velocity = Vector3.zero;
            m_rb.angularVelocity = Vector3.zero;
            m_VelocityBeforeFreeze = Vector3.zero;
            m_rb.useGravity = false;
            m_collidedPaddle = false;
            m_triggerBall = false;
            m_addNoForce = false;
        }

        /// <summary>
        /// accessor method for collision time
        /// </summary>
        public float TimeOfCollide
        {
            get { return m_timeToCollide; }
        }

        /// <summary>
        /// speed for ball to travel at
        /// </summary>
        public float Speed
        {
            get { return m_speed; }
        }
    }
}
