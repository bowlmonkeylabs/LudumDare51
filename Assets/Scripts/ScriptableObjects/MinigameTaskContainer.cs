using System;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BML.Scripts.ScriptableObjects
{
    [InlineEditor()]
    [CreateAssetMenu(fileName = "MinigameTaskContainer", menuName = "BML/Minigame/MinigameTaskContainer", order = 0)]
    public class MinigameTaskContainer : ScriptableObject
    {
        public float _timeToComplete = 5f;
        public string _winText = "You Win!";
        public string _loseText = "You Lose!";
        public List<MinigameTask> _minigameTasks = new List<MinigameTask>();
    }
}