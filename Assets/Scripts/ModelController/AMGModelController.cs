using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Physics;
using MaterialUI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
	public class AMGModelController : MonoBehaviour
    {
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

		public bool changeEyeBallLR = false;
		public bool enableBreath = true;

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

		private Vector3 screenPos;
		private Vector3 offset;
		private SelectionBoxConfig ControlDropdownBox;
		public string DisplayName;
		public string ModelPath;
		public ArrayList animationClips;

		private Animation animations;
		public Animation Animation   // property
		{
			get { return animations; }   // get method
			set { animations = value; }  // set method
		}

		public void setControlDropdownBox(SelectionBoxConfig ControlDropdownBox)
		{
			this.ControlDropdownBox = ControlDropdownBox;
		}

		void OnMouseDown()
		{
			screenPos = Camera.main.WorldToScreenPoint(transform.position);//获取物体的屏幕坐标     
			offset = screenPos - Input.mousePosition;//获取物体与鼠标在屏幕上的偏移量    
		}
		void OnMouseDrag()
		{
			this.GetComponent<CubismModel>().gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + offset);//将拖拽后的物体屏幕坐标还原为世界坐标
		}

		public void Start()
		{
			getParameters();
			initBreath();
		}

		public void Update()
		{
			try
			{
				if (ControlDropdownBox.selectedText.text == this.GetComponent<CubismModel>().name)
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
						var Scale = Input.GetAxis("Mouse ScrollWheel") * 2f;
						this.GetComponent<CubismModel>().gameObject.transform.localScale += new Vector3(Scale, Scale);
					}
				}
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
			catch (Exception err)
			{
				Globle.DataLog = Globle.DataLog + "模型发生错误 " + err.Message + " : " + err.StackTrace;
			}
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

		public void getParameters()
		{
			var jsonDataPath = Application.streamingAssetsPath + "/Parameters.json";
			JObject jsonParams = AMGParametersController.getParametersJson(jsonDataPath);
			var model = this.GetComponent<CubismModel>();
			paramEyeLOpen = AMGParametersController.getParametersFromJson("paramEyeLOpen", jsonParams, model);
			paramEyeROpen = AMGParametersController.getParametersFromJson("paramEyeROpen", jsonParams, model);

			paramAngleX = AMGParametersController.getParametersFromJson("paramAngleX", jsonParams, model);
			paramAngleY = AMGParametersController.getParametersFromJson("paramAngleY", jsonParams, model);
			paramAngleZ = AMGParametersController.getParametersFromJson("paramAngleZ", jsonParams, model);
			paramBodyAngleX = AMGParametersController.getParametersFromJson("paramBodyAngleX", jsonParams, model);
			paramBodyAngleY = AMGParametersController.getParametersFromJson("paramBodyAngleY", jsonParams, model);
			paramBodyAngleZ = AMGParametersController.getParametersFromJson("paramBodyAngleZ", jsonParams, model);

			paramEyeBallX = AMGParametersController.getParametersFromJson("paramEyeBallX", jsonParams, model);
			paramEyeBallY = AMGParametersController.getParametersFromJson("paramEyeBallY", jsonParams, model);
			paramBrowLForm = AMGParametersController.getParametersFromJson("paramBrowLForm", jsonParams, model);
			paramBrowRForm = AMGParametersController.getParametersFromJson("paramBrowRForm", jsonParams, model);
			paramBrowAngleL = AMGParametersController.getParametersFromJson("paramBrowAngleL", jsonParams, model);
			paramBrowAngleR = AMGParametersController.getParametersFromJson("paramBrowAngleR", jsonParams, model);

			paramBrowLY = AMGParametersController.getParametersFromJson("paramBrowLY", jsonParams, model);
			paramBrowRY = AMGParametersController.getParametersFromJson("paramBrowRY", jsonParams, model);
			paramMouthOpenY = AMGParametersController.getParametersFromJson("paramMouthOpenY", jsonParams, model);
			paramMouthForm = AMGParametersController.getParametersFromJson("paramMouthForm", jsonParams, model);
			paramBreath = AMGParametersController.getParametersFromJson("paramBreath", jsonParams, model);
		}

		public void setParameter(CubismParameter param, float value, float align)
		{
			if (param != null)
			{
				param.Value = value - align;
			}
		}

		public void setParameterAlign()
		{
			//paramEyeLOpenAlignValue = 1 - paramEyeLOpenValue;
			//paramEyeROpenAlignValue = 1 - paramEyeROpenValue;
			paramAngleXAlignValue   = paramAngleXValue;
			paramAngleYAlignValue   = paramAngleYValue;
			paramAngleZAlignValue   = paramAngleZValue;
			//paramBrowLYAlignValue   = paramBrowLYValue;
			//paramBrowRYAlignValue   = paramBrowRYValue;
			paramEyeBallXAlignValue = ParamEyeBallXValue;
			paramEyeBallYAlignValue = ParamEyeBallYValue;
			//paramBrowAngleLAlignValue = paramBrowAngleLValue;
			//paramBrowAngleRAlignValue = paramBrowAngleRValue;
			//paramMouthOpenYAlignValue = 1 - paramMouthOpenYValue;
			//paramMouthFormAlignValue  = paramMouthFormValue;
		}

		public string getParameterAlign()
		{
			var returnStr =
				getParameterAlignStr("paramAngleXAlignValue", paramAngleXAlignValue) +
				getParameterAlignStr("paramAngleYAlignValue", paramAngleYAlignValue) +
				getParameterAlignStr("paramAngleZAlignValue", paramAngleZAlignValue) +
				getParameterAlignStr("paramEyeBallXAlignValue", paramEyeBallXAlignValue) +
				getParameterAlignStr("paramEyeBallYAlignValue", paramEyeBallYAlignValue);
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
			returnArray.Add("paramChangeEyeLR"     , changeEyeBallLR.ToString());
			returnArray.Add("paramAngleXAlignValue", paramAngleXAlignValue.ToString());
			returnArray.Add("paramAngleYAlignValue", paramAngleYAlignValue.ToString());
			returnArray.Add("paramAngleZAlignValue", paramAngleZAlignValue.ToString());
			returnArray.Add("paramEyeBallXAlignValue", paramEyeBallXAlignValue.ToString());
			returnArray.Add("paramEyeBallYAlignValue", paramEyeBallYAlignValue.ToString());
			returnArray.Add("transformXValue", transform.position.x.ToString());
			returnArray.Add("transformYValue", transform.position.y.ToString());
			returnArray.Add("transformSValue", transform.localScale.x.ToString());
			returnArray.Add("transformRValue", transform.localEulerAngles.z.ToString());

			return returnArray;
		}

		public void SetModelSettings(Dictionary<string, string> userInfo)
		{
			try
			{
				if (userInfo["paramChangeEyeLR"] == "true")
				{
					changeEyeBallLR = true;
				}
				paramAngleXAlignValue = Convert.ToSingle(userInfo["paramAngleXAlignValue"]);
				paramAngleYAlignValue = Convert.ToSingle(userInfo["paramAngleYAlignValue"]);
				paramAngleZAlignValue = Convert.ToSingle(userInfo["paramAngleZAlignValue"]);
				paramEyeBallXAlignValue = Convert.ToSingle(userInfo["paramEyeBallXAlignValue"]);
				paramEyeBallYAlignValue = Convert.ToSingle(userInfo["paramEyeBallYAlignValue"]);
				transform.position = new Vector3(Convert.ToSingle(userInfo["transformXValue"]), Convert.ToSingle(userInfo["transformYValue"]));
				transform.localScale = new Vector3(Convert.ToSingle(userInfo["transformSValue"]), Convert.ToSingle(userInfo["transformSValue"]));
				transform.localEulerAngles = new Vector3(0, 0, Convert.ToSingle(userInfo["transformRValue"]));
			}
			catch (Exception err)
			{
				Globle.AddDataLog("[Model]设置模型时发生错误：" + err.Message);
			}

		}

	}
}