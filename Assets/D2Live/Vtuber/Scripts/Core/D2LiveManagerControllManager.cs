using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace D2LiveManager
{
    [DisallowMultipleComponent, RequireComponent (typeof(D2LiveManagerProcessOrderList))]
    public class D2LiveManagerControllManager : MonoBehaviour
    {
        protected List<D2LiveManagerProcess> processOrderList;

        // Use this for initialization
        protected virtual IEnumerator Start ()
        {
            enabled = false;

            yield return null;

            processOrderList = GetComponent<D2LiveManagerProcessOrderList> ().GetProcessOrderList ();
            if (processOrderList == null)
                yield break;

            foreach (var item in processOrderList) {
                if (item == null)
                    continue;

                //Debug.Log("Setup : "+item.gameObject.name);

                item.Setup ();
            }

            enabled = true;
        }

        // Update is called once per frame
        protected virtual void Update ()
        {
            if (processOrderList == null)
                return;

            foreach (var item in processOrderList) {
                if (item == null)
                    continue;

                if (!item.gameObject.activeInHierarchy || !item.enabled)
                    continue;

                //Debug.Log("UpdateValue : " + item.gameObject.name);

                item.UpdateValue ();
            }
        }

        // Update is called once per frame
        protected virtual void LateUpdate ()
        {
            if (processOrderList == null)
                return;

            foreach (var item in processOrderList) {
                if (item == null)
                    continue;

                if (!item.gameObject.activeInHierarchy || !item.enabled)
                    continue;

                //Debug.Log("LateUpdateValue : " + item.gameObject.name);

                item.LateUpdateValue ();
            }
        }

        protected virtual void OnDestroy ()
        {
            Dispose ();
        }

        public virtual void Dispose ()
        {
            
        }
    }
}