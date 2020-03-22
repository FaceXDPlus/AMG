using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace D2LiveManager
{
    [DisallowMultipleComponent]
    public class D2LiveManagerProcessOrderList : MonoBehaviour
    {
        [SerializeField]
        List<D2LiveManagerProcess> processOrderList = default(List<D2LiveManagerProcess>);

        public List<D2LiveManagerProcess> GetProcessOrderList ()
        {
            return processOrderList;
        }
    }
}
