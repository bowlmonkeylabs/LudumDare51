using BML.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace BML.Scripts
{
    public class MinigameTaskEvent : MonoBehaviour
    {
        [SerializeField] private MinigameTask _task;
        [SerializeField] private UnityEvent _onIsCurrentTask;
        [SerializeField] private UnityEvent _onIsNotCurrentTask;

        public void Raise()
        {
            if (_task.isCurrentTask)
                _onIsCurrentTask.Invoke();
            else
                _onIsNotCurrentTask.Invoke();
        }
    }
}