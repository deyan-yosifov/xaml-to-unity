using CAGD.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace CAGD.Controls.Controls3D.Iteractions
{
    public class IteractionRestrictor
    {
        private readonly float minumumCosineWithConstraintPlaneNormal = 0.03f;
        private readonly float minimumCosineWithConstrainedAxisDirection = 0.01f;
        private readonly Dictionary<AxisDirection, Vector3> axisDirectionToVector;
        private PointIteractionPositionInfo firstIteractionInfo;
        private Vector3 firstIteractionPosition;
        private Vector3? lastIteractionPosition;

        public IteractionRestrictor()
        {
            this.firstIteractionInfo = null;
            this.axisDirectionToVector = new Dictionary<AxisDirection, Vector3>();
            this.axisDirectionToVector[AxisDirection.X] = new Vector3(1, 0, 0);
            this.axisDirectionToVector[AxisDirection.Y] = new Vector3(0, 1, 0);
            this.axisDirectionToVector[AxisDirection.Z] = new Vector3(0, 0, 1);
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

        public void BeginIteraction(Camera camera, Vector3 point)
        {
            this.firstIteractionPosition = point;
            Vector2 viewportPosition = camera.WorldToScreenPoint(point);
            this.CalculateFirstIteractionInfo(camera, viewportPosition);

            if (this.firstIteractionInfo != null)
            {
                this.firstIteractionInfo.Camera = camera;
            }
        }

        public void EndIteraction()
        {
            this.firstIteractionInfo = null;
        }

        public bool TryGetIteractionPoint(Vector2 viewportPosition, out Vector3 position)
        {
            if (this.IsInIteraction)
            {
                this.CalculateLastIteractionPosition(viewportPosition);
            }
            else
            {
                this.lastIteractionPosition = null;
            }

            position = this.lastIteractionPosition.HasValue ? this.lastIteractionPosition.Value : new Vector3();

            return this.lastIteractionPosition.HasValue;
        }

        private void CalculateLastIteractionPosition(Vector2 viewportPosition)
        {
            PointIteractionPositionInfo info = this.firstIteractionInfo;
            Vector3 iteractionPosition = this.CalculateIteractionPosition(info.Camera, viewportPosition, info.MovementPlane);

            if (info.ProjectionLineVector.HasValue)
            {
                iteractionPosition = IteractionRestrictor.ProjectPointOntoLine(this.firstIteractionPosition, info.ProjectionLineVector.Value, iteractionPosition);

                if (info.ProjectionPlaneVector.HasValue)
                {
                    Ray ray = new Ray(info.Camera.transform.position, iteractionPosition - info.Camera.transform.position);
                    Plane plane = new Plane(info.ProjectionPlaneVector.Value, this.firstIteractionPosition);
                    plane.Raycast(ray, out float distance);
                    iteractionPosition = ray.GetPoint(distance);
                }
            }

            Vector3 movementVector = iteractionPosition - info.InitialIteractionPosition;
            this.TryMovePointToValidPosition(this.firstIteractionPosition + movementVector);
        }

        private void TryMovePointToValidPosition(Vector3 position)
        {
            if (Vector3.Dot(this.firstIteractionInfo.Camera.transform.forward, position - this.firstIteractionInfo.Camera.transform.position) > 0)
            {
                this.lastIteractionPosition = position;
            }
            else
            {
                this.lastIteractionPosition = null;
            }
        }

        private static Vector3 ProjectPointOntoLine(Vector3 linePoint, Vector3 lineDirection, Vector3 pointToProject)
        {
            float coordinate = Vector3.Dot(pointToProject - linePoint, lineDirection);
            Vector3 projection = linePoint + lineDirection * coordinate;

            return projection;
        }

        private void CalculateFirstIteractionInfo(Camera perspectiveCamera, Vector2 viewportPosition)
        {
            int allowedDirections = (this.CanMoveOnXAxis ? 1 : 0) + (this.CanMoveOnYAxis ? 1 : 0) + (this.CanMoveOnZAxis ? 1 : 0);

            if (allowedDirections == 3)
            {
                this.CalculateFirstIteractionMovingParallelToProjectionPlane(perspectiveCamera, viewportPosition);
            }
            else if (allowedDirections == 2)
            {
                AxisDirection axis = this.CanMoveOnXAxis ? (this.CanMoveOnYAxis ? AxisDirection.Z : AxisDirection.Y) : AxisDirection.X;
                this.CalculateFirstIteractionMovingInAxisPlane(perspectiveCamera, viewportPosition, this.axisDirectionToVector[axis]);
            }
            else if (allowedDirections == 1)
            {
                AxisDirection axis = this.CanMoveOnXAxis ? AxisDirection.X : (this.CanMoveOnYAxis ? AxisDirection.Y : AxisDirection.Z);
                this.CalculateFirstIteractionMovingInAxisDirection(perspectiveCamera, viewportPosition, this.axisDirectionToVector[axis]);
            }
            else
            {
                this.firstIteractionInfo = null;
            }
        }

        private void CalculateFirstIteractionMovingInAxisDirection(Camera perspectiveCamera, Vector2 viewportPosition, Vector3 lineDirection)
        {
            Vector3 viewVector = this.firstIteractionPosition - perspectiveCamera.transform.position;
            viewVector.Normalize();

            float cosine = Vector3.Dot(lineDirection, viewVector);

            if (Mathf.Abs(cosine).IsEqualTo(1, minimumCosineWithConstrainedAxisDirection))
            {
                this.firstIteractionInfo = null;
                return;
            }

            this.CalculateFirstIteractionMovingParallelToProjectionPlane(perspectiveCamera, viewportPosition);
            PointIteractionPositionInfo info = this.firstIteractionInfo;

            float planeNormalCosine = Vector3.Dot(info.MovementPlane.normal, lineDirection);
            if (planeNormalCosine.IsZero())
            {
                info.ProjectionLineVector = lineDirection;
                info.InitialIteractionPosition = IteractionRestrictor.ProjectPointOntoLine(this.firstIteractionPosition, info.ProjectionLineVector.Value, info.InitialIteractionPosition);
            }
            else
            {
                Vector3 pointToProject = this.firstIteractionPosition + lineDirection;
                Ray ray = new Ray(perspectiveCamera.transform.position, pointToProject - perspectiveCamera.transform.position);
                info.MovementPlane.Raycast(ray, out float distance);
                Vector3 projectedPoint = ray.GetPoint(distance);
                Vector3 projectionVector = projectedPoint - this.firstIteractionPosition;
                projectionVector.Normalize();
                info.ProjectionLineVector = projectionVector;

                Vector3 projectionVectorLineDirectionNormal = Vector3.Cross(lineDirection, projectionVector);
                Vector3 projectionPlaneNormal = Vector3.Cross(projectionVectorLineDirectionNormal, lineDirection);
                projectionPlaneNormal.Normalize();
                info.ProjectionPlaneVector = projectionPlaneNormal;

                info.InitialIteractionPosition = IteractionRestrictor.ProjectPointOntoLine(this.firstIteractionPosition, info.ProjectionLineVector.Value, info.InitialIteractionPosition);
                Plane projectionPlane = new Plane(projectionPlaneNormal, this.firstIteractionPosition);
                Ray initialRay = new Ray(perspectiveCamera.transform.position, info.InitialIteractionPosition - perspectiveCamera.transform.position);
                projectionPlane.Raycast(initialRay, out float initialDistance);
                info.InitialIteractionPosition = initialRay.GetPoint(initialDistance);
            }
        }

        private void CalculateFirstIteractionMovingInAxisPlane(Camera perspectiveCamera, Vector2 viewportPosition, Vector3 planeNormal)
        {
            Vector3 viewVector = this.firstIteractionPosition - perspectiveCamera.transform.position;
            viewVector.Normalize();
            float cosine = Vector3.Dot(viewVector, planeNormal);

            if (cosine.IsZero(minumumCosineWithConstraintPlaneNormal))
            {
                this.CalculateFirstIteractionMovingParallelToProjectionPlane(perspectiveCamera, viewportPosition);
                Vector3 projectionVector = Vector3.Cross(perspectiveCamera.transform.forward, planeNormal);
                projectionVector.Normalize();
                PointIteractionPositionInfo info = this.firstIteractionInfo;
                info.ProjectionLineVector = projectionVector;
                info.InitialIteractionPosition = IteractionRestrictor.ProjectPointOntoLine(this.firstIteractionPosition, projectionVector, info.InitialIteractionPosition);
            }
            else
            {
                PointIteractionPositionInfo info = new PointIteractionPositionInfo();
                info.MovementPlane = new Plane(planeNormal, this.firstIteractionPosition);
                info.InitialIteractionPosition = this.CalculateIteractionPosition(perspectiveCamera, viewportPosition, info.MovementPlane);

                this.firstIteractionInfo = info;
            }
        }

        private void CalculateFirstIteractionMovingParallelToProjectionPlane(Camera camera, Vector2 viewportPosition)
        {
            PointIteractionPositionInfo info = new PointIteractionPositionInfo();
            Vector3 planeNormal = camera.transform.forward;
            planeNormal.Normalize();
            info.MovementPlane = new Plane(planeNormal, this.firstIteractionPosition);
            info.InitialIteractionPosition = this.CalculateIteractionPosition(camera, viewportPosition, info.MovementPlane);

            this.firstIteractionInfo = info;
        }

        private Vector3 CalculateIteractionPosition(Camera camera, Vector2 viewportPosition, Plane movementPlane)
        {
            Ray ray = camera.ScreenPointToRay(viewportPosition);
            movementPlane.Raycast(ray, out float distance);
            Vector3 interactionOnMovementPlane = ray.GetPoint(distance);

            return interactionOnMovementPlane;
        }
    }
}
