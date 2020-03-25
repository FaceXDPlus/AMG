using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Newtonsoft.Json.Linq;

namespace D2LiveManager.Live2DCubism3
{
    public class Live2DCubism3FaceAnimationController : FaceAnimationController
    {
        
        [Header ("[Target]")]

        public bool isModelChanged = false;

        public CubismModel live2DCubism3Model;

        protected CubismParameter paramEyeLOpen;

        protected CubismParameter paramEyeROpen;

        protected CubismParameter paramBrowLY;

        protected CubismParameter paramBrowRY;

        protected CubismParameter paramMouthOpenY;

        protected CubismParameter paramMouthForm;


        #region D2LiveManagerProcess

        public override string GetDescription ()
        {
            return "Update face animation of Live2DCubism3Model using FaceLandmarkGetter.";
        }

        public override void LateUpdateValue ()
        {
            if (live2DCubism3Model == null)
                return;

            if (isModelChanged)
            {
                getParameters();
            }

            if (enableEye) {
                paramEyeLOpen.Value = Mathf.Lerp (0.0f, 1.0f, EyeParam);
                paramEyeROpen.Value = Mathf.Lerp(0.0f, 1.0f, EyeParam);
            }

            if (enableBrow) {
                paramBrowLY.Value = Mathf.Lerp (-1.0f, 1.0f, BrowParam);
                paramBrowRY.Value = Mathf.Lerp (-1.0f, 1.0f, BrowParam);
            }

            if (enableMouth) {
                paramMouthOpenY.Value = Mathf.Lerp (0.0f, 1.0f, MouthOpenParam);
                paramMouthForm.Value = Mathf.Lerp (-1.0f, 1.0f, MouthSizeParam);
            }
        }

        #endregion


        #region FaceAnimationController

        public override void Setup ()
        {
            base.Setup ();

            NullCheck (live2DCubism3Model, "live2DCubism3Model");


            getParameters();
        }

        public void getParameters()
        {
            var jsonDataPath = Application.streamingAssetsPath + "/Parameters.json";
            JObject jsonParams = D2LiveParametersController.getParametersJson(jsonDataPath);

            paramEyeLOpen = D2LiveParametersController.getParametersFromJson("paramEyeLOpen", jsonParams, live2DCubism3Model);
            paramEyeROpen = D2LiveParametersController.getParametersFromJson("paramEyeROpen", jsonParams, live2DCubism3Model);


            paramBrowLY = D2LiveParametersController.getParametersFromJson("paramBrowLY", jsonParams, live2DCubism3Model);
            paramBrowRY = D2LiveParametersController.getParametersFromJson("paramBrowRY", jsonParams, live2DCubism3Model);
            paramMouthOpenY = D2LiveParametersController.getParametersFromJson("paramMouthOpenY", jsonParams, live2DCubism3Model);
            paramMouthForm = D2LiveParametersController.getParametersFromJson("paramMouthForm", jsonParams, live2DCubism3Model);
        }

        protected override void UpdateFaceAnimation (List<Vector2> points)
        {
            if (isModelChanged)
            {
                getParameters();
            }

            if (enableEye) {
                float eyeOpen = (GetLeftEyeOpenRatio (points) + GetRightEyeOpenRatio (points)) / 2.0f;
                //Debug.Log ("eyeOpen " + eyeOpen);

                if (eyeOpen >= 0.88f) {
                    eyeOpen = 1.0f;
                } else if (eyeOpen >= 0.45f) {
                    eyeOpen = 0.5f;
                } else if (eyeOpen >= 0.25f) {
                    eyeOpen = 0.2f;
                } else {
                    eyeOpen = 0.0f;
                }

                EyeParam = Mathf.Lerp (EyeParam, eyeOpen, eyeLeapT);
            }

            if (enableBrow) {
                float browOpen = (GetLeftEyebrowUPRatio (points) + GetRightEyebrowUPRatio (points)) / 2.0f;
                //Debug.Log("browOpen " + browOpen);

                if (browOpen >= 0.75f) {
                    browOpen = 1.0f;
                } else if (browOpen >= 0.4f) {
                    browOpen = 0.5f;
                } else {
                    browOpen = 0.0f;
                }
                BrowParam = Mathf.Lerp (BrowParam, browOpen, browLeapT);
            }

            if (enableMouth) {
                float mouthOpen = GetMouthOpenYRatio (points);
                //Debug.Log("mouthOpen " + mouthOpen);

                if (mouthOpen >= 0.7f) {
                    mouthOpen = 1.0f;
                } else if (mouthOpen >= 0.25f) {
                    mouthOpen = 0.5f;
                } else {
                    mouthOpen = 0.0f;
                }
                MouthOpenParam = Mathf.Lerp (MouthOpenParam, mouthOpen, mouthLeapT);


                float mouthSize = GetMouthOpenXRatio (points);
                //Debug.Log("mouthSize " + mouthSize);

                if (mouthSize >= 0.5f) {
                    mouthSize = 1.0f;
                } else {
                    mouthSize = 0.0f;
                }
                MouthSizeParam = Mathf.Lerp (MouthSizeParam, mouthSize, mouthLeapT);
            }
        }

        #endregion

    }
}