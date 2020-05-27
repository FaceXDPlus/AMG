using Live2D.Cubism.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
    public class Live2dModelController : MonoBehaviour
    {
        public string ConnectionIP;
        public string ConnectionMessage;
		public MainPanelController MainPanelController = null;
		public GameObject ConnectionLost;

		private Vector3 screenPos;
		private Vector3 offset;
		private Vector3 ConnectionLostScreenPos;
		private Vector3 ConnectionLostOffset;

		#region paramValues

		[Range(0, 1)]
		public float paramEyeLOpenValue = 0;

		[Range(0, 1)]
		public float paramEyeROpenValue = 0;

		[Range(-1, 1)]
		public float paramBrowLYValue = 0;

		[Range(-1, 1)]
		public float paramBrowRYValue = 0;

		[Range(-1, 1)]
		public float paramBrowLFormValue = 0;

		[Range(-1, 1)]
		public float paramBrowRFormValue = 0;

		[Range(-1, 1)]
		public float paramBrowAngleLValue = 0;

		[Range(-1, 1)]
		public float paramBrowAngleRValue = 0;

		[Range(0, 1)]
		public float paramMouthOpenYValue;

		[Range(-1, 1)]
		public float paramMouthFormValue;

		[Range(-30, 30)]
		public float paramAngleXValue;

		[Range(-30, 30)]
		public float paramAngleYValue;

		[Range(-30, 30)]
		public float paramAngleZValue;

		[Range(-1, 1)]
		public float ParamEyeBallXValue;

		[Range(-1, 1)]
		public float ParamEyeBallYValue;

		[Range(-10, 10)]
		public float ParamBodyAngleXValue;

		[Range(-10, 10)]
		public float ParamBodyAngleYValue;

		[Range(-10, 10)]
		public float ParamBodyAngleZValue;

		#endregion


		public bool changeEyeBallLR = false;
		public bool lostReset = false;
		public bool lostResetFlag = false;
		public float paramAngleZLastValue;
		public bool enableBreath = true;

		#region paramParameterAlign

		public float paramEyeLOpenAlignValue = 0;
		public float paramEyeROpenAlignValue = 0;
		public float paramBrowLYAlignValue = 0;
		public float paramBrowRYAlignValue = 0;
		public float paramBrowAngleLAlignValue = 0;
		public float paramBrowAngleRAlignValue = 0;
		public float paramBrowLFormAlignValue = 0;
		public float paramBrowRFormAlignValue = 0;
		public float paramMouthOpenYAlignValue = 0;
		public float paramMouthFormAlignValue = 0;
		public float paramAngleXAlignValue = 0;
		public float paramAngleYAlignValue = 0;
		public float paramAngleZAlignValue = 0;
		public float paramEyeBallXAlignValue = 0;
		public float paramEyeBallYAlignValue = 0;

		#endregion

		#region paramParameter

		protected CubismParameter paramEyeLOpen;
		protected CubismParameter paramEyeROpen;
		protected CubismParameter paramBrowLY;
		protected CubismParameter paramBrowRY;
		protected CubismParameter paramBrowLForm;
		protected CubismParameter paramBrowRForm;
		protected CubismParameter paramMouthOpenY;
		protected CubismParameter paramMouthForm;
		protected CubismParameter paramAngleX;
		protected CubismParameter paramAngleY;
		protected CubismParameter paramAngleZ;
		protected CubismParameter paramBodyAngleX;
		protected CubismParameter paramBodyAngleY;
		protected CubismParameter paramBodyAngleZ;
		protected CubismParameter paramEyeBallX;
		protected CubismParameter paramEyeBallY;
		protected CubismParameter paramBrowAngleL;
		protected CubismParameter paramBrowAngleR;

		protected CubismParameter paramBreath;

		#endregion

		public string DisplayName;
		public string ModelPath;
		public ArrayList animationClips;
		public Animation Animation;

		void Start()
        {
			getParameters();
			initBreath();
		}

		public void Update()
		{
			try
			{
				if (MainPanelController != null)
				{
					if (MainPanelController.GetModelSelected() == this.GetComponent<CubismModel>().name)
					{
						ProcessPosition();
					}
				}
				//处理存入的数据
				//DoJsonPrase(ConnectionMessage);
				//更新模型位置
				ProcessModelParameter();
				
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
				//var Scale = Input.GetAxis("Mouse ScrollWheel") * 0.005f;
				//ConnectionLost.gameObject.transform.localScale += new Vector3(Scale, Scale);
			}
		}

		public void ProcessModelParameter()
		{
			if (changeEyeBallLR)
			{
				setParameter(paramEyeROpen, paramEyeLOpenValue, paramEyeLOpenAlignValue);
				setParameter(paramEyeLOpen, paramEyeROpenValue, paramEyeROpenAlignValue);
			}
			else
			{
				setParameter(paramEyeLOpen, paramEyeLOpenValue, paramEyeLOpenAlignValue);
				setParameter(paramEyeROpen, paramEyeROpenValue, paramEyeROpenAlignValue);
			}

			setParameter(paramAngleX, paramAngleXValue, paramAngleXAlignValue);
			setParameter(paramAngleY, paramAngleYValue, paramAngleYAlignValue);
			setParameter(paramAngleZ, paramAngleZValue, paramAngleZAlignValue);

			//setParameter(paramBodyAngleX, ParamBodyAngleXValue);
			//setParameter(paramBodyAngleY, ParamBodyAngleYValue);
			//setParameter(paramBodyAngleZ, ParamBodyAngleZValue);

			setParameter(paramBrowLY, paramBrowLYValue, paramBrowLYAlignValue);
			setParameter(paramBrowRY, paramBrowRYValue, paramBrowRYAlignValue);

			setParameter(paramEyeBallX, ParamEyeBallXValue, paramEyeBallXAlignValue);
			setParameter(paramEyeBallY, ParamEyeBallYValue, paramEyeBallYAlignValue);
			setParameter(paramBrowAngleL, paramBrowAngleLValue, paramBrowAngleLAlignValue);
			setParameter(paramBrowAngleR, paramBrowAngleRValue, paramBrowAngleRAlignValue);
			setParameter(paramBrowLForm, paramBrowLFormValue, paramBrowLFormAlignValue);
			setParameter(paramBrowRForm, paramBrowRFormValue, paramBrowRFormAlignValue);
			setParameter(paramMouthOpenY, paramMouthOpenYValue, paramMouthOpenYAlignValue);
			setParameter(paramMouthForm, paramMouthFormValue, paramMouthFormAlignValue);
		}

		void OnMouseDown()
		{
			screenPos = Camera.main.WorldToScreenPoint(transform.position);//获取物体的屏幕坐标     
			offset = screenPos - Input.mousePosition;//获取物体与鼠标在屏幕上的偏移量    
		}

		void OnMouseDrag()
		{
			var cc = Camera.main.ScreenToWorldPoint(Input.mousePosition + offset);
			transform.position = new Vector3(cc.x, cc.y, transform.position.z);
		}

		void OnMouseDownConnectionLost()
		{
			ConnectionLostScreenPos = Camera.main.WorldToScreenPoint(ConnectionLost.transform.position);//获取物体的屏幕坐标     
			ConnectionLostOffset = ConnectionLostScreenPos - Input.mousePosition;//获取物体与鼠标在屏幕上的偏移量    
		}

		void OnMouseDragConnectionLost()
		{
			var cc = Camera.main.ScreenToWorldPoint(Input.mousePosition + ConnectionLostOffset);
			ConnectionLost.transform.position = new Vector3(cc.x, cc.y, ConnectionLost.transform.position.z);
		}

		public void ResetModel()
		{
			paramEyeLOpenValue = 1 - paramEyeLOpenAlignValue;
			paramEyeROpenValue = 1 - paramEyeROpenAlignValue;
			paramAngleXValue = paramAngleXAlignValue;
			paramAngleYValue = paramAngleYAlignValue;
			paramAngleZValue = paramAngleZAlignValue;
			paramBrowLYValue = paramBrowLYAlignValue;
			paramBrowRYValue = paramBrowRYAlignValue;
			paramBrowAngleLValue = paramBrowAngleLAlignValue;
			paramBrowAngleRValue = paramBrowAngleRAlignValue;
			paramBrowLFormValue = paramBrowLFormAlignValue;
			paramBrowRFormValue = paramBrowRFormAlignValue;
			ParamEyeBallXValue = paramEyeBallXAlignValue;
			ParamEyeBallYValue = paramEyeBallYAlignValue;
			paramMouthOpenYValue = paramMouthOpenYAlignValue;
			paramMouthFormValue = paramMouthFormAlignValue;

			if (changeEyeBallLR)
			{
				setParameter(paramEyeROpen, paramEyeLOpenValue, paramEyeLOpenAlignValue);
				setParameter(paramEyeLOpen, paramEyeROpenValue, paramEyeROpenAlignValue);
			}
			else
			{
				setParameter(paramEyeLOpen, paramEyeLOpenValue, paramEyeLOpenAlignValue);
				setParameter(paramEyeROpen, paramEyeROpenValue, paramEyeROpenAlignValue);
			}

			setParameter(paramAngleX, paramAngleXValue, paramAngleXAlignValue);
			setParameter(paramAngleY, paramAngleYValue, paramAngleYAlignValue);
			setParameter(paramAngleZ, paramAngleZValue, paramAngleZAlignValue);

			setParameter(paramBrowLY, paramBrowLYValue, paramBrowLYAlignValue);
			setParameter(paramBrowRY, paramBrowRYValue, paramBrowRYAlignValue);

			setParameter(paramEyeBallX, ParamEyeBallXValue, paramEyeBallXAlignValue);
			setParameter(paramEyeBallY, ParamEyeBallYValue, paramEyeBallYAlignValue);
			setParameter(paramBrowAngleL, paramBrowAngleLValue, paramBrowAngleLAlignValue);
			setParameter(paramBrowAngleR, paramBrowAngleRValue, paramBrowAngleRAlignValue);
			setParameter(paramBrowLForm, paramBrowLFormValue, paramBrowLFormAlignValue);
			setParameter(paramBrowRForm, paramBrowRFormValue, paramBrowRFormAlignValue);
			setParameter(paramMouthOpenY, paramMouthOpenYValue, paramMouthOpenYAlignValue);
			setParameter(paramMouthForm, paramMouthFormValue, paramMouthFormAlignValue);
		}

		public void initBreath()
		{
			var breathController = this.gameObject.AddComponent<CubismBreathController>();
			if (paramBreath != null)
			{
				breathController.enabled = true;
				paramBreath.gameObject.AddComponent<CubismBreathParameter>();
			}
		}

		public void DoJsonPrase(string input)
        {
            var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(input);
            if (paramAngleZLastValue != float.Parse(jsonResult["headRoll"].ToString()))
            {
				//ConnectionLost.SetActive(false);
				paramAngleZLastValue = float.Parse(jsonResult["headRoll"].ToString());
                paramMouthOpenYValue = float.Parse(jsonResult["mouthOpenY"].ToString());
                ParamEyeBallXValue = float.Parse(jsonResult["eyeX"].ToString());
                ParamEyeBallYValue = float.Parse(jsonResult["eyeY"].ToString());
                paramAngleXValue = float.Parse(jsonResult["headYaw"].ToString());
                paramAngleYValue = float.Parse(jsonResult["headPitch"].ToString());
                paramAngleZValue = float.Parse(jsonResult["headRoll"].ToString());
                //ParamBodyAngleXValue = float.Parse(jsonResult["bodyAngleX"].ToString());
                //ParamBodyAngleYValue = float.Parse(jsonResult["bodyAngleY"].ToString());
                //ParamBodyAngleZValue = float.Parse(jsonResult["bodyAngleZ"].ToString());
                paramBrowLFormValue = float.Parse(jsonResult["eyeBrowLForm"].ToString());
                paramBrowRFormValue = float.Parse(jsonResult["eyeBrowRForm"].ToString());
                paramBrowAngleLValue = float.Parse(jsonResult["eyeBrowAngleL"].ToString());
                paramBrowAngleRValue = float.Parse(jsonResult["eyeBrowAngleR"].ToString());
                paramMouthFormValue = float.Parse(jsonResult["mouthForm"].ToString());
                paramBrowRYValue = float.Parse(jsonResult["eyeBrowYR"].ToString());
                paramBrowLYValue = float.Parse(jsonResult["eyeBrowYL"].ToString());
                paramEyeROpenValue = float.Parse(jsonResult["eyeROpen"].ToString());
                paramEyeLOpenValue = float.Parse(jsonResult["eyeLOpen"].ToString());
                if (jsonResult.Property("paramAngleXAlignValue") != null)
                {
                    paramAngleXAlignValue = float.Parse(jsonResult["paramAngleXAlignValue"].ToString());
                    paramAngleYAlignValue = float.Parse(jsonResult["paramAngleYAlignValue"].ToString());
                    paramAngleZAlignValue = float.Parse(jsonResult["paramAngleZAlignValue"].ToString());
                    paramEyeBallXAlignValue = float.Parse(jsonResult["paramEyeBallXAlignValue"].ToString());
                    paramEyeBallYAlignValue = float.Parse(jsonResult["paramEyeBallYAlignValue"].ToString());
                }
            }
        }

		public void getParameters()
		{
			var jsonDataPath = Application.streamingAssetsPath + "/Parameters.json";
			JObject jsonParams = Live2DParametersController.getParametersJson(jsonDataPath);
			var model = this.GetComponent<CubismModel>();
			paramEyeLOpen = Live2DParametersController.getParametersFromJson("paramEyeLOpen", jsonParams, model);
			paramEyeROpen = Live2DParametersController.getParametersFromJson("paramEyeROpen", jsonParams, model);

			paramAngleX = Live2DParametersController.getParametersFromJson("paramAngleX", jsonParams, model);
			paramAngleY = Live2DParametersController.getParametersFromJson("paramAngleY", jsonParams, model);
			paramAngleZ = Live2DParametersController.getParametersFromJson("paramAngleZ", jsonParams, model);
			paramBodyAngleX = Live2DParametersController.getParametersFromJson("paramBodyAngleX", jsonParams, model);
			paramBodyAngleY = Live2DParametersController.getParametersFromJson("paramBodyAngleY", jsonParams, model);
			paramBodyAngleZ = Live2DParametersController.getParametersFromJson("paramBodyAngleZ", jsonParams, model);

			paramEyeBallX = Live2DParametersController.getParametersFromJson("paramEyeBallX", jsonParams, model);
			paramEyeBallY = Live2DParametersController.getParametersFromJson("paramEyeBallY", jsonParams, model);
			paramBrowLForm = Live2DParametersController.getParametersFromJson("paramBrowLForm", jsonParams, model);
			paramBrowRForm = Live2DParametersController.getParametersFromJson("paramBrowRForm", jsonParams, model);
			paramBrowAngleL = Live2DParametersController.getParametersFromJson("paramBrowAngleL", jsonParams, model);
			paramBrowAngleR = Live2DParametersController.getParametersFromJson("paramBrowAngleR", jsonParams, model);

			paramBrowLY = Live2DParametersController.getParametersFromJson("paramBrowLY", jsonParams, model);
			paramBrowRY = Live2DParametersController.getParametersFromJson("paramBrowRY", jsonParams, model);
			paramMouthOpenY = Live2DParametersController.getParametersFromJson("paramMouthOpenY", jsonParams, model);
			paramMouthForm = Live2DParametersController.getParametersFromJson("paramMouthForm", jsonParams, model);
			paramBreath = Live2DParametersController.getParametersFromJson("paramBreath", jsonParams, model);
		}

		public void setParameter(CubismParameter param, float value, float align)
		{
			if (param != null)
			{
				var smooth = Mathf.SmoothStep(param.Value, value - align, 0.5f);
				//param.Value = value - align;
				param.Value = smooth;
			}
		}

		public void setParameterAlign()
		{
			paramEyeLOpenAlignValue = 1 - paramEyeLOpenValue;
			paramEyeROpenAlignValue = 1 - paramEyeROpenValue;
			paramAngleXAlignValue = paramAngleXValue;
			paramAngleYAlignValue = paramAngleYValue;
			paramAngleZAlignValue = paramAngleZValue;
			paramBrowLYAlignValue = paramBrowLYValue;
			paramBrowRYAlignValue = paramBrowRYValue;
			paramBrowLFormAlignValue = paramBrowLFormValue;
			paramBrowRFormAlignValue = paramBrowRFormValue;
			paramEyeBallXAlignValue = ParamEyeBallXValue;
			paramEyeBallYAlignValue = ParamEyeBallYValue;
			paramBrowAngleLAlignValue = paramBrowAngleLValue;
			paramBrowAngleRAlignValue = paramBrowAngleRValue;
			paramMouthOpenYAlignValue = paramMouthOpenYValue;
			paramMouthFormAlignValue = paramMouthFormValue;
		}

		public string getParameterAlign()
		{
			var returnStr =
				getParameterAlignStr("paramEyeLOpenAlignValue", paramEyeLOpenAlignValue) +
				getParameterAlignStr("paramEyeROpenAlignValue", paramEyeROpenAlignValue) +
				getParameterAlignStr("paramAngleXAlignValue", paramAngleXAlignValue) +
				getParameterAlignStr("paramAngleYAlignValue", paramAngleYAlignValue) +
				getParameterAlignStr("paramAngleZAlignValue", paramAngleZAlignValue) +
				getParameterAlignStr("paramBrowLYAlignValue", paramBrowLYAlignValue) +
				getParameterAlignStr("paramBrowRYAlignValue", paramBrowRYAlignValue) +
				getParameterAlignStr("paramBrowLFormAlignValue", paramBrowLFormAlignValue) +
				getParameterAlignStr("paramBrowRFormAlignValue", paramBrowRFormAlignValue) +
				getParameterAlignStr("paramEyeBallXAlignValue", paramEyeBallXAlignValue) +
				getParameterAlignStr("paramEyeBallYAlignValue", paramEyeBallYAlignValue) +
				getParameterAlignStr("paramBrowAngleLAlignValue", paramBrowAngleLAlignValue) +
				getParameterAlignStr("paramBrowAngleRAlignValue", paramBrowAngleRAlignValue) +
				getParameterAlignStr("paramMouthOpenYAlignValue", paramMouthOpenYAlignValue) +
				getParameterAlignStr("paramMouthFormAlignValue", paramMouthFormAlignValue);
			return returnStr;
		}

		public string getParameterAlignStr(string name, float value)
		{
			var returnStr = ",\"" + name + "\":\"" + value + "\"";
			return returnStr;
		}

		public Dictionary<string, string> GetModelSettings()
		{
			var returnArray = new Dictionary<string, string>();
			returnArray.Add("paramChangeEyeLR", changeEyeBallLR.ToString());
			returnArray.Add("paramLostReset", lostReset.ToString());
			returnArray.Add("paramEyeLOpenAlignValue", paramEyeLOpenAlignValue.ToString());
			returnArray.Add("paramEyeROpenAlignValue", paramEyeROpenAlignValue.ToString());
			returnArray.Add("paramAngleXAlignValue", paramAngleXAlignValue.ToString());
			returnArray.Add("paramAngleYAlignValue", paramAngleYAlignValue.ToString());
			returnArray.Add("paramAngleZAlignValue", paramAngleZAlignValue.ToString());
			returnArray.Add("paramBrowLYAlignValue", paramBrowLYAlignValue.ToString());
			returnArray.Add("paramBrowRYAlignValue", paramBrowRYAlignValue.ToString());
			returnArray.Add("paramBrowLFormAlignValue", paramBrowLFormAlignValue.ToString());
			returnArray.Add("paramBrowRFormAlignValue", paramBrowRFormAlignValue.ToString());
			returnArray.Add("paramEyeBallXAlignValue", paramEyeBallXAlignValue.ToString());
			returnArray.Add("paramEyeBallYAlignValue", paramEyeBallYAlignValue.ToString());
			returnArray.Add("paramBrowAngleLAlignValue", paramBrowAngleLAlignValue.ToString());
			returnArray.Add("paramBrowAngleRAlignValue", paramBrowAngleRAlignValue.ToString());
			returnArray.Add("paramMouthOpenYAlignValue", paramMouthOpenYAlignValue.ToString());
			returnArray.Add("paramMouthFormAlignValue", paramMouthFormAlignValue.ToString());
			returnArray.Add("transformXValue", transform.position.x.ToString());
			returnArray.Add("transformYValue", transform.position.y.ToString());
			returnArray.Add("transformSValue", transform.localScale.x.ToString());
			returnArray.Add("transformRValue", transform.localEulerAngles.z.ToString());
			//returnArray.Add("ctransformXValue", ConnectionLost.transform.position.x.ToString());
			//returnArray.Add("ctransformYValue", ConnectionLost.transform.position.y.ToString());
			//returnArray.Add("ctransformSValue", ConnectionLost.transform.localScale.x.ToString());

			return returnArray;
		}

		public void SetModelSettings(Dictionary<string, string> userInfo)
		{
			try
			{
				if (userInfo["paramChangeEyeLR"] == "True")
				{
					changeEyeBallLR = true;
				}

				if (userInfo["paramLostReset"] == "True")
				{
					lostReset = true;
				}

				paramEyeLOpenAlignValue = Convert.ToSingle(userInfo["paramEyeLOpenAlignValue"]);
				paramEyeROpenAlignValue = Convert.ToSingle(userInfo["paramEyeROpenAlignValue"]);
				paramAngleXAlignValue = Convert.ToSingle(userInfo["paramAngleXAlignValue"]);
				paramAngleYAlignValue = Convert.ToSingle(userInfo["paramAngleYAlignValue"]);
				paramAngleZAlignValue = Convert.ToSingle(userInfo["paramAngleZAlignValue"]);
				paramBrowLYAlignValue = Convert.ToSingle(userInfo["paramBrowLYAlignValue"]);
				paramBrowRYAlignValue = Convert.ToSingle(userInfo["paramBrowRYAlignValue"]);
				paramEyeBallXAlignValue = Convert.ToSingle(userInfo["paramEyeBallXAlignValue"]);
				paramEyeBallYAlignValue = Convert.ToSingle(userInfo["paramEyeBallYAlignValue"]);
				paramBrowLFormAlignValue = Convert.ToSingle(userInfo["paramBrowLFormAlignValue"]);
				paramBrowRFormAlignValue = Convert.ToSingle(userInfo["paramBrowRFormAlignValue"]);
				paramBrowAngleLAlignValue = Convert.ToSingle(userInfo["paramBrowAngleLAlignValue"]);
				paramBrowAngleRAlignValue = Convert.ToSingle(userInfo["paramBrowAngleRAlignValue"]);
				paramMouthOpenYAlignValue = Convert.ToSingle(userInfo["paramMouthOpenYAlignValue"]);
				paramMouthFormAlignValue = Convert.ToSingle(userInfo["paramMouthFormAlignValue"]);

				transform.position = new Vector3(Convert.ToSingle(userInfo["transformXValue"]), Convert.ToSingle(userInfo["transformYValue"]), transform.position.z);
				transform.localScale = new Vector3(Convert.ToSingle(userInfo["transformSValue"]), Convert.ToSingle(userInfo["transformSValue"]));
				transform.localEulerAngles = new Vector3(0, 0, Convert.ToSingle(userInfo["transformRValue"]));
				//ConnectionLost.transform.position = new Vector3(Convert.ToSingle(userInfo["ctransformXValue"]), Convert.ToSingle(userInfo["ctransformYValue"]), ConnectionLost.transform.position.z);
				//ConnectionLost.transform.localScale = new Vector3(Convert.ToSingle(userInfo["ctransformSValue"]), Convert.ToSingle(userInfo["ctransformSValue"]));
			}
			catch (Exception err)
			{
				Globle.AddDataLog("[Model]设置模型时发生错误：" + err.Message);
			}

		}
	}
}