using System.Collections.Generic;
using UnityEngine;

namespace D2LiveManager
{
    public interface IFaceLandmarkGetter
    {
        List<Vector2> GetFaceLanmarkPoints ();
    }
}