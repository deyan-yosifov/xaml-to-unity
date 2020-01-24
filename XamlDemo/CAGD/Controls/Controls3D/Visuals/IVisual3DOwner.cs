using System;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Visuals
{
    public interface IVisual3DOwner
    {
        Visual3D Visual { get; }
    }
}
