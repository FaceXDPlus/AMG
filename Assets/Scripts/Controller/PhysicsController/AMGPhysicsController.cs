using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AMG
{
    public class AMGPhysicsController : MonoBehaviour
    {
        [SerializeField] private GameObject Floor;
        [SerializeField] private GameObject CameraGroup;

        void Update()
        {
        }


        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("开始碰撞:" + collision.collider.gameObject.name);
        }

        private void OnCollisionStay(Collision collision)
        {
            Debug.Log("持续碰撞:" + collision.collider.gameObject.name);
        }

        private void OnCollisionExit(Collision collision)
        {
            Debug.Log("结束碰撞:" + collision.collider.gameObject.name);
        }
    }
}
