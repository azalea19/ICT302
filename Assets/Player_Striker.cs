using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StrikerData
{
    public Vector3 position;
    public Quaternion rotation;
}

public class Player_Striker : MonoBehaviour {

    SortedDictionary<float, StrikerData> playerStrikerData;

    public GameObject m_tableTennisBat;
    public GameObject m_catch;

    public enum StrikerSelection
    {
        //catch renderes no striker object
        Catch = 0,
        //renders a ping pong paddle
        TableTennis = 1,
        /*ADD MORE AS NEEDED*/
    }//selection method or the striking object to be drawn. Adds scalability to other sports.
    public StrikerSelection m_StrikerSelection;

    void Start()
    {
        switch (m_StrikerSelection)
        {
            case StrikerSelection.TableTennis:
                Object.Instantiate(m_tableTennisBat, this.transform);
                break;
            case StrikerSelection.Catch:
                Object.Instantiate(m_catch, this.transform);
                break;
        }

    }

    // Update is called once per frame
    void Update () {
		if(GetTime() > 0)
        {
            StrikerData tmp = new StrikerData();
            tmp.position = this.transform.position;
            tmp.rotation = this.transform.rotation;

            playerStrikerData.Add(GetTime(), tmp);
        }
	}

    void ClearData()
    {
        playerStrikerData.Clear();
    }

    float GetTime()
    {
        return 0;
    }

    void SetOccluded(bool newOcclusion)
    {
        this.GetComponent<Renderer>().enabled = newOcclusion;
    }
}
