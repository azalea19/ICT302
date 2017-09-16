using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreativeBall : MonoBehaviour {

    private Vector3 m_position; //the location of the ball this frame, repalce this with the transform
    private int m_currentFrame; //Make a time object/manager which has the current recording frame for all objects in the scene
    private bool m_isOccluded;  //Whether the ball is occluded, used to stop the mesh render being set every frame

    public bool m_isHitByPlayer;            //If the ball hit theplayers paddle, used for the result
    public float m_smallestDistToStriker; //The smallest  distance from the paddle to the ball, used for the results at the end if the ball was not hit
    public double m_smallestDistFrame;      //The frame in which the smallest distance occurred, this is so there is potential for the exact frame to be simulated to show the setup of the scene at that point.
    public GameObject m_playerPaddle;       //The player's paddle, used to detect the distance between the ball and paddle

    public GameObject testResult;
    public AudioClip m_hitSound;

    // Use this for initialization
    void Start () { //Setup starting data for some variables
        m_isHitByPlayer = false;
        m_isOccluded = false;
        m_smallestDistToStriker = GetDistanceToStriker();   //the smallest distance is the initial distance (should be the largest possible distance)
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 translation = new Vector3(0, 0, 0.02f);
        m_position = GetBallPosition(GetCurrentTime()) + translation;
        this.transform.position = m_position;
        if (GetOcclusion(GetCurrentTime()) == true)     //If the file says the ball should be occluded this frame
        {
            if(m_isOccluded == false)                   //If the ball is not already occluded, might comment out this if statement if there is no performance difference
            {
                this.GetComponent<Renderer>().enabled = false;      //Then make the object invisible
            }
        }
        else
        {
            if(m_isOccluded == true)                //If the ball is occluded, but the file states the ball should not be occluded
            {
                this.GetComponent<Renderer>().enabled = true;   //make the object visible
            }
        }

        //NOTE: the only takes the origins of the objects into account, in order to make it more accurate a mesh-mesh distance must be used
        if(GetDistanceToStriker() < m_smallestDistToStriker)   //If the ball and paddle are closer than they have ever been
        {

            //update the data for the smallest distance (time and the vec3)
            m_smallestDistToStriker = Vector3.Distance(this.transform.position, m_playerPaddle.transform.position);
            m_smallestDistFrame = GetCurrentTime();
        }

        if(m_isHitByPlayer)
        {
            testResult.GetComponent<TextMesh>().text = "Smallest Distance: " + m_smallestDistToStriker + "   HIT THE BALL!";
        }
        else
        {
            testResult.GetComponent<TextMesh>().text = "Smallest Distance: " + m_smallestDistToStriker + "  Distance to ball: " + GetDistanceToStriker();
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "player_Paddle")
        {
            m_isHitByPlayer = true;
        }

        //AudioSource audio = GetComponent<AudioSource>();
        AudioSource.PlayClipAtPoint(m_hitSound, this.transform.position);
    }

    int GetCurrentTime()       //Gets the current number of the recordings frame
    {
        return 0;
    }

    bool GetOcclusion(int currentTime)     //Returns whether the ball is occluded at this frame in the config file
    {
        return false;
    }

    Vector3 GetBallPosition(int currentTime)   //Gets the balls position at a specified frame from the data files
    {
        Vector3 ballPos = this.transform.position;
        return ballPos;
    }

    float GetDistanceToStriker()              //Gets the distance between the origin of the paddle and ball
    {
        float distance = Vector3.Distance(this.transform.position, m_playerPaddle.transform.position);
        return distance;
    }
}
