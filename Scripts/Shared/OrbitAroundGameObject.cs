using UnityEngine;
namespace AngleVisualiser.Shared
{
    public class OrbitAroundGameObject : MonoBehaviour
    {
        public Transform target;
        public Vector3 axisOfRotation;
        void Update()
        {
            transform.position = target.position + new Vector3(Mathf.Sin(Time.time) * axisOfRotation.x,
                                                                    Mathf.Cos(Time.time) * axisOfRotation.y,
                                                                    Mathf.Cos(Time.time) * axisOfRotation.z);
        }
    }
}