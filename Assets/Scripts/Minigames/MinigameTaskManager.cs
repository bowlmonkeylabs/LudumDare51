using System;
using System.Collections.Generic;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.Variables;
using BML.Scripts.ScriptableObjects;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BML.Scripts
{
    public class MinigameTaskManager : MonoBehaviour
    {
        [SerializeField] private GameEvent _onMinigameCompleted;
        [SerializeField] private GameEvent _onMinigameFailed;
        [SerializeField] private GameEvent _onMinigameEnded;
        [SerializeField] private BoolVariable _minigameSucceeded;

        [SerializeField] private MMF_Player _onSuccceededFeedbacks;
        [SerializeField] private MMF_Player _onFailedFeedbacks;
        
        [SerializeField] private TMP_Text _taskText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private MinigameTaskContainer _minigameTaskContainer;

        [ShowInInspector, ReadOnly] private List<MinigameTask> Tasks;

        private MinigameTask currentTask;
        private float minigameStartTime = Mathf.NegativeInfinity;
        private bool isMiniGameStarted;

        #region Unity Lifecycle

        private void Start()
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
            
            if (isMiniGameStarted)
            {
                float remainingTime = Mathf.Max(0f,
                    _minigameTaskContainer._timeToComplete - (Time.time - minigameStartTime));
                _timerText.text = remainingTime.ToString("0.0");
            }
        }

        #endregion

        #region Tasks

        private void StartNextTask()
        {
            if (currentTask != null)
            {
                // Unsubscribe previous task from success method
                currentTask.UnSubscribeOnSuccess(TaskSuccess);
                currentTask.isCurrentTask = false;
            }
            
            currentTask = GetNextTask();
            currentTask.SubscribeOnSuccess(TaskSuccess);
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
            Tasks = (_minigameTaskContainer.RandomizeTaskOrder)
                ? _minigameTaskContainer._minigameTasks.OrderBy(t => Random.value).ToList()
                : _minigameTaskContainer._minigameTasks;

            _minigameSucceeded.Value = false;
            isMiniGameStarted = true;
            minigameStartTime = Time.time;

            CleanUp();
            StartNextTask();
        }

        private void OnMinigameComplete()
        {
            Debug.Log($"Minigame SUCCEEDED");

            _minigameSucceeded.Value = true;
            _onMinigameCompleted.Raise();
            isMiniGameStarted = false;
            _taskText.text = _minigameTaskContainer._winText;
            CleanUp();
            
            // NEED to call OnMinigameEnded() at the end of these feedbacks !!!
            if (_onSuccceededFeedbacks != null) _onSuccceededFeedbacks.PlayFeedbacks();
            else OnMinigameEnded();
        }
        
        private void OnMinigameFailed()
        {
            Debug.Log($"Minigame FAILED");

            _minigameSucceeded.Value = false;
            _onMinigameFailed.Raise();
            isMiniGameStarted = false;
            _taskText.text = _minigameTaskContainer._loseText;
            CleanUp();
            
            // NEED to call OnMinigameEnded() at the end of these feedbacks !!!
            if (_onFailedFeedbacks != null) _onFailedFeedbacks.PlayFeedbacks();
            else OnMinigameEnded();
        }

        public void OnMinigameEnded()
        {
            Debug.Log($"Minigame ENDED");
            
            _minigameSucceeded.Value = true;
            _onMinigameEnded.Raise();
        }

        // Don't leave any event subscriptions active
        private void CleanUp()
        {
            foreach (var task in Tasks)
            {
                task.UnSubscribeOnSuccess(TaskSuccess);
                task.isCurrentTask = false;
            }
        }

        #endregion
    }
}