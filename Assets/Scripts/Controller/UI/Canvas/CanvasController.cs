using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
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
        [SerializeField] private SettingPanelController SettingPanelController;

        public static Storage MainStorage;

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

            Application.targetFrameRate = 60;
            //自动启动WebSocket
            Load();
            MainPanelController.WebSocketToggle.isOn = true;
            SettingPanelController.ResolutionRatioX.text = MainStorage.ResolutionRatioX.ToString();
            SettingPanelController.ResolutionRatioY.text = MainStorage.ResolutionRatioY.ToString();
            GetComponent<DXHelper>().renderTextureHeight = MainStorage.ResolutionRatioY;
            GetComponent<DXHelper>().renderTextureWidth = MainStorage.ResolutionRatioX;
        }

        public struct Storage
        {
            public int ResolutionRatioX;//Width
            public int ResolutionRatioY;//Width
        };

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        public void Save()
        {
            var serializer = new XmlSerializer(typeof(Storage));
            var stream = new FileStream(Application.streamingAssetsPath + "/Storage", FileMode.Create);

            using (stream)
            {
                serializer.Serialize(stream, MainStorage);
            }
        }

        public void Load()
        {
            var serializer = new XmlSerializer(typeof(Storage));
            if (File.Exists(Application.streamingAssetsPath + "/Storage"))
            {
                var stream = new FileStream(Application.persistentDataPath + "/Storage", FileMode.Open);


                using (stream)
                {
                    MainStorage = (Storage)serializer.Deserialize(stream);
                }
            }
            else
            {
                MainStorage.ResolutionRatioX = 1280;
                MainStorage.ResolutionRatioX = 720;
            }
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

        private void OnApplicationQuit()
        {
            Save();
        }
    }
}
