using Live2D.Cubism.Core;
using System;

namespace AMG
{
    public class ShortcutClass
    {
        private CubismModel model = null;
        private string animationClip = "";
        private string keyPressed = "";
        private AMGModelController aMGodelController = null;
        private bool isLoop = false;

        public string AnimationClip
        {
            get { return animationClip; }
            set { animationClip = value; }
        }

        public string KeyPressed
        {
            get { return keyPressed; }
            set { keyPressed = value; }
        }


        public CubismModel Model
        {
            get { return model; }
            set { model = value; }
        }

        public AMGModelController modelController
        {
            get { return aMGodelController; }
            set { aMGodelController = value; }
        }
    }

}
