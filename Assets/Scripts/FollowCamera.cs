using UnityEngine;

namespace TwoPersonProject
{
    [RequireComponent(typeof(Camera))]
    public class FollowCamera : MonoBehaviour
    {
        public Transform target;
        [Range(0, 1)]
        public float speed;

        private Camera _camera;

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        private void FixedUpdate()
        {
            Vector3 targetPos = Vector3.Lerp(
                _camera.transform.position, 
                target.position, 
                speed
            );
            targetPos.z = _camera.transform.position.z;
            _camera.transform.position = targetPos;
        }
    }
}