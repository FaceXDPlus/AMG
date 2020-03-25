using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace D2LiveManager
{
    public abstract class D2LiveAnimationController : D2LiveManagerProcess
    {

        #region D2LiveManagerProcess

        public override void Setup ()
        {
            //NullCheck (faceLandmarkGetterInterface, "faceLandmarkGetter");
        }

        public override void UpdateValue ()
        {
            
        }

        #endregion

        protected abstract void UpdateFaceAnimation ();

    }
}