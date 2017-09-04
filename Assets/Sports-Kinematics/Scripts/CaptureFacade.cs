using UnityEngine;


/// <summary>
/// Facade used to contain all KinectWrappers
/// If input device is changed, this script as well as
/// the new devices api will have to be editted and integrated.
/// All SportsKinematic scripts that require the kinect
/// should make reference to this script, as to localize changes
/// throughout the project and devices' api.
/// </summary>
/// <author>Justin Johnston</author>
/// <date>25/04/2017</date>
///
namespace SportsKinematics
{
    public class CaptureFacade : MonoBehaviour  //FR1 - Virtual opponent modelling from captured data 
                                                //& FR2 - Motion Capture of sport professionals and users & NFR1 - Scalability.
    {
        /// <summary>
        /// Body view GameObject reference
        /// </summary>
        public GameObject bodyView;

        /// <summary>
        /// colour view GameObject reference
        /// </summary>
        public GameObject colourView;

        /// <summary>
        /// Infrared View GameObject reference
        /// </summary>
        public GameObject infraredView;

        /// <summary>
        /// DepthView GameObject refeence
        /// </summary>
        public GameObject depthView;
        
        /// <summary>
        /// MultiFrame GameObject Reference
        /// </summary>
        public GameObject multiFrame;

        /// <summary>
        /// Script reference for BodySourceView Script
        /// </summary>
        private BodySourceView m_bodyView;

        /// <summary>
        /// Script reference for ColourSourceView Script
        /// </summary>
        private ColorSourceView m_colourView;

        /// <summary>
        /// Script reference for InfraredSourceView Script
        /// </summary>
        private InfraredSourceView m_infraredView;

        /// <summary>
        /// Script reference for DepthSourceView Script
        /// </summary>
        private DepthSourceView m_depthView;

        /// <summary>
        /// Script reference for MultiSourceManager Script
        /// </summary>
        private MultiSourceManager m_multiFrame;

        /// <summary>
        /// Accessor method for bodySourceView Script. READ-ONLY
        /// </summary>
        public BodySourceView BodyView
        {
            get { return m_bodyView; }
        }

        /// <summary>
        /// Accessor method for ColourSourceView Script. READ-ONLY
        /// </summary>
        public ColorSourceView ColourView
        {
            get { return m_colourView; }
        }

        /// <summary>
        /// Accessor method for InfraredSourceView Script. READ-ONLY
        /// </summary>
        public InfraredSourceView InfraredView
        {
            get { return m_infraredView; }
        }

        /// <summary>
        /// Accessor method for DepthSourceView Script. READ-ONLY
        /// </summary>
        public DepthSourceView DepthView
        {
            get { return m_depthView; }
        }

        /// <summary>
        /// Accessor method for MultiSourceManager Script. READ-ONLY
        /// </summary>
        public MultiSourceManager MultiManager
        {
            get { return m_multiFrame; }
        }

        /// <summary>
        /// Awake Method for GameObject
        /// </summary>
        void Awake()
        {
            m_bodyView = bodyView.GetComponent<BodySourceView>();
            m_colourView = colourView.GetComponent<ColorSourceView>();
            m_infraredView = infraredView.GetComponent<InfraredSourceView>();
            m_depthView = depthView.GetComponent<DepthSourceView>();
            m_multiFrame = multiFrame.GetComponent<MultiSourceManager>();
        }
    }

}