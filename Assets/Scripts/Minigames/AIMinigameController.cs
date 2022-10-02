using System;
using System.Collections.Generic;
using System.Linq;
using BML.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BML.Scripts
{
    public class AIMinigameController : MonoBehaviour
    {
        #region Inspector
        
        [SerializeField] private MinigameTask _task;
        [FormerlySerializedAs("_imageOptions")] [SerializeField] private List<SpriteRenderer> _imageRenderers = new List<SpriteRenderer>();
        [SerializeField] private List<AiMinigamePreset> _minigamePresets = new List<AiMinigamePreset>();

        #endregion
        
        private Clickable correctClickable;

        #region Unity lifecycle
        
        private void Awake()
        {
            InitializeGame();
        }

        private void OnDisable()
        {
            correctClickable.UnSubscribeOnClick(WinMinigame);
        }

        #endregion
        
        #region Game

        private void InitializeGame()
        {
            // Pick a random prompt and set of images
            int randomPresetIndex = Random.Range(0, _minigamePresets.Count - 1);
            AiMinigamePreset randomPreset = _minigamePresets[randomPresetIndex];

            // Set task text
            _task._taskText = randomPreset.Prompt;
            
            // Randomly choose where the "correct" image is located
            int correctButtonIndex = Random.Range(0, _imageRenderers.Count);
            _imageRenderers[correctButtonIndex].sprite = randomPreset.CorrectImage;
            correctClickable = _imageRenderers[correctButtonIndex].GetComponent<Clickable>();
            correctClickable.SubscribeOnClick(WinMinigame);

            // Assign the "incorrect" images to the remaining slots
            List<SpriteRenderer> remainingOptionsList = _imageRenderers
                .Where(ir => ir != _imageRenderers[correctButtonIndex])
                .OrderBy(ir => Random.value)
                .ToList();
            //Assuming 3 images in the list...
            remainingOptionsList[0].sprite = randomPreset.WrongImages[0];
            remainingOptionsList[1].sprite = randomPreset.WrongImages[1];
        }
        
        private void WinMinigame()
        {
            _task.InvokeOnSuccess();
        }
        
        #endregion
        
    }
}