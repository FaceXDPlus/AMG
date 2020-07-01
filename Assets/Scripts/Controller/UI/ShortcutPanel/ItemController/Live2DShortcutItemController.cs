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

        [SerializeField] public Toggle InvertToggle;
        [SerializeField] public Toggle LockToggle;
        [SerializeField] public Toggle LoopToggle;

        [SerializeField] private ShortcutController ShortcutController;
        [SerializeField] private GameObject ShortcutClassObject;
        [SerializeField] private GameObject ShortcutClassObjectParent;

        public GameObject Model;
        public string Name;
        public string UUID = "";
        public bool isAnimation = false;

        private void Start()
        {
            Duration.onValueChanged.AddListener((float value) => { OnDurationValueChanged(value); });
            ItemButton.onClick.AddListener(OnItemButtonClick);
            SetButton.onClick.AddListener(OnSetButtonClick);
            ClearButton.onClick.AddListener(OnClearButtonClick);
            InvertToggle.onValueChanged.AddListener((bool value) => { OnInvertToggleValueChanged(value); });
            LockToggle.onValueChanged.AddListener((bool value) => { OnLockToggleValueChanged(value); });
            LoopToggle.onValueChanged.AddListener((bool value) => { OnLoopToggleValueChanged(value); });
        }

        private void OnLockToggleValueChanged(bool isOn)
        {
            if (UUID != "")
            {
                foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutController.ShortcutDict)
                {
                    foreach (KeyValuePair<string, ShortcutClass> kkvp in kvp.Value)
                    {
                        if (UUID == kkvp.Key)
                        {
                            ShortcutController.ShortcutDict[kvp.Key][UUID].IsLock = isOn;
                            break;
                        }
                    }
                }
            }
        }

        private void OnLoopToggleValueChanged(bool isOn)
        {
            if (UUID != "")
            {
                foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutController.ShortcutDict)
                {
                    foreach (KeyValuePair<string, ShortcutClass> kkvp in kvp.Value)
                    {
                        if (UUID == kkvp.Key)
                        {
                            ShortcutController.ShortcutDict[kvp.Key][UUID].IsLoop = isOn;
                            break;
                        }
                    }
                }
            }
        }

        private void OnInvertToggleValueChanged(bool isOn)
        {
            if (UUID != "")
            {
                foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutController.ShortcutDict)
                {
                    foreach (KeyValuePair<string, ShortcutClass> kkvp in kvp.Value)
                    {
                        if (UUID == kkvp.Key)
                        {
                            ShortcutController.ShortcutDict[kvp.Key][UUID].IsInvert = isOn;
                            break;
                        }
                    }
                }
            }
        }

        private void OnDurationValueChanged(float value)
        {
            if (UUID != "")
            {
                foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutController.ShortcutDict)
                {
                    foreach (KeyValuePair<string, ShortcutClass> kkvp in kvp.Value)
                    {
                        if (UUID == kkvp.Key)
                        {
                            ShortcutController.ShortcutDict[kvp.Key][UUID].fps = value;
                            break;
                        }
                    }
                }
            }
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
                    sclass.IsLoop = LoopToggle.isOn;
                }
                else
                {
                    sclass.Type = 1;
                    sclass.Parameter = Name;
                    sclass.IsInvert = InvertToggle.isOn;
                    sclass.IsLock = LockToggle.isOn;
                    sclass.fps = Duration.value;
                }
                UUID = System.Guid.NewGuid().ToString();
                ShortcutController.SetShortcutClass(isPressed, sclass, UUID);
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