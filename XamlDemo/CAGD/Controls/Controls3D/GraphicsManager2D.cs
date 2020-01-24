using CAGD.Utilities;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CAGD.Controls.Controls3D
{
    public class GraphicsManager2D : INotifyPropertiesChanged, ICloneable<GraphicsManager2D>
    {
        private Color fill;
        private Color stroke;
        private double strokeThickness;
        private double[] strokeDashArray;

        internal GraphicsManager2D()
        {
            this.fill = Colors.Black;
            this.stroke = Colors.Black;
            this.strokeThickness = 0;
            this.strokeDashArray = null;
        }

        private GraphicsManager2D(GraphicsManager2D other)
        {
            this.fill = other.Fill;
            this.stroke = other.Stroke;
            this.strokeThickness = other.StrokeThickness;
            this.strokeDashArray = other.strokeDashArray;
        }

        public Color Fill
        {
            get
            {
                return this.fill;
            }
            set
            {
                if (this.fill != value)
                {
                    this.fill = value;
                    this.OnPropertyChanged(GraphicPropertyNames.Fill2D);
                }
            }
        }

        public Color Stroke
        {
            get
            {
                return this.stroke;
            }
            set
            {
                if (this.stroke != value)
                {
                    this.stroke = value;
                    this.OnPropertyChanged(GraphicPropertyNames.Stroke2D);
                }
            }
        }

        public double StrokeThickness
        {
            get
            {
                return this.strokeThickness;
            }
            set
            {
                if (this.strokeThickness != value)
                {
                    this.strokeThickness = value;
                    this.OnPropertyChanged(GraphicPropertyNames.StrokeThickness2D);
                }
            }
        }

        public double[] StrokeDashArray
        {
            get
            {
                return this.strokeDashArray;
            }
            set
            {
                if (this.strokeDashArray != value)
                {
                    this.strokeDashArray = value;
                    this.OnPropertyChanged(GraphicPropertyNames.StrokeDashArray2D);
                }
            }
        }

        public event EventHandler<PropertiesChangedEventArgs> PropertiesChanged;

        public GraphicsManager2D Clone()
        {
            return new GraphicsManager2D(this);
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertiesChanged(new string[] { propertyName });
        }

        private void OnPropertiesChanged(IEnumerable<string> propertyNames)
        {
            if (this.PropertiesChanged != null)
            {
                this.PropertiesChanged(this, new PropertiesChangedEventArgs(propertyNames));
            }
        }
    }
}
