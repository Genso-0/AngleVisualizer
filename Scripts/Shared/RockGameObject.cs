
using UnityEngine;
namespace AngleVisualiser
{
    public class RockGameObject : MonoBehaviour
    {
        public enum AxisAngleRotation
        {
            Up, Right, Forward
        }
        [SerializeField] AxisAngleRotation axisAngleRotation = AxisAngleRotation.Right;
        [SerializeField] float speed = 1;
        [SerializeField] float magnitude = 1;

        // Update is called once per frame
        void Update()
        {
            float newX = 0;
            float newY = 0;
            float newZ = 0;
            switch (axisAngleRotation)
            {
                case AxisAngleRotation.Up:
                    newY = Mathf.Sin(Time.unscaledTime * speed) * magnitude;
                    break;
                case AxisAngleRotation.Right:
                    newX = Mathf.Sin(Time.unscaledTime * speed) * magnitude;
                    break;
                case AxisAngleRotation.Forward:
                    newZ = Mathf.Sin(Time.unscaledTime * speed) * magnitude;
                    break;
            }
            var newRotationEulers = new Vector3(newX, newY, newZ);
            transform.rotation = Quaternion.Euler(newRotationEulers);
        }
    }
}