using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

namespace AMG
{
    public class AMGInterceptKeys : MonoBehaviour
    {


        [DllImport("user32.dll")]   // 注意下面这三个函数和其他网上找到的 不一样 我把一些 IntPtr 参数以及返回值 改成了 int  (不改  编辑器以及mono打包是没问题的)
                                    //不改的话 il2cpp 会说参数C错误  不知道为什么 希望知道的大神告知 学习学习
        private static extern int SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 
        /// 模拟按键按下抬起
        /// 
        /// keybd_event((byte)65, 0, 0, 0);   按下F10
        /// keybd_event((byte)65, 0, 2, 0);   按下后松开F10
        /// 
        /// </summary>
        /// <param name="bVk"></param>
        /// <param name="bScan"></param>
        /// <param name="dwFlags">0为按下 2为释放</param>
        /// <param name="dwExtraInfo"></param>
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        //获取程序集模块的句柄
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        //全局钩子键盘为13
        private const int WH_KEYBOARD_LL = 13;

        //按下键
        private const int WM_KEYDOWN = 0x0100;
        //放开键
        private const int WM_KEYUP = 0x0101;

        private static LowLevelKeyboardProc _proc = HookCallback;

        private static int _hookID = 0;

        public void StartHook()
        {
            UnhookWindowsHookEx((IntPtr)_hookID);
            _hookID = SetHook(_proc);
        }

        public void StopHook()
        {
            UnhookWindowsHookEx((IntPtr)_hookID);
        }

        //安装Hook,用于截获键盘。
        private static int SetHook(LowLevelKeyboardProc proc)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, IntPtr.Zero, 0);
        }

        private delegate int LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);



        [MonoPInvokeCallback]
        private static int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //nCode>0表示此消息已由Hook函数处理了,不会交给Windows窗口过程处理了
            //nCode=0则表示消息继续传递给Window消息处理函数
            if (Globle.KeyboardHookStart == true)
            {

                try
                {
                    if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                    {
                        int vkCode = Marshal.ReadInt32(lParam);
                        string vsCode = vkCode.ToString();
                        if (Globle.supportedControlKey.Contains(vsCode) && !Globle.KeyboardPressed.Contains(vkCode))
                        {
                            Globle.KeyboardPressed.Add(vkCode);
                        }
                        else if (Globle.supportedKeyboardKey.Contains(vsCode))
                        {
                            var KeyboardPressed = ObjectCopier.Clone(Globle.KeyboardPressed);
                            Globle.KeyboardPressed = new ArrayList();
                            string keys = "";
                            foreach (int key in KeyboardPressed)
                            {
                                if (Globle.KeyTranslation.ContainsKey(key))
                                {
                                    keys = keys + Globle.KeyTranslation[key] + "+";
                                }
                                else
                                {
                                    keys = keys + key.ToString() + "+";
                                }
                                //UnityEngine.Debug.Log("ControlKeyboardPressedList:" + key);
                            }
                            if (Globle.KeyTranslation.ContainsKey(vkCode))
                            {
                                keys = keys + Globle.KeyTranslation[vkCode];
                            }
                            else
                            {
                                keys = keys + vsCode;
                            }
                            KeyboardPressed.Add(vkCode);
                            KeyboardPressed.Sort();
                            var strings = from object o in KeyboardPressed
                                          select o.ToString();
                            var KeyboardPressedString = string.Join(",", strings.ToArray());
                            if (Globle.KeyboardHookSetStart == true)
                            {
                                if (Globle.HookSetController != null)
                                {
                                    //处理快捷键
                                    var shortcutClass = new ShortcutClass();
                                    shortcutClass.AnimationClip = Globle.HookSetController.AnimationClip;
                                    shortcutClass.Model = Globle.HookSetController.cubismModel;
                                    shortcutClass.modelController = Globle.HookSetController.modelController;
                                    shortcutClass.KeyPressed = keys;
                                    Globle.HookSetController.ItemShortcut.text = keys;
                                    Globle.KeyboardHookSetStart = false;
                                    Globle.HookSetController = null;
                                    string id = System.Guid.NewGuid().ToString();
                                    if (!Globle.KeyboardHotkeyDict.ContainsKey(KeyboardPressedString))
                                    {
                                        var adddict = new Dictionary<string, ShortcutClass>();
                                        adddict.Add(id, shortcutClass);
                                        Globle.KeyboardHotkeyDict.Add(KeyboardPressedString, adddict);
                                    }
                                    else
                                    {
                                        var WaitToRemove = new Dictionary<string, Dictionary<string,string>>();
                                        foreach (KeyValuePair<string, Dictionary<string, ShortcutClass>> kvp in Globle.KeyboardHotkeyDict)
                                        {
                                            foreach (KeyValuePair<string, ShortcutClass> kkvp in kvp.Value)
                                            {
                                                if (kkvp.Value.AnimationClip == shortcutClass.AnimationClip)
                                                {
                                                    UnityEngine.Debug.Log("Isset " + kkvp.Key);
                                                    string iid = System.Guid.NewGuid().ToString();
                                                    var dd = new Dictionary<string, string>();
                                                    dd.Add(kvp.Key, kkvp.Key);
                                                    WaitToRemove.Add(iid, dd);
                                                }
                                            }
                                        }
                                        foreach (KeyValuePair<string, Dictionary<string, string>> kvp in WaitToRemove)
                                        {
                                            foreach (KeyValuePair<string, string> kkvp in kvp.Value)
                                            {
                                                Globle.KeyboardHotkeyDict[kkvp.Key].Remove(kkvp.Value);
                                            }
                                        }
                                        Globle.KeyboardHotkeyDict[KeyboardPressedString].Add(id, shortcutClass);
                                    }
                                }
                            }
                            else
                            {
                                if (Globle.KeyboardHotkeyDict.ContainsKey(KeyboardPressedString))
                                {
                                    //Globle.AddDataLog("喵");
                                    var kkk = Globle.KeyboardHotkeyDict[KeyboardPressedString];
                                    foreach (KeyValuePair<string, ShortcutClass> kvp in kkk)
                                    {
                                        UnityEngine.Debug.Log("Processing " + kvp.Key);
                                        kvp.Value.modelController.Animation.Blend(kvp.Value.AnimationClip);
                                    }
                                }
                            }
                        }

                    }
                    else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
                    {
                        int vkCode = Marshal.ReadInt32(lParam);
                        string vsCode = vkCode.ToString();
                        if (Globle.supportedControlKey.Contains(vsCode) && Globle.KeyboardPressed.Contains(vkCode))
                        {
                            Globle.KeyboardPressed.Remove(vkCode);
                        }
                    }
                }catch (Exception err)
                {
                    Globle.AddDataLog(err.Message + "" + err.StackTrace);
                }
            }
            return CallNextHookEx((IntPtr)_hookID, nCode, wParam, lParam);//传给下一个钩子
        }
    }
}