using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D
{
    public interface IContentProvider
    {
        IEnumerable<Point3D> GetContentPoints();
    }
}
