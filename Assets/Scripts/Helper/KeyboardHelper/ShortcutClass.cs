using Live2D.Cubism.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
    public class ShortcutClass : MonoBehaviour
    {
        public string AnimationClip;
        public string Parameter;
        public GameObject Model;
        public int Type = 0;
        //0动作 1Param
        public int Duration = 1;
        public bool InProgress = false;

        public void PlayAnimation()
        {
            if (Model.GetComponent<Live2DModelController>() != null)
            {
                Model.GetComponent<Live2DModelController>().Animation.Blend(AnimationClip);
            }
        }

        public void PlayParameter(bool isIncrease)
        {
            if (Model.GetComponent<Live2DModelController>() != null)
            {
                var paraC = Model.GetComponent<Live2DModelController>().InitedParameters[Parameter];
                var para = (CubismParameter)paraC.Parameter;
                var get = (paraC.MaxValue - paraC.MinValue) / 60;
                if (para.Value <= paraC.MinValue || para.Value >= paraC.MaxValue)
                {
                    InProgress = false;
                }
                if (isIncrease)
                {
                    para.Value = para.Value + get;
                }
                else
                {
                    para.Value = para.Value - get;
                }
            }
        }

        public void FixedUpdate()
        {
            if (InProgress)
            {
                PlayParameter(true);
            }
        }
    }
}
