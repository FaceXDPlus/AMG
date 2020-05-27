using System;
using System.Collections;
using System.Collections.Generic;

namespace AMG
{
    public class Globle
    {
        public static string APPName = "AMG-RM";
        public static string APPVersion = "Alpha 0.1";
        public static string APPBuild = "1";

        public static int ModelNum = 1;
        //public static string APPHostName = GetComputerName();
        //public static int ModelNum = 1;
        //public static int IPNum = 0;
        //public static Dictionary<string, string> ModelToIP;
        //public static Dictionary<string, string> IPMessage;
        //public static Dictionary<string, string> IPAlign;
        //public static Dictionary<string, string> RemoteIPMessage;
        //public static bool globleIPChanged = false;
        public static string DataLog = "\n[Main] 软件版本：" + APPVersion + "，构建版本：" + APPBuild;


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

}
