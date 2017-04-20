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

	#if UNITY_ANDROID
		private string _deviceID = "E4:72:2B:5E:4E:A3";
	#else
		private string _deviceID = "9480DDC6-F94E-4B0D-837C-F48C49AC8C39";
	#endif

	private float[] floatData;
	private Vector3 rotate_data;

	bool _connected = false;
	bool _scanning = false;
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
		Debug.Log ("Initialize\r\n");


	}
	
	void disconnect (Action<string> action)
	{
		BluetoothLEHardwareInterface.DisconnectPeripheral (Address.text, action);
	}

	public void OnSend ()
	{
		Debug.Log ("OnSend\r\n");
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
		Debug.Log ("OnConnect\r\n");
		if (!_connecting)
		{
			Debug.Log ("_connecting\r\n");
			if (Connected)
			{
				Debug.Log ("Already connected\r\n");
				disconnect ((Address) => {
					Connected = false;
				});
			}
			else
			{
				Debug.Log ("attempt to connect\r\n");
				_readFound = false;
				_writeFound = false;

				BluetoothLEHardwareInterface.ConnectToPeripheral (_deviceID, (address) => {
				},
				(address, serviceUUID) => {
				},
				(address, serviceUUID, characteristicUUID) => {
						Debug.Log ("serviceUUID" + serviceUUID + "\r\n");
					// discovered characteristic
					if (IsEqual(serviceUUID, _serviceUUID))
					{
							
						_connectedID = address;
						
						Connected = true;
						
						if (IsEqual (characteristicUUID, _readCharacteristicUUID))
						{
							_readFound = true;
								Debug.Log ("_readFound\r\n");
						}
						else if (IsEqual (characteristicUUID, _writeCharacteristicUUID))
						{
							_writeFound = true;
								Debug.Log ("_writeFound\r\n");
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
		Debug.Log ("Start\r\n");
		Connected = false;

		BluetoothLEHardwareInterface.Initialize (true, false, () => {
			OnScan ();
		}, (error) => {
			
		});

		//OnConnect ();
	}
	public void OnScan ()
	{

		if (_scanning) {
			BluetoothLEHardwareInterface.StopScan ();

			_scanning = false;
		} 
		else 
		{
			BluetoothLEHardwareInterface.ScanForPeripheralsWithServices (null, (address, name) => {

				//AddPeripheral (name, address);

				//Debug.Log(string.Format ("Device: {0} address: {1} ", name, address));

				if (name.Contains("Seed")) {
					_scanning = true;
					Debug.Log("PairKey Found");
					OnConnect();
				}

			}, (address, name, rssi, advertisingInfo) => {

				//if (advertisingInfo != null)
				//Debug.Log (string.Format ("Device: {0} RSSI: {1} Data Length: {2} ", name, rssi, advertisingInfo.Length));
			});

		}
			
			


	}
	// Update is called once per frame
	void Update ()
	{
		//Debug.Log ("Update()\r\n");
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
						//Debug.Log (string.Format ("data length: {0}", data.Length));
						if (data.Length == 0)
						{
						}
						else
						{
							//string s = ASCIIEncoding.UTF8.GetString (data);
							//Debug.Log (s);


							var acc_x = (short)(data[0] << 8 | data[1]);
							var acc_y = (short)(data[2] << 8 | data[3]);
							var acc_z = (short)(data[4] << 8 | data[5]);
							Debug.Log (string.Format ("acc_x: {0} ", acc_x));
							Debug.Log (string.Format ("acc_y: {0} ", acc_y));
							Debug.Log (string.Format ("acc_z: {0} ", acc_z));

							double pitch = Math.Atan(acc_x/Math.Sqrt(Mathf.Pow(acc_y,2) + Mathf.Pow(acc_z,2)));
							double roll = Math.Atan(acc_y/Math.Sqrt(Mathf.Pow(acc_x,2) + Mathf.Pow(acc_z,2)));


							rotate_data.x = (float)pitch * 180 / Mathf.PI;
							rotate_data.y = (float)roll * 180 / Mathf.PI;
							rotate_data.z = 0.0F;

							Debug.Log (string.Format ("acc_x: {0} ", rotate_data.x));
							Debug.Log (string.Format ("acc_y: {0} ", rotate_data.y));

							Quaternion target = Quaternion.Euler (rotate_data.x, 0.0F, rotate_data.y);
							transform.rotation = target;
						}
					}
					
				});
				
				//SendButton.SetActive (true);
			}
		}
	}
	protected string BytesToString (byte[] bytes)
	{
		string result = "";

		foreach (var b in bytes)
			result += b.ToString ("X2");

		return result;
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
