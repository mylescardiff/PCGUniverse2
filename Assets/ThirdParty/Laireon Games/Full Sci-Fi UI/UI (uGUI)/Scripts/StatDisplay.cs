using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class StatDisplay : MonoBehaviour
    {
        public Text nameLabel;
        public TextStepper valueLabel;

        public void InitStatDisplay(string _name, float _value, string _format)
        {
            switch(_format)
            {
                case "normal":
                    valueLabel.valueFormat = TextStepper.ValueFormat.Normal;
                    break;
                case "time":
                    valueLabel.valueFormat = TextStepper.ValueFormat.TimeFormat;
                    break;
            }

            nameLabel.text = _name;
            valueLabel.FinalValue = _value;
        }

        public void RefreshDisplay(float _duration)
        {
            valueLabel.PlayStepper(_duration);
        }
    }
}