using System;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace BML.Scripts
{
    public class MinigameTaskManager : MonoBehaviour
    {
        [SerializeField] private GameEvent _onMinigameCompleted;
        [SerializeField] private GameEvent _onMinigameFailed;
        [SerializeField] private TMP_Text _taskText;
        [SerializeField] private MinigameTaskContainer _minigameTaskContainer;

        private List<MinigameTask> Tasks => _minigameTaskContainer._minigameTasks;
        
        private MinigameTask currentTask;
        private float minigameStartTime = Mathf.NegativeInfinity;
        private bool isMiniGameStarted;

        #region Unity Lifecycle

        private void Awake()
        {
            StartMinigame();
        }

        private void OnDisable()
        {
            CleanUp();
        }

        private void Update()
        {
            // If run out of time
            if (isMiniGameStarted &&
                minigameStartTime + _minigameTaskContainer._timeToComplete < Time.time)
                OnMinigameFailed();
        }

        #endregion

        #region Tasks

        private void StartNextTask()
        {
            if (currentTask != null)
            {
                // Unsubscribe previous task from success method
                currentTask._successEvent.Unsubscribe(TaskSuccess);
                currentTask.isCurrentTask = false;
            }
            
            currentTask = GetNextTask();
            currentTask._successEvent.Subscribe(TaskSuccess);
            currentTask.isCurrentTask = true;
            
            SetTaskText();
        }

        private MinigameTask GetNextTask()
        {
            // If first task
            if (currentTask == null)
                return Tasks[0];
            
            int nextTaskIndex = Tasks.IndexOf(currentTask) + 1;

            return Tasks[nextTaskIndex];
        }

        private void TaskSuccess()
        {
            //If this is the last task, complete game
            if (Tasks.IndexOf(currentTask) == Tasks.Count - 1)
                OnMinigameComplete();
            else
                StartNextTask();
        }

        private void SetTaskText()
        {
            _taskText.text = currentTask._taskText;
        }

        #endregion

        #region Minigame State

        private void StartMinigame()
        {
            isMiniGameStarted = true;
            minigameStartTime = Time.time;

            CleanUp();
            StartNextTask();
        }

        private void OnMinigameComplete()
        {
            _onMinigameCompleted.Raise();
            isMiniGameStarted = false;
            CleanUp();
        }
        
        private void OnMinigameFailed()
        {
            _onMinigameFailed.Raise();
            isMiniGameStarted = false;
            CleanUp();
        }

        // Don't leave any event subscriptions active
        private void CleanUp()
        {
            foreach (var task in Tasks)
            {
                task._successEvent.Unsubscribe(TaskSuccess);
                task.isCurrentTask = false;
            }
        }

        #endregion
    }
}