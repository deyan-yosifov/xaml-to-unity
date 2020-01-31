using CAGD.Controls.Controls3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CAGD
{
    /// <summary>
    /// Interaction logic for TensorProductBezierSurface.xaml
    /// </summary>
    public partial class TensorProductBezierSurface : UserControl
    {
        private readonly TensorProductBezierViewModel viewModel;

        public TensorProductBezierSurface()
        {
            InitializeComponent();
            this.viewModel = new TensorProductBezierViewModel(this.scene);
            this.DataContext = this.viewModel;
        }
    }
}
