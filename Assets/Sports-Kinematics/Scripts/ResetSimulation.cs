using UnityEngine;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Resets the simulation scene
    /// </summary>
    public class ResetSimulation : MonoBehaviour
    {
        //GameObject reference of action renderer to reset values, set in Unity.
        public GameObject actionRenderer;
        //GameObject reference of the ball to reset ball values, set in Unity.
        public GameObject ball;

        //Action renderer reference to reset values
        private ActionRenderer m_actionRenderer;
        //Ball reference to reset ball values
        private BallScript m_ball;

        /// <summary>
        /// Sets ball and action render values from public GameObjects.
        /// </summary>
        void Start()
        {
            m_actionRenderer = actionRenderer.GetComponent<ActionRenderer>();
            if(ball)
                m_ball = ball.GetComponent<BallScript>();
        }

        /// <summary>
        /// Callback used to reset simulation when 'Reset' button is clicked.
        /// </summary>
        public void TaskOnClick()
        {
            if (ball)
                m_ball.ResetBall();
            m_actionRenderer.ResetRenderFrame();
        }
    }
}
