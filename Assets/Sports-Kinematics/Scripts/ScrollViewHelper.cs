using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Provides additional functionality to the ScrollView.
    /// NOT CURRENTLY USED
    /// </summary>
    public class ScrollViewHelper : MonoBehaviour
    {
        /// <summary>
        /// Used for unrestricted scrollviews. When attempting to scroll past
        /// the scrollview limits, the scroll sensitivity is set to 0.
        /// </summary>
        /// <param name="e">Scroll event.</param>
        public void TaskOnScroll(BaseEventData e)
        {
            float val = Input.GetAxis("Mouse ScrollWheel");

            ScrollRect scrollrect = GameObject.Find("Simulation/PlaylistBuilderCanvas/ScrollView").GetComponent<ScrollRect>();
            Scrollbar scrollbar = GameObject.Find("Simulation/PlaylistBuilderCanvas/ScrollView/ScrollbarVertical").GetComponent<Scrollbar>();

            float value = scrollbar.value;

            if (value + val > 1.0f)
            {
                scrollrect.scrollSensitivity = 0.0f;
            }
            else
                if (value - val < 0.0f)
                {
                    scrollrect.scrollSensitivity = 0.0f;
                }

            StartCoroutine(wait(scrollrect));
        }

        /// <summary>
        /// Causes a 2 second wait, after which the ScrollView sensitivity is set back to 10.
        /// </summary>
        /// <param name="scrollrect">Scroll rect to change the sensitivity of.</param>
        /// <returns></returns>
        private IEnumerator wait(ScrollRect scrollrect)
        {
            yield return new WaitForSeconds(2f);
            scrollrect.scrollSensitivity = 10.0f;
        }
    }
}