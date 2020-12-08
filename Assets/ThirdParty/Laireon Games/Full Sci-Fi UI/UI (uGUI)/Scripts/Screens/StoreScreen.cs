using UnityEngine;
using System.Collections;

namespace LaireonFramework
{
    public class StoreScreen : MonoBehaviour
    {
        public static StoreScreen Instance;

        public TransitionGroup mainTransition;

        void Start()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        public void Show()
        {
            mainTransition.TriggerGroupTransition();
            Shutters.Instance.Show();
            MainMenu.Instance.Minimise();
        }

        public void Close()
        {
            mainTransition.TriggerGroupFadeOut();
            Shutters.Instance.Close();
            MainMenu.Instance.Show();
        }
    }
}
