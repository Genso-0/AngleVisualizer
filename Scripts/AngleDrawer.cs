using AngleVisualiser.Shared;
using System;
using UnityEditor;
using UnityEngine;

namespace AngleVisualiser
{
    [ExecuteAlways]
    public class AngleDrawer : MonoBehaviour
    {
        public Color textColor = Color.white;
        public bool drawText;
        public bool drawNormal;
        [SerializeField] TextData angleText;
        bool needsClearing;
        ArcData data; 
        public void UpdateAngleData(ArcData data) => this.data = data;
         
        void Update()
        {
            if (Validate() && drawText)
                DrawAngleText();
            else if (needsClearing)
                angleText.description.SetText("");
        }
        void OnDrawGizmos()
        { 
            DrawArc();  
        }
        void DrawArc()
        {
            UnityEditor.Handles.color = data.color;
            UnityEditor.Handles.DrawSolidArc(data.center, data.normal, data.from, data.angle, data.radius);
            UnityEditor.Handles.color = Color.white;

            if (drawNormal) DrawHelper.DrawDirection(data.center, data.normal, Handles.yAxisColor);
        }
        void DrawAngleText()
        {
            needsClearing = true; 
            var textRotation = Quaternion.LookRotation(-data.normal, data.from);

            textRotation = FlipUp(textRotation);

            //changing the angle to the positive range for display on the text.
            var modifiedAngleToTarget = data.angle < 0 ? data.angle * -1 : data.angle; 
            ///To find the position of the text on the arc we
            ///Add the center offset
            ///Rotate the "forward" vector of the begining of the arc by a quaternion
            ///This quaternion is calculated using the Angle Axis method and passing in the angle * half and the normal of the arc (This normal acts as an axis to rotate around of) 
            var textPosition = data.center + (Quaternion.AngleAxis(data.angle * .5f, data.normal) * data.from);
            angleText.description.color = textColor; 
            angleText.DrawText(textPosition, textRotation, $"{Math.Round(modifiedAngleToTarget, 2)} °");
        }

        /// <summary>
        /// Checks to see if the text is facing downwards to flip it up
        /// </summary>
        /// <param name="textRotation"></param>
        /// <returns></returns>
        private Quaternion FlipUp(Quaternion textRotation)
        {
            var rotatedForward = textRotation * Vector3.forward;
            if (Vector3.Dot(rotatedForward, Vector3.up) > 0)
                return textRotation *= Quaternion.Euler(new Vector3(0, 180, 0));
            else return textRotation;
        }

        private bool Validate()
        {
            if (angleText == null)
            {
                print($"Missing text. Make sure to create text by right clicking {this} component and selecting \"Create Text\"");
                return false;
            }
            return true;
        }
        [ContextMenu("Create Text")]
        public void AddText()
        {
            angleText = new TextData(transform, "Angle Visualiser Text - Signed Angle To Target");
        }
    }
    public readonly struct ArcData
    {
        public readonly Vector3 center;
        public readonly Vector3 normal;
        public readonly Vector3 from;
        public readonly Color color;
        public readonly float radius;
        public readonly float angle;

        public ArcData(Vector3 center, Vector3 normal, Vector3 from, Color color, float radius, float angle)
        {
            this.center = center;
            this.normal = normal;
            this.from = from;
            this.color = color;
            this.radius = radius;
            this.angle = angle;
        }
    }
}
