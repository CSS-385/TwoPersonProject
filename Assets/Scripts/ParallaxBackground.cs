using UnityEngine;

namespace TwoPersonProject
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ParallaxBackground : MonoBehaviour
    {
        [Range(0.01f, 20)]
        public float distance;
        public Camera target;

        private SpriteRenderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            // Teleport if camera goes to the left or right of bounds
            Bounds spriteBounds = _renderer.sprite.bounds;
            spriteBounds.center = transform.position;
            spriteBounds.size = Vector3.Scale(spriteBounds.size, transform.lossyScale);

            float camHeight = target.orthographicSize * 2;
            float camWidth = camHeight * Screen.width / Screen.height;
            Bounds cameraBounds = new(target.transform.position, new Vector3(camWidth, camHeight));
            if (cameraBounds.max.x > spriteBounds.max.x + spriteBounds.extents.x)
            {
                transform.position += spriteBounds.size.x * Vector3.right;
            }
            else if (cameraBounds.min.x < spriteBounds.min.x - spriteBounds.extents.x)
            {
                transform.position += spriteBounds.size.x * Vector3.left;
            }

            float delta = target.velocity.x * Time.deltaTime / distance;
            transform.position = new Vector3(
                delta + transform.position.x,
                target.transform.position.y
            );
        }
    }
}