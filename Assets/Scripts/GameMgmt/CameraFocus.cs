// Author: Myles Cardiff, myles@mylescardiff.com
// Created: 9/3/2020

using UnityEngine;

namespace PcgUniverse2
{

    /// <summary>
    /// Empty object for positioning the camera. The cinemachine vitrual
    /// camera follows this object so moving or rotating it will change view
    /// </summary>
    public class CameraFocus : MonoBehaviour
    {
        [SerializeField] private float m_movementSpeed = 4f;
        [SerializeField] private float m_rotateSpeed = 40f;

        private int m_currentFloor = 0;
        public int currentFloor { get => m_currentFloor; }

        /// <summary>
        /// Updates rotation and position of the camera focus object; The virtual 
        /// cinemachine is linked to this object so it moves when it moves
        /// </summary>
        void Update()
        {

            //// look up / down
            //float vertical = Input.GetAxis("Mouse Y");
            //if (transform.rotation.x > -90f && transform.rotation.y < 90f)
            //    transform.Rotate(vertical * m_mouseLookSpeed, 0, 0);
          

            // rotation
            if (Input.GetKey(KeyCode.Q))
                transform.Rotate(Vector3.up * Time.deltaTime * m_rotateSpeed);

            if (Input.GetKey(KeyCode.E))
                transform.Rotate(-Vector3.up * Time.deltaTime * m_rotateSpeed);

            if (Input.GetKey(KeyCode.UpArrow))
                transform.Translate(Vector3.up * Time.deltaTime * m_movementSpeed);

            if (Input.GetKey(KeyCode.DownArrow))
                transform.Translate(Vector3.down * Time.deltaTime * m_movementSpeed);

            // movement
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");

            if (horizontalAxis == 0 && verticalAxis == 0)
                return;

            Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            Vector3 localRight = transform.worldToLocalMatrix.MultiplyVector(transform.right);
            Vector3 forward = new Vector3(localForward.x, 0f, localForward.z);
            Vector3 desiredMovement = forward * verticalAxis + localRight * horizontalAxis;

            transform.Translate(desiredMovement * Time.deltaTime * m_movementSpeed);
        }
    }

}