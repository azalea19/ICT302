using SportsKinematics;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

/// <summary>
/// Used to scale VR movement speed relative to last position.
/// Also used to snap kinect body to the VR position.
/// </summary>
///
public class VRmovement : MonoBehaviour
{
    /// <summary>
    /// last position of VR position, used to determine direction moved
    /// </summary>
    private Vector3 m_lastPos = Vector3.zero;
    /// <summary>
    /// movement scaling factor
    /// </summary>
    public float XZ_factor = 12.0f;

    /// <summary>
    /// capture facade, used to access body data
    /// </summary>
    public GameObject captureFacade;
    /// <summary>
    /// kinect game object to add capture facade in unity
    /// </summary>
    private CaptureFacade m_KinectFacade;
    /// <summary>
    /// tracked body to snap to VR position
    /// </summary>
    private GameObject trackedBody;
    /// <summary>
    /// render game object to add action render in unity
    /// </summary>
    public GameObject m_renderer;
    /// <summary>
    /// action renderer variable to render tracked body
    /// </summary>
    private ActionRenderer m_actionRenderer;

    /// <summary>
    /// Init kinect facade as well as action render.
    /// Set VR position to line-up with center table position
    /// </summary>
    ///
    private void Start()
    {
        m_KinectFacade = captureFacade.GetComponent<CaptureFacade>();
        m_actionRenderer = m_renderer.GetComponent<ActionRenderer>();
        transform.position = new Vector3(transform.position.x-1f, transform.position.y, transform.position.z + 0.2f);
    }

    /// <summary>
    /// Update VR positions last position and scale in the correct
    /// direction. Also snaps body to VR every update frame.
    /// </summary>
    ///
    void Update()
    {
        if (m_lastPos == Vector3.zero)
        {
            m_lastPos = transform.position;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y - 1.2f, transform.position.z);
        var offset = transform.position - m_lastPos;
        transform.parent.position += offset * XZ_factor;
        m_lastPos = transform.position;

        SnapKinectBodyToVR();
    }
    /// <summary>
    /// Snaps Kinect body to VR head position as well as
    /// updating the line render and removing the head cube.
    /// </summary>
    /// 
    public void SnapKinectBodyToVR()
    {
        trackedBody = m_KinectFacade.BodyView.TrackedBody;

        if (trackedBody)
        {
            Dictionary<Kinect.JointType, float[]> postions = new Dictionary<Windows.Kinect.JointType, float[]>();
            Dictionary<Kinect.JointType, float[]> orientation = new Dictionary<Windows.Kinect.JointType, float[]>();

            for (int i = 0; i < 25; i++)
            {
                float[] p = new float[3];
                float[] o = new float[3];


                Vector3 pos = trackedBody.transform.Find(((Kinect.JointType)i).ToString()).transform.position;
                p[0] = pos.x;
                p[1] = pos.y;
                p[2] = pos.z;

                Quaternion ori = trackedBody.transform.Find(((Kinect.JointType)i).ToString()).transform.rotation;
                o[0] = pos.x;
                o[1] = pos.y;
                o[2] = pos.z;

                postions.Add((Kinect.JointType)i, p);
                orientation.Add((Kinect.JointType)i, o);
            }

            for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
            {
                if(!jt.ToString().Equals("Neck"))
                    m_actionRenderer.RefreshBodyLines(trackedBody, postions, trackedBody.transform.Find(jt.ToString()).transform, jt);

                trackedBody.transform.Find(jt.ToString()).transform.rotation = new Quaternion(orientation[jt][0], orientation[jt][1], orientation[jt][2], 1);

                if(trackedBody.transform.Find("Head").gameObject)
                    trackedBody.transform.Find("Head").gameObject.layer = LayerMask.NameToLayer("Occluded");

                //trackedBody.transform.FindChild("Neck").gameObject.layer = LayerMask.NameToLayer("Occluded");
            }

            trackedBody.transform.position = transform.position - trackedBody.transform.Find("Neck").localPosition;
        }
    }
}