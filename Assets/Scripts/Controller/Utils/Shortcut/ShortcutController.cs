using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRawInput;

namespace AMG
{
    public class ShortcutController : MonoBehaviour
    {
        private bool hookEnabled = false;
        //public Dictionary<string, ShortcutClass> ShortcutDict = new Dictionary<string, ShortcutClass>();
        public Dictionary<ArrayList, Dictionary<string, ShortcutClass>> ShortcutDict = new Dictionary<ArrayList, Dictionary<string, ShortcutClass>>();
        public ArrayList isPressed = new ArrayList();

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
            if (isPressed.Contains(key))
            {
                isPressed.Remove(key);
                isPressed.Sort();
            }
        }

        private void HandleKeyDown(RawKey key) {
            if (!isPressed.Contains(key))
            {
                isPressed.Add(key);
                isPressed.Sort();
            }
        }

        public void SetShortcutClass(ArrayList keyPressed, ShortcutClass shortClass)
        {

        }

        public void RemoveShortcutClass()
        {

        }

        private void FixedUpdate()
        {
            
        }

        private void OnApplicationQuit()
        {
#if UNITY_STANDALONE_WIN
            RawKeyInput.Stop();
#endif
        }
    }
}