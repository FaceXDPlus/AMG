using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using D2LiveManager;

namespace D2Live
{
    public class D2Livemain : MonoBehaviour
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

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick ()
        {
            Debug.Log("OK");
            Application.Quit();
        }

        public void OnChangeCameraButtonClick ()
        {
            webCamTextureMatSourceGetter.ChangeCamera ();
        }
    }
}