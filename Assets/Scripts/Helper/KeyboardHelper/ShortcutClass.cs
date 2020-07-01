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

        public GameObject Model;
        public int MType = 0;
        //0 Live2D 1 VRM
        public int Type = 0;
        //0动作 1Param
        //public int Duration = 1;
        //public bool InProgress = false;


        public bool IsInvert = false;
        public bool IsLock = false;
        public bool IsLoop = false;
        public bool IsLoopRun = false;
        public bool LoopLock = false;
        public float fps = 60;
        public bool InProgress = false;
        public bool InProgressIncrease = true;

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
                            if (!IsLock)
                            {
                                PlayParameter(!IsInvert);
                            }
                            else if (IsLock && !InProgress)
                            {
                                InProgress = true;
                            }
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
            var model = Model.FindCubismModel();
            if (model.GetComponent<Live2DModelController>() != null)
            {
                var animation = model.GetComponent<Live2DModelController>().Animation;
                if (IsLoop && !IsLoopRun && !LoopLock)
                {
                    var nn = animation.GetClip(AnimationClip);
                    animation.RemoveClip(nn);
                    nn.wrapMode = WrapMode.Loop;
                    animation.AddClip(nn, nn.name);
                    animation.Blend(AnimationClip);
                    IsLoopRun = true;
                    LoopLock = true;
                    Invoke("UnlockLoop", 1f);
                }
                else if (IsLoop && IsLoopRun && !LoopLock)
                {
                    animation.Stop(AnimationClip);
                    IsLoopRun = false;
                    LoopLock = true;
                    Invoke("UnlockLoop", 1f);
                }
                else if (!IsLoop) 
                {
                    var nn = animation.GetClip(AnimationClip);
                    animation.RemoveClip(nn);
                    nn.wrapMode = WrapMode.Once;
                    animation.AddClip(nn, nn.name);
                    animation.Blend(AnimationClip);
                }
            }
        }

        private void UnlockLoop()
        {
            LoopLock = false;
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
                            if (!IsLock)
                            {
                                if (!ShortcutController.GetContains(isCPressed))
                                {
                                    PlayParameter(IsInvert);
                                }
                            }
                            else if (IsLock && InProgress)
                            {
                                PlayParameter(InProgressIncrease);
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
            var model = Model.FindCubismModel();
            //加入FixedUpdate，如果不操作就归0
            if (model.GetComponent<Live2DModelController>() != null)
            {
                var paraC = model.Parameters.FindById(Parameter);
                if (paraC != null)
                {
                    var get = (paraC.MaximumValue - paraC.MinimumValue) / fps;
                    var now = 0f;
                    if (isIncrease)
                    {
                        if (paraC.Value < paraC.MaximumValue)
                        {
                            now = paraC.Value + get;
                            model.GetComponent<Live2DModelController>().setParameter(paraC, now, paraC.MinimumValue, paraC.MaximumValue, paraC.MinimumValue, paraC.MaximumValue);
                        }
                    }
                    else
                    {
                        if (paraC.Value > paraC.MinimumValue)
                        {
                            now = paraC.Value - get;
                            model.GetComponent<Live2DModelController>().setParameter(paraC, now, paraC.MinimumValue, paraC.MaximumValue, paraC.MinimumValue, paraC.MaximumValue);
                        }
                    }
                    if (IsLock && InProgress)
                    {
                        if (now >= paraC.MaximumValue)
                        {
                            InProgress = false;
                            InProgressIncrease = false;
                            //Debug.Log("Stop " + now + InProgressIncrease);
                        }
                        else if (now <= paraC.MinimumValue)
                        {
                            InProgress = false;
                            InProgressIncrease = true;
                            //Debug.Log("Stop " + now + InProgressIncrease);
                        }
                        //Debug.Log("rua " + now + InProgressIncrease);
                    }
                }
            }
        }
    }
}
