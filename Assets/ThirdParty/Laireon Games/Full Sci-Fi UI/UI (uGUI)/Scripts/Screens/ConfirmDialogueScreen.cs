using UnityEngine;
using System.Collections;

namespace LaireonFramework
{
    public class ConfirmDialogueScreen : MonoBehaviour
    {
        public static ConfirmDialogueScreen Instance;

        public TransitionalObject mainTransition;

        void Start()
        {
            Instance = this;

            gameObject.SetActive(false);
        }

        public void Show()
        {
            mainTransition.TriggerTransition();
        }

        public void Close()
        {
            mainTransition.TriggerFadeOut();
        }
    }
}