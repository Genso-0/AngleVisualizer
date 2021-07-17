using AngleVisualiser.Shared;
using System;
using UnityEditor;
using UnityEngine;

namespace AngleVisualiser
{
    [ExecuteAlways]
    public class AngleVisualizerTool : MonoBehaviour
    {
        [SerializeField] TextData externalAngleText;
        [SerializeField] TextData internalAngleText;

        public Vector3 rotationOffset;

        [Header("Arc properties")]
        float radius = 1f;
        public float radiusOfText = 0.25f;
        public bool drawExternalAngle = true;
        public bool drawInternalAngle = true;
        public bool drawText = true;
        public bool drawVectors = true;
        Vector3 ReferenceRight { get { return transform.rotation * Quaternion.Euler(rotationOffset) * Vector3.right; } }
        Vector3 ReferenceForward { get { return transform.rotation * Quaternion.Euler(rotationOffset) * Vector3.forward; } }
        Vector3 ReferenceUp { get { return transform.rotation * Quaternion.Euler(rotationOffset) * Vector3.up; } }

        //UsefulPointsAndVectors
        float internalAngle = 0.0f;
        float externalAngle = 0.0f;
        Vector3 arcNormal;
        Vector3 hitNormal;
        Vector3 hitPoint;
        Vector3 orthogonalToNormalRight;
        Vector3 orthogonalToNormalUp;
        Quaternion hitRotation;
        bool hitDetected;
        bool visualsNeedClearing;
        void Update()
        {
            var rayDirection = ReferenceForward;
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, Mathf.Infinity))
            {
                hitDetected = true;
                var originRayDirection = ReferenceRight;
                arcNormal = Vector3.Cross(originRayDirection, rayDirection);

                hitNormal = hit.normal;
                hitPoint = hit.point;
                externalAngle = Vector3.Angle(originRayDirection, hitNormal);
                internalAngle = 180 - externalAngle;
                orthogonalToNormalRight = MathHelper.OrthogonalDirectionToDetectedNormal(externalAngle, ReferenceUp, ReferenceForward);
                orthogonalToNormalUp = Vector3.Cross(orthogonalToNormalRight, hitNormal);
                hitRotation = MathHelper.FindHitPointRotation(hitNormal, ReferenceUp, new Vector3(90, 0, 0));
            }
            else
            {
                hitDetected = false;
                if (visualsNeedClearing)
                {
                    visualsNeedClearing = false;
                    externalAngleText.ClearText();
                    internalAngleText.ClearText();
                }
            }

        }
        void OnDrawGizmos()
        {
            if (Validate())
            {
                //Ray origin vectors
                if (drawVectors)
                {
                    DrawHelper.DrawDirection(transform.position, ReferenceForward, Handles.zAxisColor);
                    DrawHelper.DrawDirection(transform.position, ReferenceRight, Handles.xAxisColor);
                    DrawHelper.DrawDirection(transform.position, ReferenceUp, Handles.yAxisColor);
                }
                if (hitDetected)
                {
                    visualsNeedClearing = true;
                    //Ray
                    Debug.DrawLine(transform.position, hitPoint, Color.yellow);
                    //Hit Vectors
                    if (drawVectors)
                    {
                        DrawHelper.DrawDirection(hitPoint, hitNormal, Handles.zAxisColor);
                        DrawHelper.DrawDirection(hitPoint, orthogonalToNormalRight, Handles.xAxisColor);
                        DrawHelper.DrawDirection(hitPoint, orthogonalToNormalUp, Handles.yAxisColor);
                    }
                    //Begining and End Points
                    Gizmos.DrawSphere(transform.position, 0.05f);
                    Gizmos.DrawSphere(hitPoint, 0.05f);
                    //Angle Arcs

                    if (drawExternalAngle)
                        DrawHelper.DrawArc(hitPoint, radius, externalAngle, -arcNormal, -ReferenceForward,DrawHelper.GetColorBasedOnAngle(externalAngle));
                    if (drawInternalAngle)
                        DrawHelper.DrawArc(hitPoint, radius, internalAngle, arcNormal, -ReferenceForward, DrawHelper.GetColorBasedOnAngle(internalAngle));
                    //Angle Text
                    if (drawText)
                    {
                        externalAngleText.DrawText(MathHelper.GetPositionOnCircle(hitPoint, -ReferenceForward, -orthogonalToNormalRight, radius, radiusOfText), hitRotation, $"{ Math.Round(externalAngle, 2)} °");
                        internalAngleText.DrawText(MathHelper.GetPositionOnCircle(hitPoint, -ReferenceForward, orthogonalToNormalRight, radius, radiusOfText), hitRotation, $"{ Math.Round(internalAngle, 2)} °");
                    }
                }
            }
        }
        private bool Validate()
        {
            if (externalAngleText == null || internalAngleText == null)
            {
                print("Missing angle text. Make sure to create text by right clicking Angle visualiser component and Selecting \"Create Angle Text\"");
                return false;
            }
            return true;
        }

        [ContextMenu("Create Angle Text")]
        public void AddText()
        {
            externalAngleText = new TextData(transform, "Angle Visualiser Text - External Angle");
            internalAngleText = new TextData(transform, "Angle Visualiser Text - Internal Angle");
        }
    }
}