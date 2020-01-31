using CAGD.Controls.Controls3D.Cameras;
using CAGD.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Iteractions
{
    public class IteractionRestrictor
    {
        private readonly SceneEditor editor;
        private readonly double minumumCosineWithConstraintPlaneNormal = 0.03;
        private readonly double minimumCosineWithConstrainedAxisDirection = 0.01;
        private readonly Dictionary<AxisDirection, Vector3D> axisDirectionToVector;
        private PointIteractionPositionInfo firstIteractionInfo;
        private Point3D firstIteractionPosition;
        private Point3D? lastIteractionPosition;

        public IteractionRestrictor(SceneEditor editor)
        {
            this.editor = editor;
            this.firstIteractionInfo = null;
            this.axisDirectionToVector = new Dictionary<AxisDirection, Vector3D>();
            this.axisDirectionToVector[AxisDirection.X] = new Vector3D(1, 0, 0);
            this.axisDirectionToVector[AxisDirection.Y] = new Vector3D(0, 1, 0);
            this.axisDirectionToVector[AxisDirection.Z] = new Vector3D(0, 0, 1);
            this.CanMoveOnXAxis = true;
            this.CanMoveOnYAxis = true;
            this.CanMoveOnZAxis = true;
        }
        
        public bool CanMoveOnXAxis { get; set; }

        public bool CanMoveOnYAxis { get; set; }

        public bool CanMoveOnZAxis { get; set; }

        public bool IsInIteraction
        {
            get
            {
                return this.firstIteractionInfo != null;
            }
        }

        public void BeginIteraction(Point3D point)
        {
            this.firstIteractionPosition = point;
            Point viewportPosition;

            this.editor.DoActionOnCamera(
                (perspectiveCamera) =>
                {
                    if (CameraHelper.TryGetVisiblePointFromPoint3D(point, this.editor.ViewportSize, perspectiveCamera, out viewportPosition))
                    {
                        this.CalculateFirstIteractionInfo(perspectiveCamera, viewportPosition, this.editor.ViewportSize);
                    }
                },
                (orthographicCamera) =>
                {
                    if (CameraHelper.TryGetVisiblePointFromPoint3D(point, this.editor.ViewportSize, orthographicCamera, out viewportPosition))
                    {
                        this.CalculateFirstIteractionInfo(orthographicCamera, viewportPosition, this.editor.ViewportSize);
                    }
                });
        }

        public void EndIteraction()
        {
            this.firstIteractionInfo = null;
        }

        public bool TryGetIteractionPoint(Point viewportPosition, out Point3D position)
        {
            if (this.IsInIteraction)
            {
                Size viewportSize = this.editor.ViewportSize;

                this.editor.DoActionOnCamera(
                    (perspectiveCamera) =>
                    {
                        this.CalculateLastIteractionPosition(perspectiveCamera, viewportPosition, viewportSize);
                    },
                    (orthographicCamera) =>
                    {
                        this.CalculateLastIteractionPosition(orthographicCamera, viewportPosition, viewportSize);
                    });
            }
            else
            {
                this.lastIteractionPosition = null;
            }

            position = this.lastIteractionPosition.HasValue ? this.lastIteractionPosition.Value : new Point3D();

            return this.lastIteractionPosition.HasValue;
        }

        private void CalculateLastIteractionPosition(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize)
        {
            PointIteractionPositionInfo info = this.firstIteractionInfo;
            Point3D iteractionPosition = this.CalculateIteractionPosition(perspectiveCamera, viewportPosition, viewportSize, info.MovementPlanePoint, info.MovementPlaneNormal);

            if (info.ProjectionLineVector.HasValue)
            {
                iteractionPosition = IteractionRestrictor.ProjectPointOntoLine(info.MovementPlanePoint, info.ProjectionLineVector.Value, iteractionPosition);

                if (info.ProjectionPlaneVector.HasValue)
                {
                    iteractionPosition = IntersectionsHelper.IntersectLineAndPlane(perspectiveCamera.Position, iteractionPosition - perspectiveCamera.Position, info.MovementPlanePoint, info.ProjectionPlaneVector.Value);
                }
            }

            Vector3D movementVector = iteractionPosition - info.InitialIteractionPosition;
            this.TryMovePointToValidPosition(perspectiveCamera, info.MovementPlanePoint + movementVector);
        }

        private void TryMovePointToValidPosition(PerspectiveCamera perspectiveCamera, Point3D position)
        {
            if (Vector3D.DotProduct(perspectiveCamera.LookDirection, position - perspectiveCamera.Position) > 0)
            {
                this.lastIteractionPosition = position;
            }
            else
            {
                this.lastIteractionPosition = null;
            }
        }

        private void CalculateLastIteractionPosition(OrthographicCamera orthographicCamera, Point viewportPosition, Size viewportSize)
        {
            throw new NotImplementedException();
        }

        private static Point3D ProjectPointOntoLine(Point3D linePoint, Vector3D lineDirection, Point3D pointToProject)
        {
            double coordinate = Vector3D.DotProduct(pointToProject - linePoint, lineDirection);
            Point3D projection = linePoint + lineDirection * coordinate;

            return projection;
        }

        private void CalculateFirstIteractionInfo(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize)
        {
            int allowedDirections = (this.CanMoveOnXAxis ? 1 : 0) + (this.CanMoveOnYAxis ? 1 : 0) + (this.CanMoveOnZAxis ? 1 : 0);

            if (allowedDirections == 3)
            {
                this.CalculateFirstIteractionMovingParallelToProjectionPlane(perspectiveCamera, viewportPosition, viewportSize);
            }
            else if (allowedDirections == 2)
            {
                AxisDirection axis = this.CanMoveOnXAxis ? (this.CanMoveOnYAxis ? AxisDirection.Z : AxisDirection.Y) : AxisDirection.X;
                this.CalculateFirstIteractionMovingInAxisPlane(perspectiveCamera, viewportPosition, viewportSize, this.axisDirectionToVector[axis]);
            }
            else if (allowedDirections == 1)
            {
                AxisDirection axis = this.CanMoveOnXAxis ? AxisDirection.X : (this.CanMoveOnYAxis ? AxisDirection.Y : AxisDirection.Z);
                this.CalculateFirstIteractionMovingInAxisDirection(perspectiveCamera, viewportPosition, viewportSize, this.axisDirectionToVector[axis]);
            }
            else
            {
                this.firstIteractionInfo = null;
            }
        }

        private void CalculateFirstIteractionMovingInAxisDirection(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize, Vector3D lineDirection)
        {
            Vector3D viewVector = this.firstIteractionPosition - perspectiveCamera.Position;
            viewVector.Normalize();

            double cosine = Vector3D.DotProduct(lineDirection, viewVector);

            if (Math.Abs(cosine).IsEqualTo(1, minimumCosineWithConstrainedAxisDirection))
            {
                this.firstIteractionInfo = null;
                return;
            }

            this.CalculateFirstIteractionMovingParallelToProjectionPlane(perspectiveCamera, viewportPosition, viewportSize);
            PointIteractionPositionInfo info = this.firstIteractionInfo;

            double planeNormalCosine = Vector3D.DotProduct(info.MovementPlaneNormal, lineDirection);
            if (planeNormalCosine.IsZero())
            {
                info.ProjectionLineVector = lineDirection;
                info.InitialIteractionPosition = IteractionRestrictor.ProjectPointOntoLine(this.firstIteractionPosition, info.ProjectionLineVector.Value, info.InitialIteractionPosition);
            }
            else
            {
                Point3D pointToProject = this.firstIteractionPosition + lineDirection;
                Point3D projectedPoint = IntersectionsHelper.IntersectLineAndPlane(perspectiveCamera.Position, pointToProject - perspectiveCamera.Position, this.firstIteractionPosition, info.MovementPlaneNormal);
                Vector3D projectionVector = projectedPoint - this.firstIteractionPosition;
                projectionVector.Normalize();
                info.ProjectionLineVector = projectionVector;

                Vector3D projectionVectorLineDirectionNormal = Vector3D.CrossProduct(lineDirection, projectionVector);
                Vector3D projectionPlaneNormal = Vector3D.CrossProduct(projectionVectorLineDirectionNormal, lineDirection);
                projectionPlaneNormal.Normalize();
                info.ProjectionPlaneVector = projectionPlaneNormal;

                info.InitialIteractionPosition = IteractionRestrictor.ProjectPointOntoLine(this.firstIteractionPosition, info.ProjectionLineVector.Value, info.InitialIteractionPosition);
                info.InitialIteractionPosition = IntersectionsHelper.IntersectLineAndPlane(perspectiveCamera.Position, info.InitialIteractionPosition - perspectiveCamera.Position, this.firstIteractionPosition, projectionPlaneNormal);
            }
        }

        private void CalculateFirstIteractionMovingInAxisPlane(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize, Vector3D planeNormal)
        {
            Vector3D viewVector = this.firstIteractionPosition - perspectiveCamera.Position;
            viewVector.Normalize();
            double cosine = Vector3D.DotProduct(viewVector, planeNormal);

            if (cosine.IsZero(minumumCosineWithConstraintPlaneNormal))
            {
                this.CalculateFirstIteractionMovingParallelToProjectionPlane(perspectiveCamera, viewportPosition, viewportSize);
                Vector3D projectionVector = Vector3D.CrossProduct(perspectiveCamera.LookDirection, planeNormal);
                projectionVector.Normalize();
                PointIteractionPositionInfo info = this.firstIteractionInfo;
                info.ProjectionLineVector = projectionVector;
                info.InitialIteractionPosition = IteractionRestrictor.ProjectPointOntoLine(this.firstIteractionPosition, projectionVector, info.InitialIteractionPosition);
            }
            else
            {
                PointIteractionPositionInfo info = new PointIteractionPositionInfo();
                info.MovementPlanePoint = this.firstIteractionPosition;
                info.MovementPlaneNormal = planeNormal;
                info.InitialIteractionPosition = this.CalculateIteractionPosition(perspectiveCamera, viewportPosition, viewportSize, info.MovementPlanePoint, info.MovementPlaneNormal);

                this.firstIteractionInfo = info;
            }
        }

        private void CalculateFirstIteractionMovingParallelToProjectionPlane(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize)
        {
            PointIteractionPositionInfo info = new PointIteractionPositionInfo();
            info.MovementPlanePoint = this.firstIteractionPosition;
            Vector3D planeNormal = perspectiveCamera.LookDirection;
            planeNormal.Normalize();
            info.MovementPlaneNormal = planeNormal;
            info.InitialIteractionPosition = this.CalculateIteractionPosition(perspectiveCamera, viewportPosition, viewportSize, info.MovementPlanePoint, info.MovementPlaneNormal);

            this.firstIteractionInfo = info;
        }

        private Point3D CalculateIteractionPosition(PerspectiveCamera perspectiveCamera, Point viewportPosition, Size viewportSize, Point3D planePoint, Vector3D planeNormal)
        {
            Vector3D iteractionDirection = CameraHelper.GetLookDirectionFromPoint(viewportPosition, viewportSize, perspectiveCamera);
            Point3D interactionOnMovementPlane = IntersectionsHelper.IntersectLineAndPlane(perspectiveCamera.Position, iteractionDirection, planePoint, planeNormal);

            return interactionOnMovementPlane;
        }

        private void CalculateFirstIteractionInfo(OrthographicCamera orthographicCamera, Point viewportPosition, Size viewportSize)
        {
            throw new NotImplementedException();
        }
    }
}
