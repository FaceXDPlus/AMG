using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
    public class MouseObject : MonoBehaviour
    {
        void Update()
        {
            transform.position = Input.mousePosition;
        }
    }
}
