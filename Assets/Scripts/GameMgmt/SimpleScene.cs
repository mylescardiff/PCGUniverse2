using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PcgUniverse2
{
    public class SimpleScene : MonoBehaviour
    {
        [SerializeField] private GameManager.ESceneIndex m_nextScene = GameManager.ESceneIndex.kNone;
        [SerializeField] private bool m_unloadCurrent = true;

        public void LoadNext()
        {
            GameManager.GetInstance().LoadScene(m_nextScene, m_unloadCurrent);
        }
       

    }

}
