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
        [SerializeField] public Toggle DXWindowTransparentToggle;
        [SerializeField] private DXHelper dxInterface;

        //模型细调部分
        [SerializeField] private Toggle ModelAdvancedToggle;
        [SerializeField] private GameObject ModelAdvancedPanel;
        [SerializeField] private ModelAdvancedController ModelAdvancedController;

        //WebSocket部分
        [SerializeField] public Toggle WebSocketToggle;
        [SerializeField] private WSHelper WebSocketHelper;

        //快捷键窗口部分
        [SerializeField] private Toggle ShortcutToggle;
        [SerializeField] private GameObject ShortcutPanel;


        //USB部分
        [SerializeField] private Toggle USBToggle;


        void Start()
        {
            DXWindowToggle.onValueChanged.AddListener((bool isOn) => { OnDXWindowToggleClick(DXWindowToggle); }); 
            DXWindowTransparentToggle.onValueChanged.AddListener((bool isOn) => { OnDXWindowTransparentToggleClick(DXWindowTransparentToggle); });
            ModelAdvancedToggle.onValueChanged.AddListener((bool isOn) => { OnModelAdvancedToggleClick(isOn); });
            WebSocketToggle.onValueChanged.AddListener((bool isOn) => { OnWebSocketToggleClick(isOn); });
            USBToggle.onValueChanged.AddListener((bool isOn) => { OnUSBToggleClick(isOn); });
            ShortcutToggle.onValueChanged.AddListener((bool isOn) => { OnShortcutToggleClick(isOn); });
        }

        #region UI

        public void OnDXWindowToggleClick(Toggle toggle) {
            bool windowOn = dxInterface.ToggleShowWindow(false);
            toggle.isOn = windowOn;
        }

        public void OnDXWindowTransparentToggleClick(Toggle toggle)
        {
            /*if (dxInterface.windowOn)
            {
                dxInterface.ToggleShowWindow(false);
                //Invoke("dxInterface.ToggleShowWindow(false)", 0.25f);
            }*/
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

        public void OnUSBToggleClick(bool isOn)
        {
            if (isOn)
            {
                WebSocketHelper.USBClientStart();
            }
            else
            {
                WebSocketHelper.USBClientStop();
            }
        }


        public void OnShortcutToggleClick(bool isOn)
        {
            ShortcutPanel.SetActive(isOn);
        }

        #endregion

    }
}
