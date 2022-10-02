using System;
using System.Collections.Generic;
using BML.Scripts.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BML.Scripts
{
    public class GardeningMinigameController : MonoBehaviour
    {
        [SerializeField] private MinigameTask _task;
        [SerializeField] private GameObject _plantPrefab;
        [SerializeField] private int _plantCount;
        [SerializeField] private List<Transform> _plantSpawnPoints = new List<Transform>();
        
        private List<Plant> plants = new List<Plant>();

        private void Start()
        {
            //Instantiate plants on spawn points with removal
            List<Transform> spawnPointTemp = _plantSpawnPoints;
            for (int i = 0; i < _plantCount; i++)
            {
                int randIndex = Random.Range(0, spawnPointTemp.Count);
                GameObject plant = Instantiate(_plantPrefab, spawnPointTemp[randIndex]);
                plants.Add(plant.GetComponent<Plant>());
                spawnPointTemp.Remove(spawnPointTemp[randIndex]);
            }
            
            foreach (var plant in plants)
            {
                plant.onWatered += CheckMinigameComplete;
            }
        }

        private void OnDisable()
        {
            foreach (var plant in plants)
            {
                plant.onWatered -= CheckMinigameComplete;
            }
        }

        private void CheckMinigameComplete()
        {
            foreach (var plant in plants)
            {
                if (!plant.IsWatered)
                    return;
            }
            
            _task.InvokeOnSuccess();
        }
    }
}