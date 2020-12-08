using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class UpgradesButton : MonoBehaviour
    {
        #region Variables
        public Image buttonFrame;
        public Image buttonIcon;
        public Image buttonGlow;
        public Text buttonLabel;

        public Gradient labelGradient;

        public Color enabledGradientTop;
        public Color enabledGradientBase;

        public Color disabledGradientTop;
        public Color disabledGradientBase;

        public Sprite disabledFrame, enabledFrame;
        public Sprite disabledIcon, enabledIcon;
        public Sprite disabledGlow, enabledGlow;

        public bool sufficientCredits = false;//this is an example. Replace with your check 
        #endregion

        #region Methods
        void Start()
        {
            CheckCredits();//see if you can afford upgrades
        }

        /// <summary>
        /// Function switches button to disabled state (red) changes navigation to store.
        /// </summary>
        public void DisableUpgrades()
        {
            buttonFrame.overrideSprite = disabledFrame;// Set the frame sprite

            labelGradient.vertex1 = disabledGradientTop;// Set the label gradient
            labelGradient.vertex2 = disabledGradientBase;

            buttonIcon.overrideSprite = disabledIcon;// Set the icon

            buttonGlow.overrideSprite = disabledGlow;// Set the glow sprite

        }

        /// <summary>
        /// Function switches button to enabled state (green) allows purchase of Upgrades
        /// </summary>
        public void EnableUpgrades()
        {
            buttonFrame.overrideSprite = enabledFrame;//Set the frame sprite

            labelGradient.vertex1 = enabledGradientTop;//Set the label gradient
            labelGradient.vertex2 = enabledGradientBase;

            buttonIcon.overrideSprite = enabledIcon;//Set the icon

            buttonGlow.overrideSprite = enabledGlow;//Set the glow sprite

        }

        /// <summary>
        ///  Function checks credits and changes functionality of button to suit
        /// </summary>
        public void CheckCredits()
        {
            if(sufficientCredits)
                EnableUpgrades();
            else
                DisableUpgrades();
        }
        #endregion
    }
}