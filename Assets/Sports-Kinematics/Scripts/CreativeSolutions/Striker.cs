using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SportsKinematics
{
    public class Striker : MonoBehaviour
    {

        public GameObject m_tableTennisBat;
        public GameObject m_catch;
        public GameObject m_simManager;

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
    }
}
