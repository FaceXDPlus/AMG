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
        [SerializeField] private GameObject ShortcutClassObject;
        [SerializeField] private GameObject ShortcutClassObjectParent;

        public CubismModel Model;
        public string Name;
        public string UUID = "";
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
            if (UUID != "")
            {
                ShortcutController.RemoveShortcutClass(UUID);
            }
            if (ShortcutController.isPressed.Count > 0)
            {
                var isPressed = ObjectCopier.Clone(ShortcutController.isPressed);
                var KeyboardPressedString = string.Join("+", isPressed.ToArray());
                Shortcut.text = KeyboardPressedString;
                var item = Instantiate(ShortcutClassObject);
                item.transform.SetParent(ShortcutClassObjectParent.transform, false);
                var sclass = item.GetComponent<ShortcutClass>();
                sclass.Model = Model;
                sclass.isCPressed = isPressed;
                sclass.isPressedText = KeyboardPressedString;
                sclass.MType = 0;
                if (isAnimation)
                {
                    sclass.Type = 0;
                    sclass.AnimationClip = Name;
                }
                else
                {
                    sclass.Type = 1;
                    sclass.Parameter = Name;
                }
                UUID = ShortcutController.SetShortcutClass(isPressed, sclass);
                item.SetActive(true);
            }
        }

        public void OnClearButtonClick()
        {
            Shortcut.text = "/";
            if (UUID != "")
            {
                ShortcutController.RemoveShortcutClass(UUID);
            }
        }

        public void BlendAnimationClips()
        {
            Model.GetComponent<Animation>().Blend(Name);
        }
    }
}