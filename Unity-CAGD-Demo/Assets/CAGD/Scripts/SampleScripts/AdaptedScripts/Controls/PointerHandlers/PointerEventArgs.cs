using UnityEngine;

namespace CAGD.Controls.PointerHandlers
{
    public class PointerEventArgs<T>
    {
        public readonly T data;
        public readonly float timeStamp;
        public readonly Camera raycastCamera;

        public PointerEventArgs(Camera raycastCamera, T data)
        {
            this.data = data;
            this.timeStamp = Time.time;
            this.raycastCamera = raycastCamera;
        }
    }
}
