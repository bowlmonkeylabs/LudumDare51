using System;
using System.Collections.Generic;
using BML.Scripts.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BML.Scripts
{
    public class BrushTeethMinigameController : MonoBehaviour
    {
        [SerializeField] private MinigameTask _task;
        [SerializeField] private GameObject _plaquePrefab;
        [SerializeField] private int _plaqueCount;
        [SerializeField] private List<Transform> _plaqueSpawnPoints = new List<Transform>();
        
        private List<Plaque> plaques = new List<Plaque>();

        private void Start()
        {
            //Instantiate plants on spawn points with removal
            List<Transform> spawnPointTemp = _plaqueSpawnPoints;
            for (int i = 0; i < _plaqueCount; i++)
            {
                int randIndex = Random.Range(0, spawnPointTemp.Count);
                GameObject plant = Instantiate(_plaquePrefab, spawnPointTemp[randIndex].position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
                plaques.Add(plant.GetComponent<Plaque>());
                spawnPointTemp.Remove(spawnPointTemp[randIndex]);
            }
            
            foreach (var plant in plaques)
            {
                plant.onCleaned += CheckMinigameComplete;
            }
        }

        private void OnDisable()
        {
            foreach (var plant in plaques)
            {
                plant.onCleaned -= CheckMinigameComplete;
            }
        }

        private void CheckMinigameComplete()
        {
            foreach (var plant in plaques)
            {
                if (!plant.IsCleaned)
                    return;
            }
            
            _task.InvokeOnSuccess();
        }
    }
}