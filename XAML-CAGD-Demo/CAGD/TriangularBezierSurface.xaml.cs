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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CAGD
{
    /// <summary>
    /// Interaction logic for TriangularBezierSurface.xaml
    /// </summary>
    public partial class TriangularBezierSurface : UserControl
    {
        private readonly TriangularBezierViewModel viewModel;

        public TriangularBezierSurface()
        {
            InitializeComponent();
            this.viewModel = new TriangularBezierViewModel(this.scene);
            this.DataContext = this.viewModel;
        }
    }
}
