using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Shapes
{
    public class Cube : ShapeBase
    {
        private static List<Point3D> positionCoordinates;
        private static List<Point> textureCoordinates;
        private static List<int> triangleIndexes;
        
        public Cube()
        {
            this.GeometryModel.Geometry = Cube.GenerateGeometry();
            this.GeometryModel.Geometry.Freeze();
        }

        private static Geometry3D GenerateGeometry()
        {
            if (Cube.textureCoordinates == null)
            {
                Cube.InitializeTextureToPointMappings();
            }

            MeshGeometry3D geometry = new MeshGeometry3D();

            foreach (Point3D positionCoordinate in Cube.positionCoordinates)
            {
                geometry.Positions.Add(positionCoordinate);
            }

            foreach (Point textureCoordinate in Cube.textureCoordinates)
            {
                geometry.TextureCoordinates.Add(textureCoordinate);
            }

            foreach (int triangleIndex in Cube.triangleIndexes)
            {
                geometry.TriangleIndices.Add(triangleIndex);
            }

            return geometry;
        }

        private static void InitializeTextureToPointMappings()
        {
            Cube.textureCoordinates = new List<Point>();
            Cube.positionCoordinates = new List<Point3D>();
            Cube.triangleIndexes = new List<int>();

            int[] zCoordinates = { 1, 1, 0, 0 };
            Point[] xyCoordinates = { new Point(1, 0), new Point(1, 1), new Point(0, 1), new Point(0, 0), new Point(1, 0) };
            
            Action<int, int> initializeSquareVertex = (u, v) =>
            {
                double yCoordinate = (v == 0 || v == (zCoordinates.Length - 1)) ? 0 : xyCoordinates[u].Y;

                Cube.textureCoordinates.Add(new Point(u / (xyCoordinates.Length - 1.0), v / (zCoordinates.Length - 1.0)));
                Cube.positionCoordinates.Add(new Point3D(xyCoordinates[u].X, yCoordinate, zCoordinates[v]));
            };

            Action<int, int> initializeSquareSide = (u, v) =>
            {
                initializeSquareVertex(u, v);
                initializeSquareVertex(u, v+1);
                initializeSquareVertex(u+1, v+1);
                initializeSquareVertex(u+1, v);

                triangleIndexes.Add(Cube.positionCoordinates.Count - 4);
                triangleIndexes.Add(Cube.positionCoordinates.Count - 3);
                triangleIndexes.Add(Cube.positionCoordinates.Count - 2);

                triangleIndexes.Add(Cube.positionCoordinates.Count - 2);
                triangleIndexes.Add(Cube.positionCoordinates.Count - 1);
                triangleIndexes.Add(Cube.positionCoordinates.Count - 4);
            };

            initializeSquareSide(0, 1);
            initializeSquareSide(1, 0);
            initializeSquareSide(1, 1);
            initializeSquareSide(1, 2);
            initializeSquareSide(2, 1);
            initializeSquareSide(3, 1);
        }
    }
}
