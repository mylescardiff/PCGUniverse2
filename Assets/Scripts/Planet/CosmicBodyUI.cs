using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PcgUniverse2
{
    public class CosmicBodyUI : MonoBehaviour
    {
        [SerializeField] private Text m_nameText = null;

        public string objectName
        {
            get => m_nameText.text;
            set => m_nameText.text = value;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}


