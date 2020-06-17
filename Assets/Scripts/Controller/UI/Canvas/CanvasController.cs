using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using UnityExtensions.Localization;


namespace AMG
{
    public class CanvasController : MonoBehaviour
    {
        [SerializeField] private Text LogText;
        [SerializeField] private Scrollbar LogScrollbar;
        [SerializeField] private LangController LangController;
        [SerializeField] private MainPanelController MainPanelController;

        void Start()
        {
            Globle.LangController = LangController;
            Globle.AddDataLog("Main", LangController.GetLang("LOG.StartVersion", Globle.APPVersion, Globle.APPBuild));

            var ipaddresses = Dns.GetHostAddresses(Dns.GetHostName());
            int i = 1;

            foreach (IPAddress ipaddress in ipaddresses)
            {
                if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    Globle.AddDataLog("Main", LangController.GetLang("LOG.AvailableIP", i.ToString(), ipaddress.ToString()));
                    i++;
                }
            }

            Globle.AddDataLog("Main", LangController.GetLang("LOG.SystemLoaded"));
            //自动启动WebSocket
            MainPanelController.WebSocketToggle.isOn = true;
        }

        void Update()
        {
            if (Globle.DataLog != "" && Globle.DataLog != null)
            {
                Log(Globle.DataLog);
                Globle.DataLog = null;
            }
        }

        public void Log(string text)
        {
            LogText.text += text;
            LogScrollbar.value = -0.0f;
        }

        public void CloseAllDropdown()
        {
            foreach (var box in GetComponentsInChildren<SelectionBoxConfig>())
            {
                CloseDropdown(box);
            }
        }

        private void CloseDropdown(SelectionBoxConfig box)
        {
            if (box.listLayer.activeSelf == true)
            {
                box.ContractList();
            }
        }

    }
}
