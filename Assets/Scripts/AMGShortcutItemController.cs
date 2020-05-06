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
        private CubismModel Model;
        private string Name;

        public void SetModel(CubismModel model)
        {
            Model = model;
        }

        public void SetAnimationName(string text)
        {
            Name = text;
        }
        public void InitShortcutItem()
        {
            ItemButton.onClick.AddListener(BlendAnimationClips);
            SetButton.onClick.AddListener(BlendKeyboardHotkey);
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
            //Globle.
            //Model.GetComponent<Animation>().Blend(Name);
        }
    }
}
