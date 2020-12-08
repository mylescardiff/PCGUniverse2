using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PcgUniverse2
{
    public class Spin : MonoBehaviour
    {
        [SerializeField]
        private float m_speed = 1;
        public float speed { get => m_speed; set => m_speed = value; }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0f, m_speed * Time.deltaTime, 0f);
        }
    }
}


