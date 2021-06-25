using UnityEngine;

namespace AngleVisualiser
{
    public static class MathHelper
    {
        // var resultRotation = Quaternion.LookRotation(hitNormal, -ReferenceUp);
        public static Quaternion FindHitPointRotation(Vector3 normal, Vector3 up, Vector3 offsetEulers)
        {
            var resultRotation = Quaternion.LookRotation(normal, -up);
            resultRotation *= Quaternion.Euler(offsetEulers); //Quaternion.Euler(new Vector3(90, 0, 0));
            return resultRotation;
        }
        //var result = center + ((direction - ReferenceForward) * radius * radiusOfText);
        public static Vector3 GetPositionOnCircle(Vector3 center, Vector3 forward, Vector3 right, float circleRadius, float positionRadius)
        {
            var result = center + ((right + forward).normalized * circleRadius * positionRadius);
            //Gizmos.DrawSphere(result, 0.1f);
            return result;
        } 
        public static Vector3 FindPointOnArcCenterLine(Vector3 centerBaseOfChord, Vector3 centerline, float arcRadius)
        {
            var result = centerBaseOfChord + (-centerline * arcRadius);
            //Gizmos.DrawSphere(result, 0.1f);
            return result;
        }
        public static Vector3 OrthogonalDirectionToDetectedNormal(float angle, Vector3 angleAxis, Vector3 dir)
        {
            var result = Quaternion.AngleAxis(angle, angleAxis) * dir;
            //Gizmos.DrawSphere(result, 0.1f);
            return result;
        }
    }
}
