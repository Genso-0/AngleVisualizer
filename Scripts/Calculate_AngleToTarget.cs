using System;
using AngleVisualiser.Shared;
using UnityEngine;

namespace AngleVisualiser
{
    public class Calculate_AngleToTarget : MonoBehaviour
    {
        public Transform target;
        AngleDrawer angleDrawer;
        //UsefulPoints
        Vector3 directionToTarget;
        Vector3 directionNormal;
        float angleToTarget;
        float dotFlipRightToLeft;
        public void CalculateAngleToTarget()
        {
            directionToTarget = (target.position - transform.position).normalized;
            dotFlipRightToLeft = Vector3.Dot(transform.right, directionToTarget) > 0 ? 1 : -1;
            directionNormal = Vector3.Cross(directionToTarget, dotFlipRightToLeft * -transform.forward);
            angleToTarget = Vector3.SignedAngle(transform.forward, directionToTarget, directionNormal);
        }
        void OnDrawGizmos()
        { 
            if (target != null)
            {
                CalculateAngleToTarget(); 
                Gizmos.DrawLine(transform.position, target.position);
                DrawArc();
            }
        } 
        private void DrawArc()
        {
            if (angleDrawer == null)
                angleDrawer = GetComponentInChildren<AngleDrawer>();
            if (angleDrawer != null)
            {
                var modifiedAngleToTarget = dotFlipRightToLeft * angleToTarget;
                var arcData = new ArcData(transform.position,
                                          directionNormal,
                                          transform.forward,
                                          DrawHelper.GetColorBasedOnAngle(modifiedAngleToTarget),
                                          2,
                                          angleToTarget);
                angleDrawer.UpdateAngleData(arcData);
            }
        }
    }
}