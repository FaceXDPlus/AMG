using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class ModelAdvancedItemController : MonoBehaviour
    {
        [SerializeField] public Text ParameterName;
        [SerializeField] public Slider TMinSlider;
        [SerializeField] public Slider TMaxSlider;
        [SerializeField] public Slider TNowSlider;

        [SerializeField] public Slider MMinSlider;
        [SerializeField] public Slider MMaxSlider;
        [SerializeField] public Slider MNowSlider;

        [SerializeField] public Slider SSlider;
        public ParametersClass parametersClass;

        private void Start()
        {
            TMinSlider.onValueChanged.AddListener((float value) => { OnTMinSliderValueChanged(value); });
            TMaxSlider.onValueChanged.AddListener((float value) => { OnTMaxSliderValueChanged(value); });

            MMinSlider.onValueChanged.AddListener((float value) => { OnMMinSliderValueChanged(value); });
            MMaxSlider.onValueChanged.AddListener((float value) => { OnMMaxSliderValueChanged(value); });


            SSlider.onValueChanged.AddListener((float value) => { OnSSliderValueChanged(value); });
        }

        private void Update()
        {
            TNowSlider.value = parametersClass.NowValue;
            MNowSlider.value = parametersClass.ParametersValue;
        }

        private void OnTMinSliderValueChanged(float value)
        {
            if (value <= TMaxSlider.value)
            {
                parametersClass.MinSetValue = value;
            }
            else
            {
                TMinSlider.value = TMaxSlider.value;
            }
        }

        private void OnTMaxSliderValueChanged(float value)
        {
            if (value >= TMinSlider.value)
            {
                parametersClass.MaxSetValue = value;
            }
            else
            {
                TMaxSlider.value = TMinSlider.value;
            }
        }

        private void OnMMinSliderValueChanged(float value)
        {
            if (value <= MMaxSlider.value)
            {
                parametersClass.MinParamValue = value;
            }
            else
            {
                MMinSlider.value = MMaxSlider.value;
            }
        }

        private void OnMMaxSliderValueChanged(float value)
        {
            if (value >= MMinSlider.value)
            {
                parametersClass.MaxParamValue = value;
            }
            else
            {
                MMaxSlider.value = MMinSlider.value;
            }
        }

        private void OnSSliderValueChanged(float value)
        {
            parametersClass.SmoothValue = value;
        }

    }
}