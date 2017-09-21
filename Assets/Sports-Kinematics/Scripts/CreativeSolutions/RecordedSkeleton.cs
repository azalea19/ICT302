using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

namespace SportsKinematics
{
    public class RecordedSkeleton : MonoBehaviour
    {
        public GameObject m_simManager;

        public void SetData(SortedList<float, SkeletonData> newData)
        {

        }

        private float GetCurrentTime(float currentTime)
        {
            return m_simManager.GetComponent<SimulationManager>().m_time;
        }
    }
}
