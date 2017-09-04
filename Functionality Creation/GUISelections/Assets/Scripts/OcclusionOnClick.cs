using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OcclusionOnClick : MonoBehaviour {

	//public Button m_btn;
	public Canvas m_friendCanvas;
	public Canvas m_rivalCanvas;

	// Use this for initialization
	/*public void Start ()
	{
		Button btn = m_btn.GetComponent<Button>();
		btn.onClick.AddListener (TaskOnClick);
	}*/

	public void TaskOnClick ()
	{
		bool active = m_friendCanvas.gameObject.activeInHierarchy;

		m_friendCanvas.gameObject.SetActive(!active);

		m_rivalCanvas.gameObject.SetActive (false);
	}
}
