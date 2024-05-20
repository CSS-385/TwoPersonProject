using UnityEngine;

namespace TwoPersonProject
{
    public class Ship : MonoBehaviour
    {
        public float speedMult;
        public float speedPow;
        public float timeMult;
        public float timeOffset;

        [Range(0f, 1f)]
        public float ySpeed;
        public float yOffset;
        public Transform target;

        private void Update()
        {
            float speed = Mathf.Pow((Time.time + timeOffset) * timeMult, speedPow) * speedMult;
            transform.position += speed * Time.deltaTime * Vector3.right;
        }

        private void FixedUpdate()
        {
            transform.position = new Vector3(
                transform.position.x,
                Mathf.Lerp(transform.position.y, target.position.y + yOffset, ySpeed)
            );
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            collision.attachedRigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}