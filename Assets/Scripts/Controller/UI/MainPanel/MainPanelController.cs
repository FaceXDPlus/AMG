using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class MainPanelController : MonoBehaviour
    {
        //DX窗口部分
        [SerializeField] public Toggle DXWindowToggle;
        [SerializeField] private DXHelper dxInterface;

        //模型细调部分
        [SerializeField] private Toggle ModelAdvancedToggle;
        [SerializeField] private GameObject ModelAdvancedPanel;
        [SerializeField] private ModelAdvancedController ModelAdvancedController;

        //WebSocket部分
        [SerializeField] private Toggle WebSocketToggle;
        [SerializeField] private WSHelper WebSocketHelper;

        //快捷键窗口部分
        [SerializeField] private Toggle ShortcutToggle;
        [SerializeField] private GameObject ShortcutPanel;

        //模型选择下拉框&IP选择下拉框
        [SerializeField] private SelectionBoxConfig ModelSelectionDropdown;
        [SerializeField] private SelectionBoxConfig IPSelectionDropdown;

        //其他控制器
        [SerializeField] private ModelPanel ModelPanel;

        void Start()
        {
            DXWindowToggle.onValueChanged.AddListener((bool isOn) => { OnDXWindowToggleClick(DXWindowToggle); });
            ModelAdvancedToggle.onValueChanged.AddListener((bool isOn) => { OnModelAdvancedToggleClick(isOn); });
            WebSocketToggle.onValueChanged.AddListener((bool isOn) => { OnWebSocketToggleClick(isOn); });
            ShortcutToggle.onValueChanged.AddListener((bool isOn) => { OnShortcutToggleClick(isOn); });
            ModelSelectionDropdown.ItemPicked += OnControlModelSelected;
            IPSelectionDropdown.ItemPicked += OnIPSelectionBoxSelected;
        }

        #region UI

        public void OnDXWindowToggleClick(Toggle toggle) {
            bool windowOn = dxInterface.ToggleShowWindow();
            toggle.isOn = windowOn;
        }

        public void OnModelAdvancedToggleClick(bool isOn)
        {
            ModelAdvancedPanel.SetActive(isOn);
        }

        public void OnWebSocketToggleClick(bool isOn)
        {
            if (isOn)
            {
                WebSocketHelper.SocketStart();
            }
            else
            {
                WebSocketHelper.SocketStop();
            }
        }

        public void OnShortcutToggleClick(bool isOn)
        {
            ShortcutPanel.SetActive(isOn);
        }

        #endregion

        #region Model
        
        public string GetModelSelected()
        {
            return ModelSelectionDropdown.selectedText.text;
        }

        public void OnControlModelSelected(int id)
        {

        }

        public void OnIPSelectionBoxSelected(int id)
        {

        }


        #endregion
    }
}
