using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using System.Collections.Generic;

namespace D2Live
{
    /// <summary>
    /// CV VTuber Example
    /// </summary>
    public class D2LiveDlibCV : MonoBehaviour
    {

        public enum DlibShapePredictorNamePreset : int
        {
            sp_human_face_68
        }

        static DlibShapePredictorNamePreset dlibShapePredictorName = DlibShapePredictorNamePreset.sp_human_face_68;

        public static string dlibShapePredictorFileName {
            get {
                return dlibShapePredictorName.ToString () + ".dat";
            }
        }

        // Use this for initializatio
        // Update is called once per frame
        void Update ()
        {

        }

    }
}