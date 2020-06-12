using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AMG
{
    public class ShortcutClass : MonoBehaviour
    {
        public string AnimationClip;
        public string Parameter;
        public string UUID;
        public string isPressedText;

        public object Model;
        public int MType = 0;
        //0 Live2D 1 VRM
        public int Type = 0;
        //0动作 1Param
        //public int Duration = 1;
        //public bool InProgress = false;
        public List<string> isCPressed;
        public ShortcutController ShortcutController;

        public void Play()
        {
            switch(MType) {
                case 0:
                    switch (Type)
                    {
                        case 0:
                            PlayAnimation();
                            break;
                        case 1:
                            PlayParameter(true);
                            break;
                    }
                    break;
                case 1:
                    //VRM
                    break;
            }
        }

        public void PlayAnimation()
        {
            var model = (CubismModel)Model;
            if (model.GetComponent<Live2DModelController>() != null)
            {
                model.GetComponent<Live2DModelController>().Animation.Blend(AnimationClip);
            }
        }

        private void LateUpdate()
        {
            switch (MType)
            {
                case 0:
                    switch (Type)
                    {
                        case 0:
                            break;
                        case 1:
                            if (!ShortcutController.GetContains(isCPressed))
                            {
                                PlayParameter(false);
                            }
                            break;
                    }
                    break;
                case 1:
                    //VRM
                    break;
            }
        }

        public void PlayParameter(bool isIncrease)
        {
            var model = (CubismModel)Model;
            //加入FixedUpdate，如果不操作就归0
            if (model.GetComponent<Live2DModelController>() != null)
            {
                var paraC = model.Parameters.FindById(Parameter);
                if (paraC != null)
                {
                    var get = (paraC.MaximumValue - paraC.MinimumValue) / 60;
                    if (isIncrease)
                    {
                        if (paraC.Value < paraC.MaximumValue)
                        {
                            var now = paraC.Value + get;
                            model.GetComponent<Live2DModelController>().setParameter(paraC, now, paraC.MinimumValue, paraC.MaximumValue, paraC.MinimumValue, paraC.MaximumValue);
                        }
                    }
                    else
                    {
                        if (paraC.Value > paraC.MinimumValue)
                        {
                            var now = paraC.Value - get;
                            model.GetComponent<Live2DModelController>().setParameter(paraC, now, paraC.MinimumValue, paraC.MaximumValue, paraC.MinimumValue, paraC.MaximumValue);
                        }
                    }
                }
            }
        }
    }
}
