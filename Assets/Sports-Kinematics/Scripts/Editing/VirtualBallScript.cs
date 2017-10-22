using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SportsKinematics
{
    public class VirtualBallScript : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
          // Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
           // Debug.Log(Input.mousePosition);
           // transform.position =(Camera.main.ScreenToViewportPoint(Input.mousePosition));
        }
        void OnMouseDrag()
        {
            Debug.Log("sd");
            Vector3 value = new Vector3(0,0,0);
            Vector3 re = new Vector3(1, -1, 1);
            Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition));
            for(int i = 0; i < 3; i++)
            {
                if (Camera.main.ScreenToViewportPoint(Input.mousePosition)[i] > 0.5)
                {
                    value[i] = 1*re[i]*10;
                }
                else
                {
                    value[i] = -1 * re[i] * 10;
                }
            }
            
            Vector3 temp = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            for(int i = 0; i < 3; i++)
            {
                temp[i] *= value[i];
            }
            Debug.Log(Camera.main.WorldToViewportPoint(transform.position));
            if (!gameObject.GetComponent<LineRenderer>())
            {
                CreateLineRenderer(temp);
            }else
            {
                RefreshLineRender(temp);
            }
        }

        private void CreateLineRenderer(Vector3 end)
        {
            LineRenderer lr = gameObject.AddComponent<LineRenderer>();
            //lr.useWorldSpace = false;
            lr.positionCount = 2;
            
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;

            Vector3[] arr = { ( transform.position), end };
            lr.SetPositions(arr);
        }

        private void RefreshLineRender( Vector3 end)
        {
            Vector3[] arr = {(transform.position), end };
            gameObject.GetComponent<LineRenderer>().SetPositions(arr);
        }
    }
}