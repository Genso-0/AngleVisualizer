using System;
using TMPro;
using UnityEditor;
using UnityEngine;

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
    Color arcColorObtuse = new Color(0, 0.7f, 0.0f, 0.1f);
    Color arcColorAcute = new Color(0.7f, 0, 0.0f, 0.1f);
    Color arcColorRight = new Color(0.0f, 0, 0.7f, 0.1f);

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
            orthogonalToNormalRight = OrthogonalDirectionToDetectedNormal(externalAngle, ReferenceUp, ReferenceForward);
            orthogonalToNormalUp = Vector3.Cross(orthogonalToNormalRight, hitNormal);
            hitRotation = FindHitPointRotation();
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
                DrawDirection(transform.position, ReferenceForward, Handles.zAxisColor);
                DrawDirection(transform.position, ReferenceRight, Handles.xAxisColor);
                DrawDirection(transform.position, ReferenceUp, Handles.yAxisColor);
            }
            if (hitDetected)
            {
                visualsNeedClearing = true;
                //Ray
                Debug.DrawLine(transform.position, hitPoint, Color.yellow);
                //Hit Vectors
                if (drawVectors)
                {
                    DrawDirection(hitPoint, hitNormal, Handles.zAxisColor);
                    DrawDirection(hitPoint, orthogonalToNormalRight, Handles.xAxisColor);
                    DrawDirection(hitPoint, orthogonalToNormalUp, Handles.yAxisColor);
                }
                //Begining and End Points
                Gizmos.DrawSphere(transform.position, 0.05f);
                Gizmos.DrawSphere(hitPoint, 0.05f);
                //Angle Arcs
                if (drawExternalAngle)
                    DrawArc(hitPoint, externalAngle, -arcNormal, -ReferenceForward);
                if (drawInternalAngle)
                    DrawArc(hitPoint, internalAngle, arcNormal, -ReferenceForward);
                //Angle Text
                if (drawText)
                {
                    externalAngleText.DrawText(GetPositionOnCircle(hitPoint, -orthogonalToNormalRight), hitRotation, $"{ Math.Round(externalAngle, 2)} °");
                    internalAngleText.DrawText(GetPositionOnCircle(hitPoint, orthogonalToNormalRight), hitRotation, $"{ Math.Round(internalAngle, 2)} °");
                    //DrawText(hitPoint);
                }
            }
            //else if (visualsNeedClearing)
            //{
            //    visualsNeedClearing = false;
            //    externalAngleText.ClearText();
            //    internalAngleText.ClearText();
            //}
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


    private Vector3 GetPositionOnCircle(Vector3 hitPoint, Vector3 prthogonalToNormalRight)
    {
        var result = hitPoint + ((prthogonalToNormalRight - ReferenceForward) * radius * radiusOfText);
        //Gizmos.DrawSphere(result, 0.1f);
        return result;
    }
    private Quaternion FindHitPointRotation()
    {
        var resultRotation = Quaternion.LookRotation(hitNormal, -ReferenceUp);
        resultRotation *= Quaternion.Euler(new Vector3(90, 0, 0));
        return resultRotation;
    }
    private Vector3 FindPointOnArcCenterLine(Vector3 centerBaseOfChord, Vector3 centerline, float arcRadius)
    {
        var result = centerBaseOfChord + (-centerline * arcRadius);
        //Gizmos.DrawSphere(result, 0.1f);
        return result;
    }
    private Vector3 OrthogonalDirectionToDetectedNormal(float angle, Vector3 angleAxis, Vector3 dir)
    {
        var result = Quaternion.AngleAxis(angle, angleAxis) * dir;
        //Gizmos.DrawSphere(result, 0.1f);
        return result;
    }
    private void DrawDirection(Vector3 start, Vector3 direction, Color color)
    {
        Handles.color = color;
        Handles.ArrowHandleCap(
            0,
            start,
            Quaternion.LookRotation(direction),
            1,
            EventType.Repaint
        );
    }
    void DrawArc(Vector3 pos, float angle, Vector3 normal, Vector3 from)
    {
        if (angle > 90f)
        {
            //obtuse angle
            UnityEditor.Handles.color = arcColorObtuse;
        }
        else if (angle < 90)
        {
            //acute angle
            UnityEditor.Handles.color = arcColorAcute;
        }
        else if (angle == 90)
        {
            //right angle
            UnityEditor.Handles.color = arcColorRight;
        }
        UnityEditor.Handles.DrawSolidArc(pos, normal, from, angle, radius);
        UnityEditor.Handles.color = Color.white;
    }

    [ContextMenu("Create Angle Text")]
    public void AddText()
    {
        externalAngleText = new TextData(transform, "External Angle");
        internalAngleText = new TextData(transform, "Internal Angle");
    }
    [Serializable]
    public class TextData
    {
        public Canvas canvas;
        public TMP_Text description;
        public TextData(Transform parent, string name)
        {
            CreateTextData(parent, name);
        }
        public void ClearText()
        {
            description.SetText("");
        }
        public void DrawText(Vector3 worlPosition, Quaternion worldRotation, string value)
        {
            canvas.transform.position = worlPosition;

            canvas.transform.rotation = worldRotation;

            description.SetText(value);
        }
        void CreateTextData(Transform parent, string name)
        {
            //Canvas
            var canvasObj = new GameObject($"Angle Visualiser Canvas - {name}");
            canvasObj.transform.SetParent(parent);
            canvasObj.transform.localPosition = Vector3.zero;

            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            var rect = canvas.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(.5f, .2f);

            //Text
            var textObj = new GameObject($"Angle Visualiser Text - {name}");
            textObj.transform.SetParent(canvasObj.transform);
            textObj.transform.localPosition = Vector3.zero;

            var tmpUGUI = textObj.AddComponent<TextMeshProUGUI>();
            tmpUGUI.fontSize = 0.1f;
            tmpUGUI.color = Color.black;
            var textRect = tmpUGUI.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.sizeDelta = new Vector2(0, 0);

            this.canvas = canvas;
            this.description = tmpUGUI.GetComponent<TMP_Text>();
        }
    }
}
