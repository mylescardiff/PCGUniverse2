using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PcgUniverse2
{
    public class CameraFacingUI : MonoBehaviour
    {
        [SerializeField]
        private Transform m_camera = null;

        // Update is called once per frame
        void Update()
        {
            transform.rotation = m_camera.transform.rotation;
        }
    }

}

