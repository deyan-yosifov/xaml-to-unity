using CAGD.Controls.Common;
using UnityEngine.EventSystems;

namespace CAGD.Controls.PointerHandlers
{
    public interface IPointerHandler : INamedObject
    {
        bool IsEnabled { get; set; }

        bool HandlesDragMove { get; }

        bool TryHandleMouseDown(PointerEventArgs<PointerEventData> e);

        bool TryHandleMouseUp(PointerEventArgs<PointerEventData> e);

        bool TryHandleMouseMove(PointerEventArgs<PointerEventData> e);

        bool TryHandleMouseWheel(PointerEventArgs<MouseWheelEventData> e);
    }
}
