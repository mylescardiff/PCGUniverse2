using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace LaireonFramework
{
    public class LoadScene : MonoBehaviour
    {
        public string sceneName;

        public void LoadSceneOnCLick()
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}