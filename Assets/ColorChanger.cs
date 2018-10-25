using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour {
	Material m_Material;

	void Start()
	{
			//Fetch the Material from the Renderer of the GameObject
			m_Material = GetComponent<Renderer>().material;
			print("Materials " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);
	}

	void OnCollisionEnter(Collision collision)
	{
		m_Material.color = Color.white;
	}

	void OnCollisionExit(Collision collision)
	{
		m_Material.color = Color.red;
	}

	void Update()
	{
			if (Input.GetKeyDown(KeyCode.A))
			{
					m_Material.color = Color.green;
			}
	}
}
