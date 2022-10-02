using System;
using System.Collections.Generic;
using System.Linq;
using BML.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BML.Scripts
{
    public class AIMinigameController : MonoBehaviour
    {
        [SerializeField] private MinigameTask _task;
        [SerializeField] private List<SpriteRenderer> _imageOptions = new List<SpriteRenderer>();
        [SerializeField] private List<AiMinigamePreset> _minigamePresets = new List<AiMinigamePreset>();

        private Clickable correctClickable;

        private void Start()
        {
            int randomPrefixIndex = Random.Range(0, _minigamePresets.Count - 1);
            AiMinigamePreset randomPreset = _minigamePresets[randomPrefixIndex];

            _task._taskText = randomPreset.Prompt;
            
            int randomButtonIndex = Random.Range(0, _minigamePresets.Count - 1);
            _imageOptions[randomButtonIndex].sprite = randomPreset.CorrectImage;
            
            correctClickable = _imageOptions[randomButtonIndex].GetComponent<Clickable>();
            correctClickable.SubscribeOnClick(WinMinigame);

            List<SpriteRenderer> remainingOptionsList = _imageOptions;
            remainingOptionsList.Remove(_imageOptions[randomButtonIndex]);
            
            //Assuming 3 images in the list...
            remainingOptionsList[0].sprite = randomPreset.WrongImages[0];
            remainingOptionsList[1].sprite = randomPreset.WrongImages[1];


        }

        private void OnDisable()
        {
            correctClickable.UnSubscribeOnClick(WinMinigame);
        }

        private void WinMinigame()
        {
            _task.InvokeOnSuccess();
        }
    }
}