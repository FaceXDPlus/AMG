using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class ModelPanel : MonoBehaviour
    {
        [SerializeField] private Toggle ModelEyeChangeToggle;
        [SerializeField] private Toggle ModelLostResetToggle;
        [SerializeField] private Slider ModelShowLevelSlider;
        [SerializeField] private SelectionBoxConfig ModelLostResetChooseDropdown;
        [SerializeField] private SelectionBoxConfig ModelLostResetActionDropdown;
        [SerializeField] private Button ModelAlignButton;

        public void SetValueFromModel(Live2dModelController controller)
        {

        }
    }
}
