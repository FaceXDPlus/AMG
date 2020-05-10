using Live2D.Cubism.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
    public class AMGKeyboardPairController : MonoBehaviour
    {
        public void RemoveKeyboardPair(CubismModel model)
        {
            var waitToRemove = new Dictionary<string, Dictionary<string, string>>();
            //uuid, <快捷键，动画>
            foreach (KeyValuePair<string, Dictionary<string, ShortcutClass>> kvp in Globle.KeyboardHotkeyDict)
            {
                foreach (KeyValuePair<string, ShortcutClass> kkvp in kvp.Value)
                {
                    if (kkvp.Value.Model == model)
                    {
                        UnityEngine.Debug.Log("Isset " + kkvp.Key);
                        var dd = new Dictionary<string, string>();
                        dd.Add(kvp.Key, kkvp.Value.AnimationClip);
                        waitToRemove.Add(kkvp.Key, dd);
                    }
                }
            }
            foreach (KeyValuePair<string, Dictionary<string, string>> kvp in waitToRemove)
            {
                foreach (KeyValuePair<string, string> kkvp in kvp.Value)
                {
                    UnityEngine.Debug.Log("Removed " + kvp.Key);
                    Globle.KeyboardHotkeyDict[kkvp.Key].Remove(kvp.Key);
                }
            }
        }

    }
}
