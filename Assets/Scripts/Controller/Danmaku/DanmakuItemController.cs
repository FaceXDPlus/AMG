using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class DanmakuItemController : MonoBehaviour
    {
        [SerializeField] public Dropdown DAction;
        [SerializeField] public Slider Duration;
        [SerializeField] public Button SetButton;
        [SerializeField] public Button ClearButton;

        [SerializeField] public Toggle InvertToggle;


        public GameObject Model;
        public string Name;
        public string UUID = "";
        public bool isAnimation = false;
        public bool isRecording = false;

        void Start()
        {
            DAction.onValueChanged.AddListener((int value) => { OnActionClicked(value); });
            SetButton.onClick.AddListener(OnSetButtonClick);
            ClearButton.onClick.AddListener(OnClearButtonClick);
            InvertToggle.onValueChanged.AddListener((bool value) => { OnInvertToggleValueChanged(value); });
            Duration.onValueChanged.AddListener((float value) => { OnDurationValueChanged(value); });
        }

        private void OnActionClicked(int value)
        {
            Name = DAction.options[value].text;
            if (Model.GetComponent<Live2DModelController>() != null)
            {
                var controller = Model.GetComponent<Live2DModelController>();
                if (controller.animationClips.Contains(DAction.options[value].text))
                {
                    isAnimation = true;
                }
                else
                {
                    isAnimation = false;
                }
            }
        }

        private void OnInvertToggleValueChanged(bool isOn)
        {

        }

        private void OnDurationValueChanged(float value)
        {

        }

        public void OnSetButtonClick()
        {

        }

        public void OnClearButtonClick()
        {
        }
    }
}
