using BML.ScriptableObjectCore.Scripts.Variables;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using EmailInbox;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace BML.Scripts {
    public class EmailTaskManager : MonoBehaviour
    {
        [SerializeField] private bool _initialDelay = false;
        
        [SerializeField] private TimerVariable _emailTimer;
        [SerializeField] private TimerVariable _gameTimer;
        [Required, SerializeField] private EmailInboxState _inboxState;
        [FormerlySerializedAs("_emailItems")] [SerializeField] private List<EmailItem> _emailItemChoices;

        private Queue<EmailItem> _spamEmailQueue;
        private Queue<EmailItem> _taskEmailQueue;

        #region Unity lifecycle

        void Awake() 
        {
            _inboxState.ClearInboxItems();

            _spamEmailQueue = new Queue<EmailItem>();
            QueueSpamEmailChoices();
            _taskEmailQueue = new Queue<EmailItem>();
            QueueTaskEmailChoices();
        }

        void OnEnable() {
            if (!_initialDelay)
            {
                OnEmailTimerFinished();
            }
            _emailTimer.SubscribeFinished(OnEmailTimerFinished);

            _emailTimer.StartTimer();
        }

        void OnDisable() {
            _emailTimer.UnsubscribeFinished(OnEmailTimerFinished);

            _emailTimer.ResetTimer();
        }

        void Update() {
            _emailTimer.UpdateTime();
        }

        #endregion

        #region Emails

        private void QueueSpamEmailChoices()
        {
            var addItems = _emailItemChoices
                .Where(emailItem => emailItem.IsSpam)
                .OrderBy(emailItem => Random.value);
            foreach (var emailItem in addItems)
            {
                _spamEmailQueue.Enqueue(emailItem);
            }
        }

        private void QueueTaskEmailChoices()
        {
            var addItems = _emailItemChoices
                .Where(emailItem => !emailItem.IsSpam)
                .OrderBy(emailItem => Random.value);
            foreach (var emailItem in addItems)
            {
                _taskEmailQueue.Enqueue(emailItem);
            }
        }

        #endregion

        private void OnEmailTimerFinished() 
        {
            // Calculate how many emails of each type to add
            int addSpamCount = UnityEngine.Random.value > 0.5 ? 1 : 0;
            int addTaskCount = 1;
            if ((_gameTimer.ElapsedTime / _gameTimer.Duration) >= 0.9) 
            {
                addTaskCount++;
            }
            int addTotalEmails = addTaskCount + addSpamCount;

            if (addSpamCount >= _spamEmailQueue.Count)
            {
                QueueSpamEmailChoices();
            }
            if (addTaskCount >= _taskEmailQueue.Count)
            {
                QueueTaskEmailChoices();
            }

            // Add emails
            for(var i = 0; i < addTotalEmails; i++)
            {
                if(addSpamCount > 0 && addTaskCount > 0) 
                {
                    bool chooseSpam = UnityEngine.Random.value > 0.5 ? true : false;
                    if(chooseSpam) {
                        _inboxState.AddInboxItem(_spamEmailQueue.Dequeue());
                        addSpamCount--;
                    } else {
                        _inboxState.AddInboxItem(_taskEmailQueue.Dequeue());
                        addTaskCount--;
                    }

                    continue;
                }

                if(addTaskCount > 0)
                {
                    _inboxState.AddInboxItem(_taskEmailQueue.Dequeue());
                    addTaskCount--;
                    continue;
                }

                _inboxState.AddInboxItem(_spamEmailQueue.Dequeue());
                addSpamCount--;
            }

            // Reset timer
            _emailTimer.ResetTimer();
            _emailTimer.StartTimer();
        }
    }
}
