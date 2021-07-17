using AngleVisualiser.Shared;
using UnityEngine;
namespace AngleVisualiser
{
    public class Calculate_AnglesOfSurfaceNormal : MonoBehaviour
    {
        public Vector3 rotationOffset;
        Vector3 ReferenceRight { get { return transform.rotation * Quaternion.Euler(rotationOffset) * Vector3.right; } }
        Vector3 ReferenceForward { get { return transform.rotation * Quaternion.Euler(rotationOffset) * Vector3.forward; } }
        Vector3 ReferenceUp { get { return transform.rotation * Quaternion.Euler(rotationOffset) * Vector3.up; } }
        public AngleDrawer externalAngleDrawer;
        public AngleDrawer internalAngleDrawer;
      
        //UsefulPointsAndVectors
        float internalAngle = 0.0f;
        float externalAngle = 0.0f;
        Vector3 arcNormal;
        Vector3 hitNormal;
        Vector3 hitPoint; 
        
        public bool CalculateSurfaceAngles()
        {
            var rayDirection = ReferenceForward;
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, Mathf.Infinity))
            {
                var originRayDirection = ReferenceRight;
                arcNormal = Vector3.Cross(originRayDirection, rayDirection);

                hitNormal = hit.normal;
                hitPoint = hit.point;
                externalAngle = Vector3.Angle(originRayDirection, hitNormal);
                internalAngle = 180 - externalAngle;
                return true;
            }
            return false;
        }
        void OnDrawGizmos()
        {
            if (CalculateSurfaceAngles())
            {
                Debug.DrawLine(transform.position, hitPoint, Color.yellow);
                DrawArc();
            } 
        }
        private void DrawArc()
        { 
            if (externalAngleDrawer != null)
            { 
                var arcData = new ArcData(hitPoint,
                                          -arcNormal,
                                          -ReferenceForward,
                                          DrawHelper.GetColorBasedOnAngle(externalAngle),
                                          2,
                                          externalAngle);
                externalAngleDrawer.UpdateAngleData(arcData);
            }
            if (internalAngleDrawer != null)
            {
                var arcData = new ArcData(hitPoint,
                                          arcNormal,
                                          -ReferenceForward,
                                          DrawHelper.GetColorBasedOnAngle(internalAngle),
                                          2,
                                          internalAngle);
                internalAngleDrawer.UpdateAngleData(arcData);
            }
        }
    }
}