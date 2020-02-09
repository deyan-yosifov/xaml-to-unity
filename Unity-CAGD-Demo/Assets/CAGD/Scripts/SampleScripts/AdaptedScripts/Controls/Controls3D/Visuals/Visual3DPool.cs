using CAGD.Controls.Common;

namespace CAGD.Controls.Controls3D.Visuals
{
    public class Visual3DPool<T> : ObjectPool<T>
        where T : IVisual3D
    {
        public Visual3DPool()
            : base((element) => ShowElement(element), (element) => HideElement(element))
        {
        }

        private static void ShowElement(T element)
        {
            element.IsVisible = true;
        }

        private static void HideElement(T element)
        {
            element.IsVisible = false;
        }
    }
}
