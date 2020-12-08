using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PcgUniverse2
{

    public class LoadingScreen : MonoBehaviour
    {

        private GameObject m_loadingScreenCanvas = null;
        private void Start()
        {
            m_loadingScreenCanvas = GameObject.FindGameObjectWithTag("LoadingScreen");
        }

        public void LoadInstructions()
        {
            GameManager.GetInstance().LoadScene(GameManager.ESceneIndex.kInstructions, true);

            if (m_loadingScreenCanvas != null)
            {
                m_loadingScreenCanvas.SetActive(false);
            }

        }

    }

}
