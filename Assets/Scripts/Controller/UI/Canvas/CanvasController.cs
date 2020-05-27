using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class CanvasController : MonoBehaviour
    {
        [SerializeField] private Text LogText;
        [SerializeField] private Scrollbar LogScrollbar;

        void Start()
        {
            var ipaddresses = Dns.GetHostAddresses(Dns.GetHostName());
            int i = 1;
            foreach (IPAddress ipaddress in ipaddresses)
            {
                if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    Globle.AddDataLog("[Main]发现第 " + i + " 个IP：" + ipaddress.ToString());
                    i++;
                }
            }
            Globle.AddDataLog("[Main]启动完成");
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
