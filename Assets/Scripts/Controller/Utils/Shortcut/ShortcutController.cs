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

        public string SetShortcutClass(List<string> keyPressed, ShortcutClass shortClass)
        {
            keyPressed.Sort(); 
            string id = System.Guid.NewGuid().ToString();
            shortClass.UUID = id;
            shortClass.ShortcutController = this;
            if (ShortcutDict.ContainsKey(keyPressed)){
                ShortcutDict[keyPressed].Add(id, shortClass);
            }
            else
            {
                var dict = new Dictionary<string, ShortcutClass>();
                dict.Add(id, shortClass);
                ShortcutDict.Add(keyPressed, dict);
            }
            return id;
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