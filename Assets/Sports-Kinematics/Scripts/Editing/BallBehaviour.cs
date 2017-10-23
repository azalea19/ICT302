using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SportsKinematics
{
    public class BallBehaviour : MonoBehaviour
    {

        public Vector3 m_destination;
        private Vector3 m_initPosition;
        public InputField m_field;
        public Rigidbody m_body;
        private float m_mass;
        public float m_boost = 200.0f;
        private float m_ballSpeed = 100;
        private GameObject debugger;
        // Use this for initialization
        void Start()
        {
            debugger = new GameObject("DebugLine");
            m_body = GetComponent<Rigidbody>();
            m_initPosition = transform.position;
            m_mass = m_body.mass;

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Reset();
                transform.position = cursorOnTransform;
                m_initPosition = transform.position;
            }
        }

        void Reset()
        {
            transform.position = m_initPosition;
            m_body.velocity = Vector3.zero;
            m_body.angularVelocity = Vector3.zero;
            m_body.angularDrag = 0.0f;
            m_body.ResetInertiaTensor();
        }
        public void UpdateDestination()
        {

            m_destination = GameObject.Find("PadParent").transform.position;
        }

        public void UpdateSpeed()
        {
            if (m_field && m_field.text != "" && m_field.text !="0")
            {
                Debug.Log("speed");
                float.TryParse(m_field.text, out m_ballSpeed);
            }
        }

        public void ApplyImpulse()
        {
            Reset();
            Vector3 direction = m_destination - transform.position;
            Debug.Log("ball speed" + m_ballSpeed);
            transform.position = m_destination + GameObject.Find("PadParent").transform.forward;
            float m_speed = Vector3.Distance(m_destination, transform.position) / 0.05f;
            m_speed /= (1.0f / (m_ballSpeed/100));
            Vector3 m_forceToApply = m_mass * m_speed * direction.normalized;
            //reset current velocity
            // m_body.velocity = Vector3.zero;
            //calculate direction of the impulse based on the position of the paddle and the ball's position
            //Vector3 direction = m_destination - transform.position;
            ////calculate the speed of change
            //float m_speed = Vector3.Distance(m_destination, transform.position) * Time.deltaTime * m_boost;
            ////calculate the impulse vector
            //Vector3 m_forceToApply = m_mass * m_speed * direction.normalized;


            Vector3 imp = new Vector3();
            float time = .5f;
            for (int i = 0; i < 3; i++)
            {
                imp[i] = m_mass * (m_destination[i] - transform.position[i]) / time - 0.0f;
            }
            imp.y = m_mass * (m_destination.y - transform.position.y) / time - 0 - 0.5f * -9.81f * time;


            m_body.AddForce(m_forceToApply, ForceMode.Impulse);
            // m_body.velocity = m_forceToApply;
            // m_body.useGravity = false;

            Debug.Log("Impulse: " + imp + "  ");
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