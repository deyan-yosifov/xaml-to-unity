using CAGD.Controls.Common;
using System.Windows.Input;

namespace CAGD.Controls.MouseHandlers
{
    public interface IPointerHandler : INamedObject
    {
        bool IsEnabled { get; set; }

        bool HandlesDragMove { get; }

        bool TryHandleMouseDown(PointerEventArgs<MouseButtonEventArgs> e);

        bool TryHandleMouseUp(PointerEventArgs<MouseButtonEventArgs> e);

        bool TryHandleMouseMove(PointerEventArgs<MouseEventArgs> e);

        bool TryHandleMouseWheel(PointerEventArgs<MouseWheelEventArgs> e);
    }
}
