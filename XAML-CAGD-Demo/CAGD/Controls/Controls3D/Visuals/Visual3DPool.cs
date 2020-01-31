using CAGD.Controls.Common;
using System.Windows.Controls;

namespace CAGD.Controls.Controls3D.Visuals
{
    public class Visual3DPool<T> : ObjectPool<T>
        where T : IVisual3DOwner
    {
        public Visual3DPool(Scene3D scene)
            : this(scene.Viewport)
        {
        }

        public Visual3DPool(Viewport3D viewport)
            : base((element) => ShowElement(element, viewport), (element) => HideElement(element, viewport))
        {
        }

        private static void ShowElement(T element, Viewport3D viewport)
        {
            viewport.Children.Add(element.Visual);
        }

        private static void HideElement(T element, Viewport3D viewport)
        {
            viewport.Children.Remove(element.Visual);
        }
    }
}
