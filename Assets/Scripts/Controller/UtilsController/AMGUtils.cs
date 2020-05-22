using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
    public class AMGUtils : MonoBehaviour
    {
        public class ArrayInfo
        {
            public string hostName { get; set; }
            public ArrayList keyboardAttached { get; set; }
            public Dictionary<string, string> ipMessage { get; set; }
        }

        public class SaveArrayInfo
        {
            public string APPVersion { get; set; }
            public Dictionary<string, string> ModelDict { get; set; }
            public Dictionary<string, Dictionary<string, string>> ShortcutPair { get; set; }
        }
    }
}
