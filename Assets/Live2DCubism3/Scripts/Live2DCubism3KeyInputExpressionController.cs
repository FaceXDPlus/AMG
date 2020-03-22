using Live2D.Cubism.Core;
using UnityEngine;

namespace D2LiveManager.Live2DCubism3
{
    public class Live2DCubism3KeyInputExpressionController : D2LiveManagerProcess
    {
        [Header("[Target]")]

        public Animator target;

        protected int expressionIndex;


        #region D2LiveManagerProcess

        public override string GetDescription()
        {
            return "Update expression BlendTree of Live2DCubism3 using KeyInput.";
        }

        public override void Setup()
        {
            NullCheck(target, "target");

            expressionIndex = target.GetLayerIndex("Expression");
        }

        public override void UpdateValue()
        {
            if (target == null)
                return;

            float value = Mathf.Clamp01(target.GetFloat("Blend") - 0.5f);
            target.SetFloat("Blend", value);

            target.SetLayerWeight(expressionIndex, 0f);

            if (Input.GetKey(KeyCode.Z))
            {
                value = Mathf.Clamp01(target.GetFloat("Blend") - 1.0f);
                target.SetFloat("Blend", value);

                target.SetLayerWeight(expressionIndex, 1f);
            }

            if (Input.GetKey(KeyCode.X))
            {
                value = Mathf.Clamp01(target.GetFloat("Blend") + 1.0f);
                target.SetFloat("Blend", value);

                target.SetLayerWeight(expressionIndex, 1f);
            }

        }

        public override void LateUpdateValue()
        {

        }

        #endregion
    }
}

