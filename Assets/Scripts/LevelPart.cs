using UnityEngine;

namespace TwoPersonProject
{
    public class LevelPart : MonoBehaviour
    {
        public Vector2 leftConnection;
        public Vector2 rightConnection;

        private void FixedUpdate()
        {
            // Destroy self if camera is far enough ahead
            if (transform.position.x - Camera.main.transform.position.x < -50)
            {
                Destroy(gameObject);
            }
        }
    }
}