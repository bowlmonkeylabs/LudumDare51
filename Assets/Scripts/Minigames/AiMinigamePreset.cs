using System.Collections.Generic;
using UnityEngine;

namespace BML.Scripts
{
    [CreateAssetMenu(fileName = "AiMinigamePreset/", menuName = "BML/Minigame/AiMinigamePreset", order = 0)]
    public class AiMinigamePreset : ScriptableObject
    {
        public string Prompt = "Pick the correct picture";
        public Sprite CorrectImage;
        public List<Sprite> WrongImages;
    }
}