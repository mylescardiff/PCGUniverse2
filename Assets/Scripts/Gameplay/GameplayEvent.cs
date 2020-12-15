using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PcgUniverse2
{
    /// <summary>
    /// Represents a preprogrammed event that can happen in the game
    /// </summary>
    [CreateAssetMenu(menuName = "Gameplay Event")]
    public class GameplayEvent : ScriptableObject
    {
        private UnityEvent m_eventTriggered = null;
        public UnityEvent eventTriggered { get => m_eventTriggered; set => m_eventTriggered = value; }

        [SerializeField] public int m_foodEffect = 0;
        [SerializeField] public int m_fuelEffect = 0;
        [SerializeField, Multiline] public string m_description;

        public void Trigger(GameManager gameManager)
        {
            gameManager.food += m_foodEffect;
            gameManager.fuel += m_fuelEffect;

            eventTriggered?.Invoke();
        }
    }

}
