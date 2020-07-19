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
        [SerializeField] public Toggle ModelAdvancedToggle;
        [SerializeField] private GameObject ModelAdvancedPanel;
        [SerializeField] private ModelAdvancedController ModelAdvancedController;

        //WebSocket部分
        [SerializeField] public Toggle WebSocketToggle;
        [SerializeField] private WSHelper WebSocketHelper;

        //快捷键窗口部分
        [SerializeField] public Toggle ShortcutToggle;
        //[SerializeField] private GameObject ShortcutPanel;
        [SerializeField] private GameObject NewShortcutPanel;

        //DX透明度
        [SerializeField] public Camera DXCamera;
        [SerializeField] public DXTransparentHelper DXTransparentHelper;

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

        public void OnDXWindowToggleClick(Toggle toggle)
        {
            bool windowOn = dxInterface.ToggleShowWindow(false);
            toggle.isOn = windowOn;
#if UNITY_STANDALONE_WIN
            if (windowOn)
            {
                DXTransparentHelper.SetMinimized("AMG DX Windows");
            }
            else
            {
                DXWindowTransparentToggle.isOn = false;
            }
#endif
        }

        public void OnDXWindowTransparentToggleClick(Toggle toggle)
        {

#if UNITY_STANDALONE_WIN
            if (dxInterface.windowOn)
            {
                if(toggle.isOn)
                {
                    DXCamera.backgroundColor = new Color(0,0,0,0);
                    DXTransparentHelper.SetTransparent("AMG DX Windows", DXTransparentHelper.enumWinStyle.WinTopAphaPenetrate);
                }
                else
                {
                    DXCamera.backgroundColor = new Color(0, 0, 0, 0);
                    DXTransparentHelper.SetTransparent("AMG DX Windows", DXTransparentHelper.enumWinStyle.WinNone);
                }
            }
            else
            {
                toggle.isOn = false;
            }
#endif
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
            NewShortcutPanel.SetActive(isOn);
        }

        #endregion

    }
}
