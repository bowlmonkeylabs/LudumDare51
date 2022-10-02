using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace BML.Scripts
{
    public class Plaque : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _plaqueSpots = new List<GameObject>();
        
        public delegate void OnCleaned();
        public event OnCleaned onCleaned;

        public UnityEvent _OnCleaned;
        public bool IsCleaned => isCleaned;

        private bool isCleaned;
        
        private List<GameObject> _plaqueSpotsModify = new List<GameObject>();

        private void Start()
        {
            _plaqueSpotsModify = _plaqueSpots;
        }

        public void DoClean()
        {
            if (_plaqueSpotsModify.Count == 0)
            {
                SetCleaned();
                return;
            }
                

            int randomIndex = Random.Range(0, _plaqueSpotsModify.Count);
            _plaqueSpotsModify[randomIndex].SetActive(false);
            _plaqueSpotsModify.RemoveAt(randomIndex);
        }

        public void SetCleaned()
        {
            isCleaned = true;
            _OnCleaned.Invoke();
            onCleaned?.Invoke();
        }
    }
}