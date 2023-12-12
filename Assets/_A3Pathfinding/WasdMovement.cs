using UnityEngine;

namespace _A3Pathfinding
{
    public class WasdMovement : MonoBehaviour
    {
        public float speed = 10f;

        public float sensitivity = 1f;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            Transform transform1;
            (transform1 = transform).position += transform.forward *
                                                 (Input.GetAxis("Vertical") *
                                                  speed * Time.deltaTime);
            transform.position += transform1.right *
                                  (Input.GetAxis("Horizontal") * speed *
                                   Time.deltaTime);

            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");
            transform.eulerAngles +=
                new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);
        }
    }
}