using OpenCVForUnity.CoreModule;

namespace D2LiveManager
{
    public interface IMatSourceGetter
    {
        Mat GetMatSource ();

        Mat GetDownScaleMatSource ();

        float GetDownScaleRatio ();
    }
}