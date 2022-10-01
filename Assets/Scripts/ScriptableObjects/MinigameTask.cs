using BML.ScriptableObjectCore.Scripts.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BML.Scripts.ScriptableObjects
{
    [InlineEditor()]
    [CreateAssetMenu(fileName = "MinigameTask", menuName = "BML/Minigame/MinigameTask", order = 0)]
    public class MinigameTask : ScriptableObject
    {
        public bool isCurrentTask;
        public string _taskText;
        public GameEvent _successEvent;
    }
}