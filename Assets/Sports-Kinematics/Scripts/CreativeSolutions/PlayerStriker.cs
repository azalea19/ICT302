using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SportsKinematics
{
    public class PlayerStriker : Striker
    {
        SortedList<float, StrikerData> m_playerStrikerData = new SortedList<float, StrikerData>();

        // Update is called once per frame
        void Update()
        {
            if (GetTime() > 0 && !m_playerStrikerData.ContainsKey(GetTime()))
            {
                StrikerData tmp = new StrikerData();
                tmp.position = this.transform.position;
                tmp.rotation = this.transform.rotation.eulerAngles;

                m_playerStrikerData.Add(GetTime(), tmp);
            }
        }

        public void ClearData()
        {
            m_playerStrikerData = new SortedList<float, StrikerData>();
        }

        public SortedList<float, StrikerData> GetData()
        {
            return m_playerStrikerData;
        }

        float GetTime()
        {
            return 0;//m_simManager.GetComponent<SimulationManager>().m_time;
        }

        public void SetOccluded(bool newOcclusion)
        {
            if(m_currentStriker != null)
                m_currentStriker.GetComponent<Renderer>().enabled = newOcclusion;
        }
    }
}