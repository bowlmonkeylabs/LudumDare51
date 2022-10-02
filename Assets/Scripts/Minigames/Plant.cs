using UnityEngine;
using UnityEngine.Events;

namespace BML.Scripts
{
    public class Plant : MonoBehaviour
    {
        public delegate void OnWatered();
        public event OnWatered onWatered;

        public UnityEvent _OnWatered;
        public bool IsWatered => isWatered;

        private bool isWatered;

        public void SetWatered()
        {
            isWatered = true;
            _OnWatered.Invoke();
            onWatered?.Invoke();
        }
    }
}