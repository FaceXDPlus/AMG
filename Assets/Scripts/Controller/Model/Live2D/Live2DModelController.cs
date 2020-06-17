using Live2D.Cubism.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace AMG
{
    public class Live2DModelController : MonoBehaviour
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
		public int LostResetAction = 0;
		//0无 1动作 2PNG队列
		public string LostResetMotion = "/";
		//判断
		public int LostResetValue = 0;
		public float LostResetLastZ = 0;
		public bool LostResetFlag = false;

		//版本判断
		public bool IsNewSDK = false;
		public bool isTracked = false;


		void Start()
        {
			InitParameters();
			GetParameters();
			InitBreath();
		}

		public void Update()
		{
			try
			{
				if (SettingPanelController != null)
				{
					if (SettingPanelController.GetModelSelected() == this.GetComponent<CubismModel>().name)
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
				//更新模型位置
				ProcessModelParameter();
				
			}
			catch (Exception err)
			{
				Globle.DataLog = Globle.DataLog + "模型发生错误 " + err.Message + " : " + err.StackTrace;
			}
		}

		public void FixedUpdate()
		{
			if (IsNewSDK)
			{
				if (isTracked != true && LostResetFlag == false)
				{
					LostResetFlag = true;
					//ResetModel();
					SetLostReset(true);
				}else if (isTracked == true && LostResetFlag == true)
				{
					LostResetFlag = false;
					SetLostReset(false);
				}
			}
			else 
			{
				var aa = (CubismParameter)InitedParameters["paramAngleZ"].Parameter;
				if (LostResetValue > 50)
				{
					if (LostResetFlag == false && aa.Value != 0)
					{
						LostResetFlag = true;
						ResetModel();
						SetLostReset(true);
					}
					else if (aa.Value != LostResetLastZ && LostResetFlag == true)
					{
						LostResetFlag = false;
						SetLostReset(false);
						LostResetLastZ = aa.Value;
						LostResetValue = 0;
					}
				}
				else
				{
					if (aa.Value == LostResetLastZ)
					{
						LostResetValue++;
					}
					else
					{
						LostResetLastZ = aa.Value;
						LostResetValue = 0;
					}
				}
			}
		}

		public void SetLostReset(bool isOn)
		{
			switch (LostResetAction)
			{
				case 0:
					if (isOn == true)
					{
						//无操作
					}
					else
					{
						ConnectionLost.SetActive(false);
					}
					break;
				case 1:
					if (isOn)
					{
						if (animationClips.Contains(LostResetMotion))
						{
							this.Animation.Blend(LostResetMotion);
						}
					}
					break;
				case 2:
					ConnectionLost.SetActive(isOn);
					break;
			}
		}

		public void InitParameters()
		{
			Parameters.Add("paramEyeLOpen", "eyeLOpen");
			Parameters.Add("paramEyeROpen", "eyeROpen");
			Parameters.Add("paramAngleX", "headYaw");
			Parameters.Add("paramAngleY", "headPitch");
			Parameters.Add("paramAngleZ", "headRoll");
			Parameters.Add("paramEyeBallX", "eyeX");
			Parameters.Add("paramEyeBallY", "eyeY");
			Parameters.Add("paramBrowLForm", "eyeBrowLForm");
			Parameters.Add("paramBrowRForm", "eyeBrowRForm");
			Parameters.Add("paramBrowAngleL", "eyeBrowAngleL");
			Parameters.Add("paramBrowAngleR", "eyeBrowAngleR");
			Parameters.Add("paramBrowLY", "eyeBrowYL");
			Parameters.Add("paramBrowRY", "eyeBrowYR");
			Parameters.Add("paramMouthOpenY", "mouthOpenY");
			Parameters.Add("paramMouthForm", "mouthForm");
			Parameters.Add("paramBreath", "Breath");
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
					transform.Rotate(0, 0, -0.5f, Space.Self);
				}
				if (Input.GetKey(KeyCode.RightArrow))
				{
					transform.Rotate(0, 0, +0.5f, Space.Self);
				}
				var Scale = Input.GetAxis("Mouse ScrollWheel") * 60f;
				this.GetComponent<CubismModel>().gameObject.transform.localScale += new Vector3(Scale, Scale);
			}
			if (Input.GetKey(KeyCode.LeftControl))
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
			}
		}

		public void ProcessModelParameter()
		{
			if (LostResetFlag == false)
			{
				foreach (KeyValuePair<string, ParametersClass> kvp in InitedParameters)
				{
					if (kvp.Value.Parameter != null && kvp.Value.Name != "paramBreath")
					{
						setParameter((CubismParameter)kvp.Value.Parameter, kvp.Value.NowValue, kvp.Value.MinValue, kvp.Value.MaxValue, kvp.Value.MinSetValue, kvp.Value.MaxSetValue);
					}
				}
			}
			else
			{
				if (LostReset == true)
				{
					ResetModel();
				}
			}
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

		void OnMouseDownConnectionLost()
		{
			ConnectionLostOffset = Camera.main.WorldToScreenPoint(ConnectionLost.transform.position) - Input.mousePosition;  
		}

		void OnMouseDragConnectionLost()
		{
			var cc = Camera.main.ScreenToWorldPoint(Input.mousePosition + ConnectionLostOffset);
			ConnectionLost.transform.position = new Vector3(cc.x, cc.y, ConnectionLost.transform.position.z);
		}

		public void ResetModel()
		{
			foreach (KeyValuePair<string, ParametersClass> kvp in InitedParameters)
			{
				if (kvp.Value.Parameter != null && kvp.Value.Name != "paramBreath")
				{
					var para = (CubismParameter)kvp.Value.Parameter;
					para.Value = 0;
				}
			}
		}

		public void InitBreath()
		{
			var breathController = this.gameObject.AddComponent<CubismBreathController>();
			if (InitedParameters.ContainsKey("paramBreath"))
			{
				breathController.enabled = true;
				var par = (CubismParameter)InitedParameters["paramBreath"].Parameter;
				par.gameObject.AddComponent<CubismBreathParameter>();
			}
		}

		public void DoJsonPrase(JObject jsonResult)
        {
			try
			{
				foreach (KeyValuePair<string, ParametersClass> kvp in InitedParameters)
				{
					if (jsonResult.ContainsKey(kvp.Value.SDKName))
					{
						kvp.Value.NowValue = float.Parse(jsonResult[kvp.Value.SDKName].ToString());
					}
				}
				if (jsonResult.ContainsKey("isTracked"))
				{
					IsNewSDK = true;
					isTracked = bool.Parse(jsonResult["isTracked"].ToString());
				}
				else
				{
					IsNewSDK = false;
				}
			}
			catch { }
        }

		public void GetParameters()
		{
			var jsonDataPath = Application.streamingAssetsPath + "/Parameters.json";
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
			}
		}

		public void setParameter(CubismParameter param, float value, float MinValue, float MaxValue, float MinSetValue, float MaxSetValue)
		{
			if (param != null)
			{
				var get = (MaxValue - MinValue) * (value - MinSetValue) / (MaxSetValue - MinSetValue) + MinValue;
				if (value <= MinSetValue)
				{
					get = MinValue;
				}
				else if(value >= MaxSetValue)
				{
					get = MaxValue;
				}
				var smooth = Mathf.SmoothStep(param.Value, get, 0.5f);
				param.Value = smooth;
			}
		}

		public Dictionary<string, string> GetModelSettings()
		{
			var returnDict = new Dictionary<string, string>();
			foreach (KeyValuePair<string, ParametersClass> kvp in InitedParameters)
			{
				if (kvp.Value.Parameter != null && kvp.Value.Name != "paramBreath")
				{
					returnDict.Add(kvp.Value.Name, kvp.Value.MinSetValue.ToString() + "|" + kvp.Value.MaxSetValue.ToString());
				}
			}
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
			returnDict.Add("LostResetAction", LostResetAction.ToString());
			returnDict.Add("LostResetMotion", LostResetMotion);
			return returnDict;
		}

		public void SetModelOtherSettings(Dictionary<string, string> otherDict)
		{
			if (CheckNameInDict("LostReset", otherDict))
			{
				LostReset = bool.Parse(otherDict["LostReset"]);
			}
			if (CheckNameInDict("LostResetAction", otherDict))
			{
				LostResetAction = int.Parse(otherDict["LostResetAction"]);
			}
			if (CheckNameInDict("LostResetMotion", otherDict))
			{
				LostResetMotion = otherDict["LostResetMotion"];
			}
		}

		public Dictionary<string, string> GetModelLocationSettings()
		{
			var returnDict = new Dictionary<string, string>();
			returnDict.Add("transformXValue", transform.position.x.ToString());
			returnDict.Add("transformYValue", transform.position.y.ToString());
			returnDict.Add("transformSValue", transform.localScale.x.ToString());
			returnDict.Add("transformRValue", transform.localEulerAngles.z.ToString());
			returnDict.Add("ctransformXValue", ConnectionLost.transform.position.x.ToString());
			returnDict.Add("ctransformYValue", ConnectionLost.transform.position.y.ToString());
			returnDict.Add("ctransformSValue", ConnectionLost.transform.localScale.x.ToString());
			return returnDict;
		}

		public void SetModelLocationSettings(Dictionary<string, string> locationInfo)
		{
			if (CheckNameInDict("transformXValue", locationInfo) && CheckNameInDict("transformYValue", locationInfo))
			{
				transform.position = new Vector3(Convert.ToSingle(locationInfo["transformXValue"]), Convert.ToSingle(locationInfo["transformYValue"]), transform.position.z);
			}
			if (CheckNameInDict("transformSValue", locationInfo))
			{
				transform.localScale = new Vector3(Convert.ToSingle(locationInfo["transformSValue"]), Convert.ToSingle(locationInfo["transformSValue"]));
			}
			if (CheckNameInDict("transformRValue", locationInfo))
			{
				transform.localEulerAngles = new Vector3(0, 0, Convert.ToSingle(locationInfo["transformRValue"]));
			}
			if (CheckNameInDict("ctransformXValue", locationInfo) && CheckNameInDict("ctransformYValue", locationInfo))
			{
				ConnectionLost.transform.position = new Vector3(Convert.ToSingle(locationInfo["ctransformXValue"]), Convert.ToSingle(locationInfo["ctransformYValue"]), ConnectionLost.transform.position.z);
			}
			if (CheckNameInDict("ctransformSValue", locationInfo))
			{
				ConnectionLost.transform.localScale = new Vector3(Convert.ToSingle(locationInfo["ctransformSValue"]), Convert.ToSingle(locationInfo["ctransformSValue"]));
			}
		}

		public bool CheckNameInDict(string name, Dictionary<string, string> otherDict)
		{
			return otherDict.ContainsKey(name);
		}
	}
}