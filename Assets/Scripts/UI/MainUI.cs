using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PcgUniverse2
{
    public class MainUI : MonoBehaviour
    {
        [SerializeField] private GameObject m_dialogBox = null;
        [SerializeField] private Text m_dialogText = null;
        [SerializeField] private Text m_fuelLabel = null;
        [SerializeField] private Text m_foodLabel = null;
        [SerializeField] private GameObject m_textOutputBase = null;

        public void ShowMessage(string messageText)
        {
            if (m_dialogBox == null || m_dialogText == null)
                return;

            m_dialogText.text = messageText;
            m_dialogBox.SetActive(true);

        }

        public void UpdateUI(float food, float fuel)
        {
            m_fuelLabel.text = fuel.ToString("N0");
            m_foodLabel.text = food.ToString("N0");
        }
    }

}
