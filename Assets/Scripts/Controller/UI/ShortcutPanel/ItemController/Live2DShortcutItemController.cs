using Live2D.Cubism.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace AMG
{
    public class Live2DShortcutItemController : MonoBehaviour
    {
        [SerializeField] public Text Action;
        [SerializeField] public Text Shortcut;
        [SerializeField] public Slider Duration;
        [SerializeField] public Button SetButton;
        [SerializeField] public Button ClearButton;
        [SerializeField] public Button ItemButton;
        [SerializeField] private ShortcutController ShortcutController;

        public CubismModel Model;
        public string Name;
        public bool isAnimation = false;

        private void Start()
        {
            Duration.onValueChanged.AddListener((float value) => { OnDurationValueChanged(value); });
            ItemButton.onClick.AddListener(OnItemButtonClick);
            SetButton.onClick.AddListener(OnSetButtonClick);
            ClearButton.onClick.AddListener(OnClearButtonClick);
        }

        private void OnDurationValueChanged(float value)
        {

        }

        private void OnItemButtonClick()
        {
            if (isAnimation)
            {
                BlendAnimationClips();
            }
            else
            {

            }
        }

        public void OnSetButtonClick()
        {
            var isPressed = ShortcutController.isPressed;
            var strings = from object o in isPressed
                          select o.ToString();
            var KeyboardPressedString = string.Join(",", strings.ToArray());
            Debug.Log(KeyboardPressedString);
            Shortcut.text = KeyboardPressedString;
        }

        public void OnClearButtonClick()
        {
            Shortcut.text = "";
        }

        public void BlendAnimationClips()
        {
            Model.GetComponent<Animation>().Blend(Name);
        }
    }
}