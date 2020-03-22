using UnityEngine;

namespace D2LiveManager
{
    public interface IHeadRotationGetter
    {
        Quaternion GetHeadRotation ();

        Vector3 GetHeadEulerAngles ();
    }
}