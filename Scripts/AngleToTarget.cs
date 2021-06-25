using System;
using UnityEditor;
using UnityEngine;
namespace AngleVisualiser
{
    public class AngleToTarget : MonoBehaviour
    {
        [SerializeField] TextData angleText;
        [SerializeField] TextData dotText;
        public Transform target;
        public float arcRadius;
        public float textRadius;

        //UsefulPoints
        Vector3 directionToTarget;
        Vector3 directionNormal;
        float angleToTarget;
        float dotRightLeft;
        float dotUpDown;

        void OnDrawGizmos()
        {
            if (Validate())
            {
                if (target != null)
                {
                    directionToTarget = GetDirectionToTarget(transform.position, target.position);
                    dotRightLeft = GetDot(transform.right, directionToTarget);
                    dotUpDown = GetDot(transform.up, directionToTarget);
                    var dotFlipRightToLeft = DotFlip(dotRightLeft); 
                    directionNormal = Vector3.Cross(directionToTarget, dotFlipRightToLeft * -transform.forward);
                    angleToTarget = GetAngleToTarget(transform.forward, directionToTarget, directionNormal);

                    var modifiedAngleToTarget = dotFlipRightToLeft * angleToTarget;
                    var textRotation = MathHelper.FindHitPointRotation(-directionNormal, Vector3.forward, new Vector3(0,0,dotFlipRightToLeft* 90));
                  
                    DrawHelper.DrawDirection(transform.position, directionNormal, Handles.yAxisColor);
                    Gizmos.DrawLine(transform.position, target.position);
                    DrawHelper.DrawArc(transform.position, arcRadius, angleToTarget, directionNormal, transform.forward, DrawHelper.GetColorBasedOnAngle(modifiedAngleToTarget));
                    DrawAngleText(textRotation, modifiedAngleToTarget);
                    dotText.DrawText(transform.position + transform.forward * arcRadius, transform.rotation*Quaternion.Euler(new Vector3(90,0,0)), $"Forward Dot {Math.Round(dotRightLeft, 2)}");
                }
            }
        }
        void DrawAngleText(Quaternion textRotation, float angle)
        {
            if (dotUpDown < 0)
            {
                textRotation *= Quaternion.Euler(new Vector3(180, 0, 0));
                textRotation *= Quaternion.Euler(new Vector3(0, 180, 0));
            }
            var angleTextPos = MathHelper.GetPositionOnCircle(transform.position, transform.forward,
                         directionToTarget, arcRadius, textRadius);
            angleText.DrawText(angleTextPos, textRotation, $"{Math.Round(angle, 2)} °");
        }



        private bool Validate()
        {
            if (angleText == null || dotText == null)
            {
                print("Missing text. Make sure to create text by right clicking Angle visualiser component and Selecting \"Create Text\"");
                return false;
            }
            return true;
        }
        [ContextMenu("Create Text")]
        public void AddText()
        {
            angleText = new TextData(transform, "Angle Visualiser Text - Signed Angle To Target");
            dotText = new TextData(transform, "Dot product between local right and direction to target");
        }
        private float GetAngleToTarget(Vector3 forward, Vector3 directionToTarget, Vector3 axis) => Vector3.SignedAngle(forward, directionToTarget, axis);
        private Vector3 GetDirectionToTarget(Vector3 origin, Vector3 target) => (target - origin).normalized;
        //private Quaternion GetTextRotation(Vector3 normal, Vector3 up) => Quaternion.LookRotation()   originRotation;// * Quaternion.Euler(new Vector3(90, 0, 0));
        private float GetDot(Vector3 right, Vector3 directionToTarget) => Vector3.Dot(right, directionToTarget);
        private float DotFlip(float dot)
        {
            if (dot > 0)
            {
                return 1;
            }
            else if (dot < 0)
            {
                return -1;
            }
            return 1;
        }
    }
}
