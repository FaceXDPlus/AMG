using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
    public class ParametersClass
    {
        //类型
        public string Type = "None";
        //参数名字
        public string Name = "None";
        //SDK名字
        public string SDKName = "None";
        public object Parameter = null;
        public float NowValue = 0;
        public float MinValue = 0;
        public float MinSetValue = 0;
        public float MinParamValue = 0;
        public float MaxValue = 0;
        public float MaxSetValue = 0;
        public float MaxParamValue = 0;
        public float SmoothValue = 0.5f;
    }

}