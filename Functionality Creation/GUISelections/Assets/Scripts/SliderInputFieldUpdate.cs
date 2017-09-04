using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderInputFieldUpdate : MonoBehaviour {

	public Slider m_slider;
	public InputField m_input;
	public byte decimals = 1;

	// Use this for initialization
	void Start() 
	{
		m_slider.onValueChanged.AddListener(ChangeValue);
		ChangeValue(m_slider.value);
	}

	void ChangeValue(float value)
	{
		m_input.text = value.ToString("n"+decimals);
	}


}