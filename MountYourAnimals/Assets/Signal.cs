using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour
{
	/// <summary>
	/// Sent when another object enters a trigger collider attached to this
	/// object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other)
	{
		//print("TriggerEnter");
		if(other.tag == "Phone")
		{
            other.GetComponentInParent<Phone>().AddSignal();
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		//print("TriggerExit");
		if(other.tag == "Phone")
		{
            other.GetComponentInParent<Phone>().SubtractSignal();
		}
	}
}
