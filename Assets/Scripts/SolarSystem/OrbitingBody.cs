using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PcgUniverse2
{

    public class OrbitingBody : MonoBehaviour
    {
        [SerializeField] private float m_orbitSpeed = 2f;
        public float orbitSpeed { get => m_orbitSpeed; set => m_orbitSpeed = value;  }

        private void Start()
        {

        }

        private void FixedUpdate()
        {
            transform.RotateAround(Vector3.zero, Vector3.up, m_orbitSpeed * Time.deltaTime);
        }

    }

}
