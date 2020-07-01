using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRawInput;
using System.Linq;

namespace AMG
{
    public class ShortcutController : MonoBehaviour
    {
        private bool hookEnabled = false;
        //public Dictionary<string, ShortcutClass> ShortcutDict = new Dictionary<string, ShortcutClass>();
        public Dictionary<List<string>, Dictionary<string, ShortcutClass>> ShortcutDict = new Dictionary<List<string>, Dictionary<string, ShortcutClass>>();
        public List<string> isPressed = new List<string>();

        void Start()
        {
#if UNITY_STANDALONE_WIN
            RawKeyInput.Start(true);
            hookEnabled = true; 
            RawKeyInput.OnKeyUp += HandleKeyUp;
            RawKeyInput.OnKeyDown += HandleKeyDown;
#endif
        }

        private void HandleKeyUp(RawKey key) {
            if (isPressed.Contains(key.ToString()))
            {
                isPressed.Remove(key.ToString());
                isPressed.Sort();
            }
        }

        private void HandleKeyDown(RawKey key) {
            if (!isPressed.Contains(key.ToString()))
            {
                isPressed.Add(key.ToString());
                isPressed.Sort();
            }
        }

        public void SetShortcutClass(List<string> keyPressed, ShortcutClass shortClass, string UUID)
        {
            keyPressed.Sort();
            shortClass.UUID = UUID;
            shortClass.ShortcutController = this;
            /*if (IssetShortcutClass(UUID))
            {
                RemoveShortcutClass(UUID);

            }*/
            var KKeyPressed = IssetShortcutClassKeyPressed(keyPressed);
            if (KKeyPressed != null){
                if (ShortcutDict[KKeyPressed].ContainsKey(UUID))
                {
                    RemoveShortcutClass(UUID);
                }
                ShortcutDict[KKeyPressed].Add(UUID, shortClass);
            }
            else
            {
                var dict = new Dictionary<string, ShortcutClass>();
                dict.Add(UUID, shortClass);
                ShortcutDict.Add(keyPressed, dict);
            }
        }

        public void RemoveShortcutClass(string uuid)
        {
            foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutDict)
            {
                foreach (KeyValuePair<string, ShortcutClass> kkvp in kvp.Value)
                {
                    if (uuid == kkvp.Key)
                    {
                        Destroy(kkvp.Value.gameObject);
                        ShortcutDict[kvp.Key].Remove(uuid);
                        break;
                    }
                }
            }
        }

        public bool IssetShortcutClass(string uuid)
        {
            foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutDict)
            {
                foreach (KeyValuePair<string, ShortcutClass> kkvp in kvp.Value)
                {
                    if (uuid == kkvp.Key)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<string> IssetShortcutClassKeyPressed(List<string> keyPressed)
        {
            foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutDict)
            {
                if (kvp.Key.SequenceEqual(keyPressed))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        public void RemoveShortcutClassByModel(GameObject model)
        {
            var WaitDict = new List<string>();
            //清理快捷键
            foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutDict)
            {
                foreach (KeyValuePair<string, ShortcutClass> kkvp in ShortcutDict[kvp.Key])
                {
                    if (kkvp.Value.Model == model)
                    {
                        WaitDict.Add(kkvp.Key);
                    }
                }
            }
            foreach (string kkvp in WaitDict)
            {
                RemoveShortcutClass(kkvp);
            }
        }

        public bool GetContains(List<string> isCPressed)
        {
            foreach (string key in isCPressed)
            {
                if (!isPressed.Contains(key))
                {
                    return false;
                }
            }
            return true;
        }

        private void LateUpdate()
        {
            foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutDict)
            {
                if (GetContains(kvp.Key))
                {
                    foreach (KeyValuePair<string, ShortcutClass> kkvp in ShortcutDict[kvp.Key])
                    {
                        kkvp.Value.Play();
                    }
                }
            }
        }

        private void OnApplicationQuit()
        {
#if UNITY_STANDALONE_WIN
            RawKeyInput.Stop();
#endif
        }
    }
}