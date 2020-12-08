using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class PieChart : MonoBehaviour
    {
        public PieSegment segmentPrefab;

        public Color[] tintColors;

        public TextStepper titleStepper; // Array for holding the Text Steppers
        public TextStepper[] textSteppers;

        PieSegment[] generatedSegments;    // Array for holding the generated pie segments

        public Image[] keyIcons;//This tints the colour of the key images

        int pieTotal = 0;    // Statistics

        PieStatData[] pieData;
        // Animation
        public float introSpeed = 2f;

        public void Reset()
        {
            for(int i = 0; i < generatedSegments.Length; i++)
                generatedSegments[i].Reset();
        }

        /// <summary>
        /// Function Plays Pie Chart Fill Animation
        /// </summary>
        public void PlayPieChart()
        {
            // Play the Title Stepper
            titleStepper.PlayStepper(introSpeed);
            // Play the pie chart
            for(int i = 0; i < generatedSegments.Length; i++)
                generatedSegments[i].PlaySegment(introSpeed);

            // Play the Key Steppers
            for(int i = 0; i < textSteppers.Length; i++)
                textSteppers[i].PlayStepper(introSpeed);
        }

        // Function takes array data and converts into a pie display
        public void GeneratePieChart(PieStatData[] _pieData)
        {
            pieData = _pieData;

            for(int i = 0; i < pieData.Length; i++) // Calculate the total
            {
                pieTotal += pieData[i].StatValue;    // Add the value of the stat to the total
                textSteppers[i].FinalValue = pieData[i].StatValue;// Set the value to each Stepper
            }


            titleStepper.FinalValue = pieTotal; // Set the total to the title stepper

            generatedSegments = new PieSegment[pieData.Length];// Declaring the array size

            GameObject temp;

            for(int i = 0; i < pieData.Length; i++)// Create the sprites
            {
                temp = GameObject.Instantiate(segmentPrefab.gameObject);
                temp.transform.SetParent(transform);
                temp.transform.localPosition = Vector3.zero;
                temp.transform.localScale = Vector3.one;
                temp.transform.localRotation = Quaternion.identity;

                generatedSegments[i] = temp.GetComponent<PieSegment>();
                generatedSegments[i].image.color = tintColors[i];

                generatedSegments[i].FillPercentage = (float)pieData[i].StatValue / (float)pieTotal;// Set the fill amount
            }

            // Setting the angles (we ignore the first segment as rotation will be set to zero)
            for(int i = 1; i < pieData.Length; i++)
            {
                // Target the previous stat to work out angle.
                PieStatData previousStat = pieData[i - 1] as PieStatData;
                float rotAngleZ = (((float)previousStat.StatValue / (float)pieTotal) * 360) + generatedSegments[i - 1].transform.localRotation.eulerAngles.z;
                generatedSegments[i].transform.localRotation = Quaternion.Euler(0, 0, rotAngleZ);
            }

            #region Tint Keys
            if(keyIcons.Length > 0)
            {
                for(int i = 0; i < keyIcons.Length; i++)
                    if(keyIcons[i] != null)
                        keyIcons[i].color = tintColors[i];
            }
            #endregion
        }
    }
}