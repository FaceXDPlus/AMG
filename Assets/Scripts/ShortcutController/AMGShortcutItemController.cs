using Live2D.Cubism.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class AMGShortcutItemController : MonoBehaviour
    {
        [SerializeField] public Text ItemAction;
        [SerializeField] public Text ItemShortcut;
        [SerializeField] public Button ItemButton;
        [SerializeField] public Button SetButton;
        [SerializeField] public Button ClrButton;
        private CubismModel Model;
        private string Name;
        private AMGModelController aMGodelController;

        public CubismModel cubismModel 
        {
            get { return Model; }
            set { Model = value; }
        }

        public string AnimationClip
        {
            get { return Name; }
            set { Name = value; }
        }

        public AMGModelController modelController
        {
            get { return aMGodelController; }
            set { aMGodelController = value; }
        }

        public void InitShortcutItem()
        {
            ItemButton.onClick.AddListener(BlendAnimationClips);
            SetButton.onClick.AddListener(BlendKeyboardHotkey);
            ClrButton.onClick.AddListener(RemoveKeyboardHotkey);
        }

        public void BlendAnimationClips()
        {
            Model.GetComponent<Animation>().Blend(Name);
        }

        public void BlendKeyboardHotkey()
        {
            Globle.KeyboardHookSetStart = true;
            Globle.HookSetModelName = Model.name;
            Globle.HookSetModelAnimationName = Name;
            Globle.HookSetController = this;
        }

        public void RemoveKeyboardHotkey()
        {
            var waitToRemove = new Dictionary<string, Dictionary<string, string>>();
            //uuid, <快捷键，动画>
            foreach (KeyValuePair<string, Dictionary<string, ShortcutClass>> kvp in Globle.KeyboardHotkeyDict)
            {
                foreach (KeyValuePair<string, ShortcutClass> kkvp in kvp.Value)
                {
                    if (kkvp.Value.Model == Model && kkvp.Value.AnimationClip == AnimationClip)
                    {
                        UnityEngine.Debug.Log("Isset " + kkvp.Key);
                        var dd = new Dictionary<string, string>();
                        dd.Add(kvp.Key, kkvp.Value.AnimationClip);
                        waitToRemove.Add(kkvp.Key, dd);
                    }
                }
            }
            foreach (KeyValuePair<string, Dictionary<string, string>> kvp in waitToRemove)
            {
                foreach (KeyValuePair<string, string> kkvp in kvp.Value)
                {
                    UnityEngine.Debug.Log("Removed " + kvp.Key);
                    Globle.KeyboardHotkeyDict[kkvp.Key].Remove(kvp.Key);
                }
            }
            ItemShortcut.text = "";
        }
    }
}
