using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Debug_Rotate : MonoBehaviour
{
	private string message;
	private float[] floatData;
	private Vector3 rotate_data;


	// Initialization
	void Start()
	{
		//mySerial = GetComponent<Serial> ();
	}

	// Executed each frame
	void Update()
	{
		//string message = mySerial.ReceivedBytes;

		//string[] str_data = message.Split (new string[] {",","\r\n"},StringSplitOptions.None);
		//print (str_data [0]);

		//floatData[0] = float.Parse(str_data[0]);
		//floatData[1] = float.Parse(str_data[1]);

		//float[] floatData = Array.ConvertAll (message.Split (','), float.Parse);

		//rotate_data.x = floatData [0] * 180 / Mathf.PI;
		//rotate_data.y = floatData [1] * 180 / Mathf.PI;
		//rotate_data.z = 0.0F;

		//Quaternion target = Quaternion.Euler (rotate_data.x, 0.0F, rotate_data.y);

		//transform.rotation = target;


		//OnSerialLine(floatData[1].ToString());
	}
}