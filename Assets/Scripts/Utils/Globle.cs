using Live2D.Cubism.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
    public class Globle
    {
        public static string APPName = "AMG-RM";
        public static string APPVersion = "Alpha 0.2";
        public static string APPBuild = "5";

        public static int ModelNum = 1;

        public static ArrayList ModelList = new ArrayList();

        //public static Dictionary<CubismModel, string> ModelDuidList = new Dictionary<CubismModel, string>();

        //uuid, WSClient
        public static Dictionary<string, WSClientClass> WSClients = new Dictionary<string, WSClientClass>();

        public static bool WSClientsChanged = false;

        public static string DataLog = "";
        public static LangController LangController = null;


        /*public static bool KeyboardHookStart = true;
        public static bool KeyboardHookSetStart = false;
        public static ArrayList KeyboardPressed = new ArrayList();
        public static Dictionary<string, string> KeyboardPressedDict = new Dictionary<string, string>();
        //储存已经按下的控制键
        public static ArrayList supportedControlKey = new ArrayList();
        public static ArrayList supportedKeyboardKey = new ArrayList();
        public static Dictionary<int, string> KeyTranslation = new Dictionary<int, string>();

        public static AMGShortcutItemController HookSetController;
        public static string HookSetModelName;
        public static string HookSetModelAnimationName;
        public static Dictionary<string, Dictionary<string, ShortcutClass>> KeyboardHotkeyDict = new Dictionary<string, Dictionary<string, ShortcutClass>>();
        //组合键操作 uuid, 控制类*/

        public static void AddDataLog(string from,string data)
        {
            DataLog = DataLog + "\n[" + from + "] " + data;
        }

        public static string GetComputerName()
        {
            return Environment.GetEnvironmentVariable("computername");
        }
    }

    [Serializable]
    public class WSClientClass
    {
        public string ip = "";
        public string message = "";
        public string uuid = "";
        public DateTime lastUpdated;
        public bool isRemote = false;
        public bool isUSB = false;

        public JObject result;
    }

}
