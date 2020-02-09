using UnityEngine;

namespace CAGD.Controls.PointerHandlers
{
    public class PointerEventArgs<T>
    {
        public readonly T data;
        public readonly GameObject sender;
        public readonly float timeStamp;

        public PointerEventArgs(GameObject sender, T data)
        {
            this.sender = sender;
            this.data = data;
            this.timeStamp = Time.time;
        }
    }
}
