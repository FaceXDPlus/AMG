using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class ShortcutItemController : MonoBehaviour
    {
        [SerializeField] public Text Action;
        [SerializeField] public Text Shortcut;
        [SerializeField] public Slider Duration;
        [SerializeField] public Button SetButton;
        [SerializeField] public Button ClearButton;
        public ShortcutItemHelper shortcutItemHelper;

        private void Start()
        {
            Duration.onValueChanged.AddListener((float value) => { OnDurationValueChanged(value); });
        }

        private void OnDurationValueChanged(float value)
        {

        }


    }
}