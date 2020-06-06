using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class ModelAdvancedItemController : MonoBehaviour
    {
        [SerializeField] public Text ParameterName;
        [SerializeField] public Slider MinSlider;
        [SerializeField] public Slider MaxSlider;
        [SerializeField] public Slider NowSlider;
        public ParametersClass parametersClass;

        private void Start()
        {
            MinSlider.onValueChanged.AddListener((float value) => { OnMinSliderValueChanged(value); });
            MaxSlider.onValueChanged.AddListener((float value) => { OnMaxSliderValueChanged(value); });
        }

        private void Update()
        {
            NowSlider.value = parametersClass.NowValue;
        }

        private void OnMinSliderValueChanged(float value)
        {
            if (value <= MaxSlider.value)
            {
                parametersClass.MinSetValue = value;
            }
            else
            {
                MinSlider.value = MaxSlider.value;
            }
        }

        private void OnMaxSliderValueChanged(float value)
        {
            if (value >= MinSlider.value)
            {
                parametersClass.MaxSetValue = value;
            }
            else
            {
                MaxSlider.value = MinSlider.value;
            }
        }
    }
}