using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SportsKinematics
{
    public class PlayerStriker : Striker
    {
        SortedList<float, StrikerData> playerStrikerData;

        // Update is called once per frame
        void Update()
        {
            if (GetTime() > 0)
            {
                StrikerData tmp = new StrikerData();
                tmp.position = this.transform.position;
                tmp.rotation = this.transform.rotation.eulerAngles;

                playerStrikerData.Add(GetTime(), tmp);
            }
        }

        void ClearData()
        {
            playerStrikerData.Clear();
        }

        float GetTime()
        {
            return m_simManager.GetComponent<SimulationManager>().m_time;
        }

        void SetOccluded(bool newOcclusion)
        {
            this.GetComponent<Renderer>().enabled = newOcclusion;
        }
    }
}