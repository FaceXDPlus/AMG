using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using MaterialUI;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class Globle : MonoBehaviour
    {
        public static string APPName    = "AMG";
        public static string APPVersion = "Alpha 0.8";
        public static string APPBuild   = "1";
        public static string APPHostName = Environment.GetEnvironmentVariable("computername");
        public static int ModelNum = 1;
        public static int IPNum = 0;
        public static Dictionary<string, string> ModelToIP;
        public static Dictionary<string, string> IPMessage;
        public static Dictionary<string, string> IPAlign;
        public static Dictionary<string, string> RemoteIPMessage;
        public static bool globleIPChanged = false;
        public static string DataLog = "\n软件版本：" + APPVersion + "，构建版本：" + APPBuild;


        public static bool KeyboardHookStart = true;
        public static bool KeyboardHookSetStart = false;
        public static ArrayList KeyboardPressed = new ArrayList();
        //储存已经按下的控制键
        public static ArrayList supportedControlKey = new ArrayList();
        public static ArrayList supportedKeyboardKey = new ArrayList();

        public static AMGShortcutItemController HookSetController;
        public static string HookSetModelName;
        public static string HookSetModelAnimationName;
        //public static Dictionary<string, string> KeyboardHotkeySetDict = new Dictionary<string, string>();

        public static Dictionary<ArrayList, Dictionary<string, string>> KeyboardHotkeyDict = new Dictionary<ArrayList, Dictionary<string, string>>();
        //组合键操作 模型展示名称 动画名称

        public static void AddDataLog(string data)
        {
            DataLog = DataLog +  "\n" + data;
        }
    }
    public class AMGMain : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera modelCamera;

        [SerializeField] private InputField ResolutionRatioX;
        [SerializeField] private InputField ResolutionRatioY;
        [SerializeField] private InputField DXResolutionRatioX;
        [SerializeField] private InputField DXResolutionRatioY;
        [SerializeField] private InputField targetFrameRate;

        [SerializeField] private InputField P2PField;
        [SerializeField] private Toggle P2PClientSwitch;

        [SerializeField] private Toggle SocketSwitch;
        [SerializeField] private Toggle UnityCamSwitch;
        [SerializeField] private Toggle DXWindowsSwitch;
        [SerializeField] private Toggle ShortcutSwitch;
        [SerializeField] private SelectionBoxConfig ModelDropdownBox;
        [SerializeField] private SelectionBoxConfig ControlModelDropdownBox;
        [SerializeField] private SelectionBoxConfig ControlIPDropdownBox;
        [SerializeField] private SelectionBoxConfig ModelPoseBox;

        [SerializeField] private DialogBoxConfig DialogBox;
        [SerializeField] private Text DialogBoxTitle;
        [SerializeField] private Text DialogBoxText;

        [SerializeField] private Text DebugLogText;
        [SerializeField] private Scrollbar DebugLogScrollbar;

        [SerializeField] private Slider ModelLevelSlider;
        [SerializeField] private Toggle ModelEyeballLRSwitch;

        [SerializeField] private Button ShortcutItem;
        [SerializeField] private VerticalLayoutGroup ShortcutGroup;
        [SerializeField] private GameObject ShortcutPanel;

        [SerializeField] private InputField APPHostNameField;

        private int FrameRate = 60;
        private ArrayList ModelList;
        private AMGController AMGController;
        private AMGShortcutController AMGShortcutController;
        private AMGSocketManager mySocketServer;
        private AMGSocketManager myP2PClient;
        private AMGInterceptKeys AMGInterceptKeysController;

        public AMGDXInterface dxInterface;

        private bool ipState = false;
        private void Awake()
        {
            Application.targetFrameRate = FrameRate;
        }

        void Start()
        {
            Application.targetFrameRate = FrameRate;

            ModelList = new ArrayList();
            Globle.ModelToIP = new Dictionary<string, string>();
            Globle.IPAlign = new Dictionary<string, string>();
            Globle.IPMessage = new Dictionary<string, string>();
            Globle.RemoteIPMessage = new Dictionary<string, string>();

            AMGController = new AMGController();
            AMGShortcutController = new AMGShortcutController();
            //初始化控制器
            AMGController.setModelDropdownBox(ModelDropdownBox);
            AMGController.setControlModelDropdownBox(ControlModelDropdownBox);
            AMGController.setControlIPDropdownBox(ControlIPDropdownBox);
            AMGController.setDialogBoxConfig(DialogBox, DialogBoxTitle, DialogBoxText);
            AMGController.InitDropdownBoxs();
            AMGController.RefreshModelDropdown();
            //初始化快捷键管理
            AMGShortcutController.setVerticalLayoutGroup(ShortcutGroup);
            AMGShortcutController.setExampleButton(ShortcutItem);

            ControlModelDropdownBox.ItemPicked += OnControlModelSelected;
            ControlIPDropdownBox.ItemPicked += OnControlIPSelected;

            APPHostNameField.text = Globle.APPHostName;
            ((InputFieldConfig)APPHostNameField.GetComponent<InputFieldConfig>()).displayText.text = Globle.APPHostName;

            AMGInterceptKeysController = new AMGInterceptKeys();
            InitKeyboardControls();
            InitKeyboardKeys();
            //AMGInterceptKeysController.StartHook();

            mySocketServer = new AMGSocketManager();
            mySocketServer.setSocketSwitch(SocketSwitch);
            myP2PClient = new AMGSocketManager();
            myP2PClient.setP2PClientSwitch(P2PClientSwitch);
        }

        public void InitKeyboardControls()
        {
            Globle.supportedControlKey.Add("18");//左边Alt
            Globle.supportedControlKey.Add("160");//左边Shift
            Globle.supportedControlKey.Add("162");//左边Ctrl
            Globle.supportedControlKey.Add("20");//左边Caps
            Globle.supportedControlKey.Add("93");//右边目录
            Globle.supportedControlKey.Add("192");//左边`
        }

        public void InitKeyboardKeys()
        {
            for (int i = 65; i <= 90;++i)
            {
                Globle.supportedKeyboardKey.Add(i.ToString());
            }
            //65-90 a-z
        }

        public void onFrameRateButtonPressed()
        {
            FrameRate = Convert.ToInt32(targetFrameRate.text);
            Application.targetFrameRate = Convert.ToInt32(targetFrameRate.text);
        }

        public void onHostNameButtonPressed()
        {
            Globle.APPHostName = APPHostNameField.text;
        }

        private void CloseAllDropdown()
        {
            closeDropdown(ModelDropdownBox);
            closeDropdown(ControlIPDropdownBox);
            closeDropdown(ControlModelDropdownBox);
        }

        private void closeDropdown(SelectionBoxConfig box)
        {
            if (box.listLayer.activeSelf == true)
            {
                box.ContractList();
            }
        }

        public void onUnityCamSwitchSwitched()
        {
            modelCamera.enabled = UnityCamSwitch.isOn;
        }

        public void onShortcutSwitchSwitched()
        {
            ShortcutPanel.gameObject.SetActive(ShortcutSwitch.isOn);
        }

        public void onDXWindowsSwitchSwitched()
        {
            bool windowOn = dxInterface.ToggleShowWindow();
            DXWindowsSwitch.isOn = windowOn;
            //buttonText.text = (windowOn ? closeWindowPlugin : showWindowPlugin);
        }

        public void onSocketSwitchSwitched()
        {
            if (SocketSwitch.isOn == true)
            {
                mySocketServer.SocketStart();
            }
            else
            {
                mySocketServer.SocketStop();
                Globle.IPMessage = new Dictionary<string, string>();
                Globle.ModelToIP = new Dictionary<string, string>();
                //Debug.Log("关闭");
                //UnityEngine.Object.Destroy(base.gameObject.GetComponent<AMGSocketManager>());
            }
        }

        public void onP2PClientSwitchSwitched()
        {
            if (P2PClientSwitch.isOn == true && P2PField.text != "")
            {
                myP2PClient.P2PClientStart(P2PField.text);

            }
            else
            {
                if (this.myP2PClient.P2PClientStatus == true)
                {
                    Globle.AddDataLog("[WSC]客户连接关闭中");
                    foreach (KeyValuePair<string, string> kvp in Globle.RemoteIPMessage)
                    {
                        if (kvp.Key.IndexOf("回调") > 0)
                        {
                            Globle.RemoteIPMessage.Remove(kvp.Key);
                        }
                    }
                    Globle.globleIPChanged = true;
                    myP2PClient.P2PClientStop();
                }
                //Debug.Log("关闭");
            }
        }

        private void OnControlModelSelected(int id)
        {
            var selected = ControlModelDropdownBox.listItems[id];
            foreach (CubismModel Model in ModelList)
            {
                if (Model.name == selected)
                {
                    CloseAllDropdown();
                    Log("\n[Main]切换到模型 " + selected);
                    ModelEyeballLRSwitch.isOn = Model.GetComponent<AMGModelController>().changeEyeBallLR;
                    ModelLevelSlider.value = Model.gameObject.GetComponent<CubismRenderController>().SortingOrder;
                    var ModelToIP = Globle.ModelToIP;
                    if (ModelToIP.ContainsKey(selected))
                    {
                        string slove = string.Empty;
                        if (ModelToIP.TryGetValue(selected, out slove))
                        {
                            ControlIPDropdownBox.selectedText.text = slove;
                        }
                        else
                        {
                            ControlIPDropdownBox.selectedText.text = "选择控制IP";
                        }
                    }
                    else
                    {
                        ControlIPDropdownBox.selectedText.text = "选择控制IP";
                    }

                    AMGShortcutController.refreshVerticalLayoutGroup();
                    foreach (string name in Model.GetComponent<AMGModelController>().animationClips)
                    {
                        AMGShortcutController.addVerticalLayoutGroupItem(name, Model);
                    }
                    //animation.Blend(animationClip.name);
                }
            }
        }

        private void OnControlIPSelected(int id)
        {
            var selected = ControlIPDropdownBox.listItems[id];
            var model = AMGController.GetModelFromDropdown(ModelList);
            if (selected != "无")
            {
                var modelname = ControlModelDropdownBox.selectedText.text;
                if (Globle.IPMessage.ContainsKey(selected) || Globle.RemoteIPMessage.ContainsKey(selected))
                {
                    if (model != null)
                    {
                        this.Log("\n[Main]设置IP " + selected + " 给模型" + modelname);
                        if (Globle.ModelToIP.ContainsKey(modelname))
                        {
                            Globle.ModelToIP[modelname] = selected;
                        }
                        else
                        {
                            Globle.ModelToIP.Add(modelname, selected);
                        }
                    }
                }
                else
                {
                    AMGController.RefreshControlIPDropdown();
                    this.Log("\n[Main]选择IP出现错误");
                }
            }
            else
            {
                ControlIPDropdownBox.currentSelection = -1;
                ControlIPDropdownBox.selectedText.text = "选择控制IP";
                this.Log("\n[Main]未选择控制器");
            }
        }

        public void onModelAlignButtonPressed()
        {
            var model = AMGController.GetModelFromDropdown(ModelList);
            if (model != null)
            {
                model.gameObject.GetComponent<AMGModelController>().setParameterAlign();
            }
            else
            {
                this.Log("\n[Main]未选择控制器");
            }
        }

        public void onModelEyeBallLRSwitchPressed()
        {
            var model = AMGController.GetModelFromDropdown(ModelList);
            if (model != null)
            {
                model.gameObject.GetComponent<AMGModelController>().changeEyeBallLR = ModelEyeballLRSwitch.isOn;
            }
            else
            {
                this.Log("\n[Main]未选择控制器");
            }
        }

        public void onModelLevelSliderChanged()
        {
            var model = AMGController.GetModelFromDropdown(ModelList);
            if (model != null)
            {
                model.gameObject.GetComponent<CubismRenderController>().SortingOrder = (int)ModelLevelSlider.value;
            }
            else
            {
                this.Log("\n[Main]未选择控制器");
            }
        }

        public void onModelRefreshButtonPressed()
        {
            CloseAllDropdown();
            Invoke("ModelRefreshButtonPressed", 0.25f);

        }

        public void onModelDeleteButtonPressed()
        {
            CloseAllDropdown();
            Invoke("ModelDeleteButtonPressed", 0.25f);
        }

        public void onModelAddButtonPressed()
        {
            CloseAllDropdown();
            Invoke("ModelAddButtonPressed", 0.25f);
        }

        private void ModelAddButtonPressed()
        {
            var model = AMGController.AddModelFromDropdown();
            if (model != null)
            {
                model.GetComponent<AMGModelController>().setControlDropdownBox(ControlModelDropdownBox);
                model.GetComponent<AMGModelController>().DisplayName = model.name;
                ModelList.Add(model);
                AMGController.RefreshControlDropdown(ModelList);
                AMGController.RefreshControlIPDropdown();
                ResetModelControllers();
            }
        }

        private void ModelDeleteButtonPressed()
        {
            foreach (CubismModel Model in ModelList)
            {
                if (ControlModelDropdownBox.selectedText.text == Model.name)
                {
                    ModelList.Remove(Model);
                    UnityEngine.Object.Destroy(Model.gameObject.GetComponent<AMGModelController>());
                    UnityEngine.Object.Destroy(Model.gameObject);
                    AMGController.RefreshControlDropdown(ModelList);
                    return;
                }
            }
        }

        private void ModelRefreshButtonPressed()
        {
            AMGController.RefreshModelDropdown();
        }

        public void ResetModelControllers()
        {
            ModelEyeballLRSwitch.isOn = false;
            ModelLevelSlider.value = 0;
        }

        public void onResolutionRatioButtonPressed()
        {
            Screen.SetResolution(Convert.ToInt32(ResolutionRatioX.text), Convert.ToInt32(ResolutionRatioY.text), false);
        }

        public void onDXResolutionRatioButtonPressed()
        {
            gameObject.GetComponent<AMGDXInterface>().renderTextureHeight = Convert.ToInt32(DXResolutionRatioY.text);
            gameObject.GetComponent<AMGDXInterface>().renderTextureWidth = Convert.ToInt32(DXResolutionRatioX.text);
            if (DXWindowsSwitch.isOn == true)
            {
                DXWindowsSwitch.isOn = false;
            }
        }

        public void OnSaveButtonClick()
        {
            foreach (CubismModel model in ModelList)
            {
                model.GetComponent<AMGModelController>();
            }
        }

        public void Update()
        {
            if (Globle.DataLog != "" && Globle.DataLog != null)
            {
                this.Log(Globle.DataLog);
                Globle.DataLog = null;
            }
            OnCheckIPMessageDropdown();
            OnBindMessageAndIP();
        }

        public void FixedUpdate()
        {
            OnSendIPMessageToHost();
        }

        public class ArrayInfo
        {
            public string hostName { get; set; }
            public ArrayList keyboardAttached { get; set; }
            public Dictionary<string, string> ipMessage { get; set; }
        }

        public void OnSendIPMessageToHost()
        {
            if (P2PField.text != "" && P2PClientSwitch.isOn == true && myP2PClient.P2PClientStatus == true)
            {
                try
                {
                    var ipmessage = ObjectCopier.Clone(Globle.IPMessage);
                    var ipmessage1 = ObjectCopier.Clone(Globle.IPMessage);
                    var ipalign = ObjectCopier.Clone(Globle.IPAlign);
                    foreach (KeyValuePair<string, string> kvp in ipmessage)
                    {
                        if (ipalign.ContainsKey(kvp.Key))
                        {
                            var oldString = ipmessage[kvp.Key].Substring(0, ipmessage[kvp.Key].Length - 1);
                            oldString = oldString + ipalign[kvp.Key] + "}";
                            //var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(oldString);
                            ipmessage1[kvp.Key] = oldString;
                        }
                    }
                    ArrayInfo arrayInfo = new ArrayInfo
                    {
                        hostName = Globle.APPHostName,
                        keyboardAttached = new ArrayList(),
                        ipMessage = ipmessage1
                    };
                    byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(arrayInfo));
                    myP2PClient.P2PClientSendBinary(byteArray);
                }
                catch (Exception ex)
                {
                    var log = "[WSC]客户连接发生错误 " + ex.Message + ":" + ex.StackTrace;
                    //var log = "[WSC]客户连接发生错误 " + ex.Message ;
                    Globle.AddDataLog(log);
                }
            }
        }

        public void OnCheckIPMessageDropdown()
        {
            if ((Globle.IPNum != Globle.IPMessage.Count && !ipState) || (Globle.globleIPChanged && !ipState))
            {
                Log("\n[Main]刷新现有IP列表");
                CloseAllDropdown();
                ipState = true;
                Globle.globleIPChanged = false;
                Invoke("CheckIPMessageDropdown", 0.25f);
            }
        }

        private void CheckIPMessageDropdown()
        {
            AMGController.RefreshControlIPDropdown();
            Globle.IPNum = Globle.IPMessage.Count;
            ipState = false;
        }

        public void OnBindMessageAndIP()
        {
            var ModelToIP = Globle.ModelToIP;
            var IPMessage = Globle.IPMessage;
            var RemoteIPMessage = Globle.RemoteIPMessage;
            foreach (KeyValuePair<string, string> kvp in ModelToIP)
            {
                if ((IPMessage.ContainsKey(kvp.Value) || RemoteIPMessage.ContainsKey(kvp.Value)))
                {
                    try
                    {
                        foreach (CubismModel Model in ModelList)
                        {
                            if (Model.GetComponent<AMGModelController>().DisplayName == kvp.Key)
                            {
                                //AMGController.GetStringJson(IPMessage[kvp.Value], Model);
                                if (IPMessage.ContainsKey(kvp.Value))
                                {
                                    if (IPMessage[kvp.Value] != null)
                                    {
                                        AMGController.DoJsonPrase(IPMessage[kvp.Value], Model);
                                        if (Globle.IPAlign.ContainsKey(kvp.Value))
                                        {
                                            Globle.IPAlign[kvp.Value] = Model.GetComponent<AMGModelController>().getParameterAlign();
                                        }
                                        else
                                        {
                                            Globle.IPAlign.Add(kvp.Value, Model.GetComponent<AMGModelController>().getParameterAlign());
                                        }
                                    }
                                }
                                else
                                {
                                    if (RemoteIPMessage[kvp.Value] != null)
                                    {
                                        AMGController.DoJsonPrase(RemoteIPMessage[kvp.Value], Model);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Log("\n[Main]发生错误 " + ex.Message + ":" + ex.StackTrace);
                    }
                }
            }
        }

        public void OnQuitButtonClick()
        {
            Application.Quit();
        }

        public void Log(string text)
        {
            //Text Text = GameObject.Find("日志文本").GetComponent<Text>();
            DebugLogText.text = DebugLogText.text + text;
            //Scrollbar Scrollbar = GameObject.Find("日志下拉").GetComponent<Scrollbar>();
            DebugLogScrollbar.value = -0.0f;
        }

        public void OnApplicationQuit()
        {
            AMGInterceptKeysController.StopHook();
        }
    }
}
