﻿using System;
using UnityEngine;

namespace D2LiveManager
{
    [System.AttributeUsage (System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class InterfaceRestrictionAttribute : PropertyAttribute
    {
        public Type type;

        public InterfaceRestrictionAttribute (Type type)
        {
            this.type = type;
        }
    }
}