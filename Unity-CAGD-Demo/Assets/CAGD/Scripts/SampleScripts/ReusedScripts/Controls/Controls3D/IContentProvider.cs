using System.Collections.Generic;
using UnityEngine;

namespace CAGD.Controls.Controls3D
{
    public interface IContentProvider
    {
        IEnumerable<Vector3> GetContentPoints();
    }
}
