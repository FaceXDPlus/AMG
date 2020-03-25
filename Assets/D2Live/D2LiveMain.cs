using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using D2LiveManager;
using System;
using System.IO;
using Live2D.Cubism.Framework.Json;

namespace D2Live
{
    public class D2LiveMain : MonoBehaviour
    {
        /// <summary>
        /// The webcam texture mat source getter.
        /// </summary>
        public WebCamTextureMatSourceGetter webCamTextureMatSourceGetter;

        public DlibFaceLandmarkGetter dlibFaceLandmarkGetter;


        // Use this for initialization
        void Start ()
        {
            // Load global settings.
            dlibFaceLandmarkGetter.dlibShapePredictorFileName = D2LiveDlibCV.dlibShapePredictorFileName;
            dlibFaceLandmarkGetter.dlibShapePredictorMobileFileName = D2LiveDlibCV.dlibShapePredictorFileName;
      }

        public void OnBackButtonClick ()
        {
            Application.Quit();
        }
    }
}