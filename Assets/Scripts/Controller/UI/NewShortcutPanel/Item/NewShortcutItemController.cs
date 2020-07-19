using Live2D.Cubism.Core;
using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class NewShortcutItemController : MonoBehaviour
    {
        [SerializeField] public Dropdown DAction;
        [SerializeField] public Text Shortcut;
        [SerializeField] public Slider Duration;
        [SerializeField] public Button SetButton;
        [SerializeField] public Button ClearButton;
        [SerializeField] public Button DeleteButton;

        [SerializeField] public Toggle InvertToggle;
        [SerializeField] public Toggle LockToggle;
        [SerializeField] public Toggle LoopToggle;

        [SerializeField] private ShortcutController ShortcutController;
        [SerializeField] private GameObject ShortcutClassObject;
        [SerializeField] private GameObject ShortcutClassObjectParent;
        [SerializeField] private SettingPanelController SettingPanelController;

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
            DeleteButton.onClick.AddListener(OnDeleteButtonClick);
            InvertToggle.onValueChanged.AddListener((bool value) => { OnInvertToggleValueChanged(value); });
            LockToggle.onValueChanged.AddListener((bool value) => { OnLockToggleValueChanged(value); });
            LoopToggle.onValueChanged.AddListener((bool value) => { OnLoopToggleValueChanged(value); });
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
            if (UUID != "")
            {
                foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutController.ShortcutDict)
                {
                    foreach (KeyValuePair<string, ShortcutClass> kkvp in kvp.Value)
                    {
                        if (UUID == kkvp.Key)
                        {
                            if (isAnimation)
                            {
                                ShortcutController.ShortcutDict[kvp.Key][UUID].Parameter = "";
                                ShortcutController.ShortcutDict[kvp.Key][UUID].AnimationClip = Name;
                            }
                            else
                            {
                                ShortcutController.ShortcutDict[kvp.Key][UUID].Parameter = Name;
                                ShortcutController.ShortcutDict[kvp.Key][UUID].AnimationClip = "";
                            }
                            break;
                        }
                    }
                }
            }
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
            }
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

        public void OnDeleteButtonClick()
        {
            if (UUID != "")
            {
                ShortcutController.RemoveShortcutClass(UUID);
            }
            Destroy(gameObject);
        }


    }
}