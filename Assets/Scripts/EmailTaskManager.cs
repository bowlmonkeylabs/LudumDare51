using BML.ScriptableObjectCore.Scripts.Variables;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using EmailInbox;
using System;
using System.Collections.Generic;
using BML.Scripts.Utils;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace BML.Scripts {
    public class EmailTaskManager : MonoBehaviour
    {
        [SerializeField] private bool _initialDelay = false;
        
        [Tooltip("Control the number of emails sent over time. The x-axis is a percentage of the _gameTimer duration (Range 0-1).")]
        [SerializeField] private AnimationCurve _numEmails;

        [Tooltip("Percent chance of _numEmails being spam.")]
        [SerializeField] private AnimationCurve _spamWeighting;
        
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

        private void QueueSpamEmailChoices(int min = 0)
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
            // Evaluate how many emails to add and spam weighting
            int addEmailCount = Mathf.FloorToInt(_numEmails.Evaluate(_gameTimer.ElapsedTimeFactor));
            float spamWeighting = _spamWeighting.Evaluate(_gameTimer.ElapsedTimeFactor);
            
            Debug.Log($"EmailTaskManager Add emails ({addEmailCount}) ({spamWeighting} spam)");

            // Fill queues if empty
            if (addEmailCount >= _spamEmailQueue.Count) QueueSpamEmailChoices();
            if (addEmailCount >= _taskEmailQueue.Count) QueueTaskEmailChoices();
            
            // Select random emails
            var emails =
                Enumerable.Range(0, addEmailCount)
                    .Select(i =>
                    {
                        bool isSpam = (Random.value <= spamWeighting);
                        EmailItem emailItem = isSpam
                            ? _spamEmailQueue.SoftDequeue(1)
                            : _taskEmailQueue.SoftDequeue(1);
                        return emailItem;
                    });

            // Add emails
            foreach (var emailItem in emails)
            {
                _inboxState.AddInboxItem(emailItem);
            }

            // Reset timer
            _emailTimer.ResetTimer();
            _emailTimer.StartTimer();
        }
    }
}
