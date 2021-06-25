using UnityEngine;
namespace AngleVisualiser
{
    public class RotateGameObject : MonoBehaviour
    {
        public Vector3 Rotation;
        public float speed = 10;

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Rotation * speed * Time.deltaTime);
        }
    }
}