using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using D2Live;
using Live2D.Cubism.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace D2LiveManager.Live2DCubism3
{
	public class D2LiveModelController : D2LiveAnimationController
	{

		[Header("[Target]")]

		public bool isModelChanged = false;

		public CubismModel live2DCubism3Model;

		[Range(0, 1)]
		public float paramEyeLOpenValue = 0;

		[Range(0, 1)]
		public float paramEyeROpenValue = 0;

		[Range(-1, 1)]
		public float paramBrowLYValue = 0;

		[Range(-1, 1)]
		public float paramBrowRYValue = 0;

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

		protected CubismParameter paramEyeLOpen;
		protected CubismParameter paramEyeROpen;
		protected CubismParameter paramBrowLY;
		protected CubismParameter paramBrowRY;
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

		public GameObject[] GameObject;

		public override void Setup()
		{
			base.Setup();

			NullCheck(live2DCubism3Model, "live2DCubism3Model");

			getParameters();
			
		}

		public void getParameters()
		{
			var jsonDataPath = Application.streamingAssetsPath + "/Parameters.json";
			JObject jsonParams = D2LiveParametersController.getParametersJson(jsonDataPath);

			paramEyeLOpen = D2LiveParametersController.getParametersFromJson("paramEyeLOpen", jsonParams, live2DCubism3Model);
			paramEyeROpen = D2LiveParametersController.getParametersFromJson("paramEyeROpen", jsonParams, live2DCubism3Model);

			paramAngleX = D2LiveParametersController.getParametersFromJson("paramAngleX", jsonParams, live2DCubism3Model);
			paramAngleY = D2LiveParametersController.getParametersFromJson("paramAngleY", jsonParams, live2DCubism3Model);
			paramAngleZ = D2LiveParametersController.getParametersFromJson("paramAngleZ", jsonParams, live2DCubism3Model);
			paramBodyAngleX = D2LiveParametersController.getParametersFromJson("paramBodyAngleX", jsonParams, live2DCubism3Model);
			paramBodyAngleY = D2LiveParametersController.getParametersFromJson("paramBodyAngleY", jsonParams, live2DCubism3Model);
			paramBodyAngleZ = D2LiveParametersController.getParametersFromJson("paramBodyAngleZ", jsonParams, live2DCubism3Model);

			paramEyeBallX = D2LiveParametersController.getParametersFromJson("paramEyeBallX", jsonParams, live2DCubism3Model);
			paramEyeBallY = D2LiveParametersController.getParametersFromJson("paramEyeBallY", jsonParams, live2DCubism3Model);
			paramBrowAngleL = D2LiveParametersController.getParametersFromJson("paramBrowAngleL", jsonParams, live2DCubism3Model);
			paramBrowAngleR = D2LiveParametersController.getParametersFromJson("paramBrowAngleR", jsonParams, live2DCubism3Model);

			paramBrowLY = D2LiveParametersController.getParametersFromJson("paramBrowLY", jsonParams, live2DCubism3Model);
			paramBrowRY = D2LiveParametersController.getParametersFromJson("paramBrowRY", jsonParams, live2DCubism3Model);
			paramMouthOpenY = D2LiveParametersController.getParametersFromJson("paramMouthOpenY", jsonParams, live2DCubism3Model);
			paramMouthForm = D2LiveParametersController.getParametersFromJson("paramMouthForm", jsonParams, live2DCubism3Model);
		}


		public override void LateUpdateValue()
		{
			if (live2DCubism3Model == null)
				return;

			if (isModelChanged)
			{
				getParameters();
			}

			paramEyeLOpen.Value = paramEyeLOpenValue;

			paramEyeROpen.Value = paramEyeROpenValue;

			paramAngleX.Value = paramAngleXValue;
			paramAngleY.Value = paramAngleYValue;
			paramAngleZ.Value = paramAngleZValue;
			paramBodyAngleX.Value = ParamBodyAngleXValue;
			paramBodyAngleY.Value = ParamBodyAngleYValue;
			paramBodyAngleZ.Value = ParamBodyAngleZValue;

			paramBrowLY.Value = paramBrowLYValue;
			paramBrowRY.Value = paramBrowRYValue;
			paramEyeBallX.Value = ParamEyeBallXValue;
			paramEyeBallY.Value = ParamEyeBallYValue;
			paramBrowAngleL.Value = paramBrowAngleLValue;
			paramBrowAngleR.Value = paramBrowAngleRValue;

			paramMouthOpenY.Value = paramMouthOpenYValue;
			paramMouthForm.Value = paramMouthFormValue;
		}

		public override void UpdateValue()
		{
			UpdateFaceAnimation();
		}


		public override string GetDescription()
		{
			return "Update face animation of Live2DCubism3Model using D2LiveModelController.";
		}

		protected override void UpdateFaceAnimation()
		{
			/*EyeLOpenParam = Mathf.Lerp(EyeLOpenParam, paramEyeLOpenValue, eyeLeapT);
			EyeROpenParam = Mathf.Lerp(EyeROpenParam, paramEyeROpenValue, eyeLeapT);
			BrowLYParam = Mathf.Lerp(BrowLYParam, paramEyeLOpenValue, browLeapT);
			BrowRYParam = Mathf.Lerp(BrowRYParam, paramEyeLOpenValue, browLeapT);
			AngleXValueParam = Mathf.Lerp(AngleXValueParam, paramAngleXValue, angleLeapT);
			AngleYValueParam = Mathf.Lerp(AngleYValueParam, paramAngleYValue, angleLeapT);
			AngleZValueParam = Mathf.Lerp(AngleZValueParam, paramAngleZValue, angleLeapT);
			BodyAngleXValueParam = Mathf.Lerp(BodyAngleXValueParam, ParamBodyAngleXValue, bodyAngleLeapT);
			BodyAngleYValueParam = Mathf.Lerp(BodyAngleYValueParam, ParamBodyAngleYValue, bodyAngleLeapT);
			BodyAngleZValueParam = Mathf.Lerp(BodyAngleZValueParam, ParamBodyAngleZValue, bodyAngleLeapT);
			EyeBallXValueParam = Mathf.Lerp(EyeBallXValueParam, ParamEyeBallXValue, eyeBallLeapT);
			EyeBallYValueParam = Mathf.Lerp(EyeBallYValueParam, ParamEyeBallYValue, eyeBallLeapT);
			BrowAngleLValueParam = Mathf.Lerp(BrowAngleLValueParam, paramBrowAngleLValue, browAngleLeapT);
			BrowAngleRValueParam = Mathf.Lerp(BrowAngleRValueParam, paramBrowAngleRValue, browAngleLeapT);
			MouthFormValueParam = Mathf.Lerp(MouthFormValueParam, paramMouthFormValue, mouthLeapT);
			MouthOpenYValueParam = Mathf.Lerp(MouthOpenYValueParam, paramMouthOpenYValue, mouthLeapT);*/

		}
	}
}