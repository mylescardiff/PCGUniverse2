using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PcgUniverse2
{

    public static class Utility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int WeightedRandom(IWeightedItem[] list)
        {

            float sumOfWeights = list.Sum(x => x.GetWeight());

            float randomRoll = Random.Range(0, sumOfWeights);
            for (int i = 0; i < list.Count(); i++)
            {
                if (randomRoll < list[i].GetWeight())
                {
                    return i;
                }
                randomRoll -= list[i].GetWeight();
            }

            return -1; // this should not happen

        }

        public static Gradient CreateGradient(Color[] colors, float[] times, float[] alphas)
        {
            Gradient grad = new Gradient();
            grad.colorKeys = new GradientColorKey[colors.Length];
            grad.alphaKeys = new GradientAlphaKey[alphas.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                grad.colorKeys[i].color = colors[i];
                grad.colorKeys[i].time = times[i];
            }
            for (int i = 0; i < alphas.Length; i++)
            {
                grad.alphaKeys[i].alpha = alphas[i];
            }

            return grad;
        }

        /// <summary>
        /// Randomly shuffles a list of any type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void ShuffleList<T>(List<T> list)
        {
            list = list.OrderBy(x => System.Guid.NewGuid()).ToList();
        }
    }

}
