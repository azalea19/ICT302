using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

public struct BallData
{
    public Vector3 ballPosition;
    public bool isOccluded;
}

public class CreativeBall : MonoBehaviour {

    public bool m_isHitByPlayer;            //If the ball hit theplayers paddle, used for the result
    public float m_smallestDistToStriker; //The smallest  distance from the paddle to the ball, used for the results at the end if the ball was not hit
    public double m_smallestDistTime;      //The frame in which the smallest distance occurred, this is so there is potential for the exact frame to be simulated to show the setup of the scene at that point.
    public GameObject m_playerPaddle;       //The player's paddle, used to detect the distance between the ball and paddle

    private SortedDictionary<float, BallData> actionBallData;   //The recorded ball data for this recording

    public GameObject testResult;
    public AudioClip m_hitSound;

    // Use this for initialization
    void Start () { //Setup starting data for some variables
        m_isHitByPlayer = false;
        m_smallestDistToStriker = GetDistanceToStriker();   //the smallest distance is the initial distance (should be the largest possible distance)
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 translation = new Vector3(0, 0, 0.02f);
        this.transform.position = GetBallPosition(GetCurrentTime()) + translation;

        if (GetOcclusion(GetCurrentTime()) == true)     //If the file says the ball should be occluded this frame
        {
            this.GetComponent<Renderer>().enabled = false;      //Then make the object invisible
        }
        else
        {
            this.GetComponent<Renderer>().enabled = true;   //make the object visible
        }

        //NOTE: the only takes the origins of the objects into account, in order to make it more accurate a mesh-mesh distance must be used
        if(GetDistanceToStriker() < m_smallestDistToStriker)   //If the ball and paddle are closer than they have ever been
        {

            //update the data for the smallest distance (time and the vec3)
            m_smallestDistToStriker = Vector3.Distance(this.transform.position, m_playerPaddle.transform.position);
            m_smallestDistTime = GetCurrentTime();
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

    void LoadRecording(string recordingFile, string configFile)
    {
        using (var reader = new StreamReader(@recordingFile))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var data = line.Split(',');

                BallData tmpData = new BallData();
                tmpData.ballPosition.x = float.Parse(data[0], CultureInfo.InvariantCulture.NumberFormat);
                tmpData.ballPosition.y = float.Parse(data[1], CultureInfo.InvariantCulture.NumberFormat);
                tmpData.ballPosition.z = float.Parse(data[2], CultureInfo.InvariantCulture.NumberFormat);
                float time = float.Parse(data[3], CultureInfo.InvariantCulture.NumberFormat);

                actionBallData.Add(time, tmpData);
            }
        }

        List<float> keys = new List<float>(actionBallData.Keys);

        //Read in ball occlusion data here, im assuming its simply - time,true/false
        //This is probably broke, and is not finished
        using (var reader = new StreamReader(configFile))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var data = line.Split(',');

                float time = float.Parse(data[0], CultureInfo.InvariantCulture.NumberFormat);
                bool isOccluded = bool.Parse(data[1]);

                BallData tmp = new BallData();
                if (actionBallData.ContainsKey(time))
                {
                    tmp = actionBallData[time];
                    tmp.isOccluded = isOccluded;
                    actionBallData[time] = tmp;
                }else
                {
                    float index = keys.BinarySearch(time);
                    //if(Math.Abs(time - index) < Math.Abs(time - index - 1))
                    //{
                    tmp = actionBallData[index];
                    tmp.isOccluded = isOccluded;
                    actionBallData[index] = tmp;
                   // }
                    //else
                    //{
                    //    actionBallData[index - 1].isOccluded = isOccluded;
                    //}
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "player_Paddle")
        {
            m_isHitByPlayer = true;
        }

        AudioSource.PlayClipAtPoint(m_hitSound, this.transform.position);
    }

    int GetCurrentTime()       //Gets the current total time for this recording
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
