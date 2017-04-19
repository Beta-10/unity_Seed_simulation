using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;

public class CentralNordicScript : MonoBehaviour
{
	public Transform PanelCentral;
	//public Text Name;
	public Text Address;
	//public Text Receive;
	//public InputField Send;
	//public Text TextConnectButton;
	//public GameObject SendButton;

	private bool _connecting = false;
	private string _connectedID = null;
	private string _serviceUUID = "0001";
	private string _readCharacteristicUUID = "0003";
	private string _writeCharacteristicUUID = "0002";
	private float _subscribingTimeout = 0f;
	private bool _readFound = false;
	private bool _writeFound = false;
	private string device_address = "E4:72:2B:5E:4E:A3";

	bool _connected = false;
	bool Connected
	{
		get { return _connected; }
		set
		{
			_connected = value;
			
			if (_connected)
			{
				Debug.Log("Disconnect");
				_connecting = false;
			}
			else
			{
				Debug.Log("Connect");
				_connectedID = null;
				//SendButton.SetActive (false);
			}
		}
	}
	
	public void Initialize ()
	{
		Connected = false;
	}
	
	void disconnect (Action<string> action)
	{
		BluetoothLEHardwareInterface.DisconnectPeripheral (Address.text, action);
	}

	public void OnSend ()
	{
		///if (Send.text.Length > 0)
		//{
			//byte[] bytes = ASCIIEncoding.UTF8.GetBytes (Send.text);
			//if (bytes.Length > 0)
				//SendBytes (bytes);

			//Send.text = "";
		//}
	}

	public void OnBack ()
	{
		if (Connected)
		{
			disconnect ((Address) => {
				
				Connected = false;
				//BLETestScript.Show (PanelCentral.transform);
			});
		}
		//else
			//BLETestScript.Show (PanelCentral.transform);
	}
	
	public void OnConnect ()
	{
		if (!_connecting)
		{
			if (Connected)
			{
				disconnect ((Address) => {
					Connected = false;
				});
			}
			else
			{
				_readFound = false;
				_writeFound = false;

				BluetoothLEHardwareInterface.ConnectToPeripheral (device_address, (address) => {
				},
				(address, serviceUUID) => {
				},
				(address, serviceUUID, characteristicUUID) => {
					
					// discovered characteristic
					if (IsEqual(serviceUUID, _serviceUUID))
					{
						_connectedID = address;
						
						Connected = true;

						if (IsEqual (characteristicUUID, _readCharacteristicUUID))
						{
							_readFound = true;
						}
						else if (IsEqual (characteristicUUID, _writeCharacteristicUUID))
						{
							_writeFound = true;
						}
					}
				}, (address) => {
					
					// this will get called when the device disconnects
					// be aware that this will also get called when the disconnect
					// is called above. both methods get call for the same action
					// this is for backwards compatibility
					Connected = false;
				});
				
				_connecting = true;
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		Debug.Log ("NRF Devices connected\r\n");
		OnConnect ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_readFound && _writeFound)
		{
			_readFound = false;
			_writeFound = false;

			_subscribingTimeout = 1f;
		}

		if (_subscribingTimeout > 0f)
		{
			_subscribingTimeout -= Time.deltaTime;
			if (_subscribingTimeout <= 0f)
			{
				_subscribingTimeout = 0f;

				BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_connectedID, FullUUID (_serviceUUID), FullUUID (_readCharacteristicUUID), (deviceAddress, notification) => {
					
				}, (deviceAddress2, characteristic, data) => {

					BluetoothLEHardwareInterface.Log ("id: " + _connectedID);
					if (deviceAddress2.CompareTo (_connectedID) == 0)
					{
						Debug.Log (string.Format ("data length: {0}", data.Length));
						if (data.Length == 0)
						{
						}
						else
						{
							string s = ASCIIEncoding.UTF8.GetString (data);
							Debug.Log (s);
							//Receive.text += s;
						}
					}
					
				});
				
				//SendButton.SetActive (true);
			}
		}
	}
	
	string FullUUID (string uuid)
	{
		return "6E40" + uuid + "-B5A3-F393-E0A9-E50E24DCCA9E";
	}
	
	bool IsEqual(string uuid1, string uuid2)
	{
		if (uuid1.Length == 4)
			uuid1 = FullUUID (uuid1);
		if (uuid2.Length == 4)
			uuid2 = FullUUID (uuid2);

		return (uuid1.ToUpper().CompareTo(uuid2.ToUpper()) == 0);
	}
	
	void SendByte (byte value)
	{
		byte[] data = new byte[] { value };
		BluetoothLEHardwareInterface.WriteCharacteristic (_connectedID, FullUUID (_serviceUUID), FullUUID (_writeCharacteristicUUID), data, data.Length, true, (characteristicUUID) => {
			
			BluetoothLEHardwareInterface.Log ("Write Succeeded");
		});
	}
	
	void SendBytes (byte[] data)
	{
		BluetoothLEHardwareInterface.Log (string.Format ("data length: {0} uuid: {1}", data.Length.ToString (), FullUUID (_writeCharacteristicUUID)));
		BluetoothLEHardwareInterface.WriteCharacteristic (_connectedID, FullUUID (_serviceUUID), FullUUID (_writeCharacteristicUUID), data, data.Length, true, (characteristicUUID) => {

			BluetoothLEHardwareInterface.Log ("Write Succeeded");
		});
	}
}
