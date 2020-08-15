using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

namespace AMG
{
    public class VRMModelController : MonoBehaviour
    {
		public string ConnectionUUID = "/";
		//public string ConnectionMessage = "";
		public JObject ConnectionResult = null;
		public GameObject ConnectionLost;
		public SettingPanelController SettingPanelController = null;

		private Vector3 ModelOffset;
		private Vector3 ConnectionLostOffset;

		private Dictionary<string, string> Parameters = new Dictionary<string, string>();
		public Dictionary<string, ParametersClass> InitedParameters = new Dictionary<string, ParametersClass>();
		public ArrayList AInitedParameters = new ArrayList();

		public string DisplayName;
		public string ModelPath;
		public ArrayList animationClips;
		public Animation Animation;

		public bool LostReset = false;
		public bool LostResetEye = false;
		public int LostResetAction = 0;
		//0无 1动作 2PNG队列
		public string LostResetMotion = "/";
		public bool LostResetMotionLoop = false;
		//判断
		public int LostResetValue = 0;
		public float LostResetLastZ = 0;
		public bool LostResetFlag = false;

		//版本判断
		public bool IsNewSDK = false;
		public bool isTracked = false;

		public GameObject MouseObject;

		void Start()
		{
		}

		public void Update()
		{
			try
			{
				if (SettingPanelController != null)
				{
					if (SettingPanelController.GetModelSelected() == name)
					{
						ProcessPosition();
					}
				}
				//处理存入的数据
				if (ConnectionUUID != "/")
				{
					if (Globle.WSClients.ContainsKey(ConnectionUUID))
					{
						ConnectionResult = Globle.WSClients[ConnectionUUID].result;
						if (ConnectionResult != null)
						{
							DoJsonPrase(ConnectionResult);
						}
					}
				}

			}
			catch (Exception err)
			{
				Globle.DataLog = Globle.DataLog + "模型发生错误 " + err.Message + " : " + err.StackTrace;
			}
		}

		public void ProcessPosition()
		{
			if (Input.GetKey(KeyCode.LeftAlt))
			{
				if (Input.GetMouseButtonDown(0))
				{
					OnMouseDown();
				}
				if (Input.GetMouseButton(0))
				{
					OnMouseDrag();
				}
				if (Input.GetKey(KeyCode.LeftArrow))
				{
					transform.Rotate(0, -0.5f, 0, Space.Self);
				}
				if (Input.GetKey(KeyCode.RightArrow))
				{
					transform.Rotate(0, +0.5f, 0, Space.Self);
				}
				if (Input.GetKey(KeyCode.UpArrow))
				{
					transform.Rotate(-0.5f, 0, 0, Space.Self);
				}
				if (Input.GetKey(KeyCode.DownArrow))
				{
					transform.Rotate(+0.5f, 0, 0, Space.Self);
				}
				var Scale = Input.GetAxis("Mouse ScrollWheel") * 1f;
				this.GetComponent<VRMMeta>().gameObject.transform.localPosition += new Vector3(0, 0, Scale);
			}
			/*if (Input.GetKey(KeyCode.LeftControl))
			{
				if (Input.GetMouseButtonDown(0))
				{
					OnMouseDownConnectionLost();
				}
				if (Input.GetMouseButton(0))
				{
					OnMouseDragConnectionLost();
				}
				var Scale = Input.GetAxis("Mouse ScrollWheel") * 0.005f;
				ConnectionLost.gameObject.transform.localScale += new Vector3(Scale, Scale);
			}*/
		}

		void OnMouseDown()
		{
			ModelOffset = Camera.main.WorldToScreenPoint(transform.position) - Input.mousePosition;
		}

		void OnMouseDrag()
		{
			var cc = Camera.main.ScreenToWorldPoint(Input.mousePosition + ModelOffset);
			transform.position = new Vector3(cc.x, cc.y, transform.position.z);
		}

		public void ResetModel()
		{
			/*foreach (KeyValuePair<string, ParametersClass> kvp in InitedParameters)
			{
				if (LostResetEye == true && (kvp.Value.Name == "paramEyeLOpen" || kvp.Value.Name == "paramEyeROpen"))
				{
					var para = (CubismParameter)kvp.Value.Parameter;
					para.Value = 1;
				}
				else if (kvp.Value.Parameter != null && kvp.Value.Name != "paramBreath")
				{
					var para = (CubismParameter)kvp.Value.Parameter;
					para.Value = 0;
				}
			}*/
		}

		public void InitBreath()
		{

		}

		public void DoJsonPrase(JObject jsonResult)
		{
			try
			{

				//self.parameter.headPitch = @(-(180 / M_PI) * self.faceNode.eulerAngles.x * 1.3);
				//self.parameter.headYaw = @((180 / M_PI) * self.faceNode.eulerAngles.y);
				//self.parameter.headRoll = @(-(180 / M_PI) * self.faceNode.eulerAngles.z + 90.0);
				/*float angle = 0.0f;
				Vector3 axis = Vector3.zero;
				var rot = UnityARMatrixOps.GetRotation();
				rot.ToAngleAxis(out angle, out axis);
				axis.x = -axis.x;
				axis.z = -axis.z;
				head.localRotation = Quaternion.AngleAxis(angle, axis);*/
				/*var xx = Convert.ToDouble(float.Parse(jsonResult["headPitch"].ToString()) / 1.3 / - (180 / 3.14));
				var yy = Convert.ToDouble(float.Parse(jsonResult["headYaw"].ToString()) / (180 / 3.14));
				var zz = Convert.ToDouble((float.Parse(jsonResult["headRoll"].ToString()) - 90) /  - (180 / 3.14));

				Vector3 rotationVector = new Vector3((float)xx * 10, -(float)yy * 10, (float)zz);
				Quaternion rotation = Quaternion.Euler(rotationVector);


				var aaa = new Quaternion((float)xx, (float)yy, 0, 0);
				float angle = 0.0f;
				Vector3 axis = Vector3.zero;
				aaa.ToAngleAxis(out angle, out axis);
				axis.x = -axis.x;
				axis.y = -axis.y;
				angle = -angle;
				GetComponent<VRMLookAtHead>().Head.localRotation = rotation;*/
				//float.Parse(jsonResult["transforms"]["face.eulerAngles"][0].ToString()) * 20F
				//var aul = new Vector3(float.Parse(jsonResult["transforms"]["face.eulerAngles"][0].ToString()) * 20F, 0f , 0f);
				//var aul = new Vector3(0f, - float.Parse(jsonResult["transforms"]["face.eulerAngles"][1].ToString()) * 20F, 0f);
				//var aul = new Vector3(0f, 0f, - float.Parse(jsonResult["transforms"]["face.eulerAngles"][2].ToString()) * 20F);
				var aul = new Vector3(float.Parse(jsonResult["transforms"]["face.eulerAngles"][0].ToString()) * 20F, - float.Parse(jsonResult["transforms"]["face.eulerAngles"][1].ToString()) * 20F, - float.Parse(jsonResult["transforms"]["face.eulerAngles"][2].ToString()) * 20F);
				GetComponent<VRMLookAtHead>().Head.localEulerAngles = aul;

				if (jsonResult.ContainsKey("blendShapes"))
				{
					var proxy = GetComponent<VRMBlendShapeProxy>();
					proxy.ImmediatelySetValue(BlendShapePreset.A, float.Parse(jsonResult["blendShapes"]["jawOpen"].ToString()) - float.Parse(jsonResult["blendShapes"]["mouthClose"].ToString()));
					proxy.ImmediatelySetValue(BlendShapePreset.U, float.Parse(jsonResult["blendShapes"]["mouthFunnel"].ToString()) + float.Parse(jsonResult["blendShapes"]["mouthPucker"].ToString()) / 1.8F);
					proxy.ImmediatelySetValue(BlendShapePreset.I, float.Parse(jsonResult["blendShapes"]["mouthStretch_R"].ToString()) + float.Parse(jsonResult["blendShapes"]["mouthStretch_L"].ToString()) / 1.8F);

					proxy.ImmediatelySetValue(BlendShapePreset.Blink_L, float.Parse(jsonResult["blendShapes"]["eyeBlink_R"].ToString()) * 1.2F);
					proxy.ImmediatelySetValue(BlendShapePreset.Blink_R, float.Parse(jsonResult["blendShapes"]["eyeBlink_L"].ToString()) * 1.2F);
				}
			}
			catch (Exception err){
				Debug.Log(err.Message + err.StackTrace);

			}
		}

		public void GetParameters()
		{
			/*var jsonDataPath = Application.streamingAssetsPath + "/Parameters.json";
			JObject jsonParams = Live2DParametersController.getParametersJson(jsonDataPath);
			var model = GetComponent<CubismModel>();
			foreach (KeyValuePair<string, string> kvp in Parameters)
			{
				var paraC = new ParametersClass();
				paraC.Name = kvp.Key;
				var para = Live2DParametersController.getParametersFromJson(kvp.Key, jsonParams, model);
				paraC.Parameter = para;
				if (para != null)
				{
					AInitedParameters.Add(para);
					paraC.MinValue = para.MinimumValue;
					paraC.MinSetValue = para.MinimumValue;
					paraC.MaxValue = para.MaximumValue;
					paraC.MaxSetValue = para.MaximumValue;
				}
				paraC.SDKName = kvp.Value;
				InitedParameters.Add(kvp.Key, paraC);
			}*/
		}


		public Dictionary<string, string> GetModelSettings()
		{
			var returnDict = new Dictionary<string, string>();
			/*foreach (KeyValuePair<string, ParametersClass> kvp in InitedParameters)
			{
				if (kvp.Value.Parameter != null && kvp.Value.Name != "paramBreath")
				{
					returnDict.Add(kvp.Value.Name, kvp.Value.MinSetValue.ToString() + "|" + kvp.Value.MaxSetValue.ToString());
				}
			}*/
			return returnDict;
		}

		public void SetModelSettings(Dictionary<string, string> userDict)
		{
			foreach (KeyValuePair<string, ParametersClass> kvp in InitedParameters)
			{
				if (userDict.ContainsKey(kvp.Value.Name) && kvp.Value.Name != "paramBreath")
				{
					var text = userDict[kvp.Value.Name];
					var splitA = text.Split('|');
					kvp.Value.MinSetValue = float.Parse(splitA[0]);
					kvp.Value.MaxSetValue = float.Parse(splitA[1]);
				}
			}
		}

		public Dictionary<string, string> GetModelOtherSettings()
		{
			var returnDict = new Dictionary<string, string>();
			returnDict.Add("LostReset", LostReset.ToString());
			returnDict.Add("LostResetEye", LostResetEye.ToString());
			returnDict.Add("LostResetAction", LostResetAction.ToString());
			returnDict.Add("LostResetMotion", LostResetMotion);
			returnDict.Add("LostResetMotionLoop", LostResetMotionLoop.ToString());
			return returnDict;
		}

		public void SetModelOtherSettings(Dictionary<string, string> otherDict)
		{
			if (CheckNameInDict("LostReset", otherDict))
			{
				LostReset = bool.Parse(otherDict["LostReset"]);
			}
			if (CheckNameInDict("LostResetEye", otherDict))
			{
				LostResetEye = bool.Parse(otherDict["LostResetEye"]);
			}
			if (CheckNameInDict("LostResetAction", otherDict))
			{
				LostResetAction = int.Parse(otherDict["LostResetAction"]);
			}
			if (CheckNameInDict("LostResetMotion", otherDict))
			{
				LostResetMotion = otherDict["LostResetMotion"];
			}
			if (CheckNameInDict("LostResetMotionLoop", otherDict))
			{
				LostResetMotionLoop = bool.Parse(otherDict["LostResetMotionLoop"]);
			}
		}

		public Dictionary<string, string> GetModelLocationSettings()
		{
			var returnDict = new Dictionary<string, string>();
			returnDict.Add("transformXValue", transform.position.x.ToString());
			returnDict.Add("transformYValue", transform.position.y.ToString());
			returnDict.Add("transformZValue", transform.position.z.ToString());
			returnDict.Add("transformRXValue", transform.localEulerAngles.x.ToString());
			returnDict.Add("transformRYValue", transform.localEulerAngles.y.ToString());
			returnDict.Add("transformRZValue", transform.localEulerAngles.z.ToString());
			//returnDict.Add("ctransformXValue", ConnectionLost.transform.position.x.ToString());
			//returnDict.Add("ctransformYValue", ConnectionLost.transform.position.y.ToString());
			//returnDict.Add("ctransformSValue", ConnectionLost.transform.localScale.x.ToString());
			return returnDict;
		}

		public void SetModelLocationSettings(Dictionary<string, string> locationInfo)
		{
			if (CheckNameInDict("transformXValue", locationInfo) && CheckNameInDict("transformYValue", locationInfo) && CheckNameInDict("transformZValue", locationInfo))
			{
				transform.position = new Vector3(Convert.ToSingle(locationInfo["transformXValue"]), Convert.ToSingle(locationInfo["transformYValue"]), Convert.ToSingle(locationInfo["transformZValue"]));
			}
			if (CheckNameInDict("transformRXValue", locationInfo) && CheckNameInDict("transformRYValue", locationInfo) && CheckNameInDict("transformRZValue", locationInfo))
			{
				transform.localEulerAngles = new Vector3(Convert.ToSingle(locationInfo["transformRXValue"]), Convert.ToSingle(locationInfo["transformRYValue"]), Convert.ToSingle(locationInfo["transformRZValue"]));
			}
			/*if (CheckNameInDict("ctransformXValue", locationInfo) && CheckNameInDict("ctransformYValue", locationInfo))
			{
				ConnectionLost.transform.position = new Vector3(Convert.ToSingle(locationInfo["ctransformXValue"]), Convert.ToSingle(locationInfo["ctransformYValue"]), ConnectionLost.transform.position.z);
			}
			if (CheckNameInDict("ctransformSValue", locationInfo))
			{
				ConnectionLost.transform.localScale = new Vector3(Convert.ToSingle(locationInfo["ctransformSValue"]), Convert.ToSingle(locationInfo["ctransformSValue"]));
			}*/
		}

		public bool CheckNameInDict(string name, Dictionary<string, string> otherDict)
		{
			return otherDict.ContainsKey(name);
		}
	}

}