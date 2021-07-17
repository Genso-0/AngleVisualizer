using UnityEditor;
using UnityEngine;

namespace AngleVisualiser.Shared
{
    public static class DrawHelper
    {
        static readonly Color arcColorObtuse = new Color(0, 0.7f, 0.0f, 0.1f);
        static readonly Color arcColorAcute = new Color(0.7f, 0, 0.0f, 0.1f);
        static readonly Color arcColorRight = new Color(0.0f, 0, 0.7f, 0.1f);
        public static void DrawArc(Vector3 pos, float radius, float angle, Vector3 normal, Vector3 from, Color color)
        { 
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawSolidArc(pos, normal, from, angle, radius);
            UnityEditor.Handles.color = Color.white;
        }
        public static Color GetColorBasedOnAngle(in float angle)
        {
            if (angle > 90f)
            {
                //obtuse angle
                return arcColorObtuse;
            }
            else if (angle < 90)
            {
                //acute angle
                return arcColorAcute;
            }
            //right angle
            return arcColorRight;
        }
        public static void DrawDirection(Vector3 start, Vector3 direction, Color color)
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
    }
}
