using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SportsKinematics
{
    public class BallBehaviour : MonoBehaviour
    {

        public Vector3 m_destination;
        public Rigidbody m_body;
        private float m_mass;
        public float m_boost = 250.0f;
        private GameObject debugger;
        // Use this for initialization
        void Start()
        {
            debugger = new GameObject("DebugLine");
            m_body = GetComponent<Rigidbody>();

            m_mass = m_body.mass;

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                m_destination = GameObject.Find("PadParent").transform.position;
                ApplyImpulse();
            }
            if (Input.GetKey(KeyCode.Q))
            {
                transform.position = cursorOnTransform;
            }
        }

        void ApplyImpulse()
        {
            //reset current velocity
            m_body.velocity = Vector3.zero;
            //calculate direction of the impulse based on the position of the paddle and the ball's position
            Vector3 direction = m_destination - transform.position;
            //calculate the speed of change
            float m_speed = Vector3.Distance(m_destination, transform.position) * Time.deltaTime * m_boost;
            //calculate the impulse vector
            Vector3 m_forceToApply = m_mass * m_speed * direction.normalized;


            m_body.AddForce(m_forceToApply, ForceMode.Impulse);
            DrawDebugLines(m_destination, Color.green);
        }

        private void DrawDebugLines(Vector3 targetVec, Color col)
        {


            debugger.transform.position = transform.position;

            if (!debugger.GetComponent<LineRenderer>())
            {
                debugger.AddComponent<LineRenderer>();
                debugger.GetComponent<LineRenderer>().material = GetComponent<MeshRenderer>().material;

            }
            LineRenderer lr = debugger.GetComponent<LineRenderer>();

            Vector3[] arr = new Vector3[2];

            arr[0] = transform.position;
            arr[1] = targetVec;

            lr.SetPositions(arr);
            lr.startColor = lr.endColor = col;
            lr.widthMultiplier = 0.1f;
        }


        private static Vector3 cursorWorldPosOnNCP
        {
            get
            {
                return Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y,
                    Camera.main.nearClipPlane));
            }
        }

        private static Vector3 cameraToCursor
        {
            get
            {
                return cursorWorldPosOnNCP - Camera.main.transform.position;
            }
        }

        private Vector3 cursorOnTransform
        {
            get
            {
                Vector3 camToTrans = transform.position - Camera.main.transform.position;
                return Camera.main.transform.position +
                    cameraToCursor *
                    (Vector3.Dot(Camera.main.transform.forward, camToTrans) / Vector3.Dot(Camera.main.transform.forward, cameraToCursor));
            }
        }
    }
}