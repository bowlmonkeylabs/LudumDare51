using BML.ScriptableObjectCore.Scripts.Variables;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using EmailInbox;
using System;
using System.Collections.Generic;

namespace BML.Scripts {
    public class EmailTaskManager : MonoBehaviour
    {
        [SerializeField] private bool _initialDelay = false;
        [SerializeField] private TimerVariable _emailTimer;
        [SerializeField] private TimerVariable _gameTimer;
        [Required, SerializeField] private EmailInboxState _inboxState;
        [SerializeField] private List<EmailItem> _emailItems;

        private List<EmailItem> _spamEmails;
        private List<EmailItem> _taskEmails;

        #region Unity lifecycle

        void Awake() 
        {
            _inboxState.ClearInboxItems();
            
            _spamEmails = _emailItems.Where(emailItem => {
                return emailItem.IsSpam;
            }).ToList();

            _taskEmails = _emailItems.Where(emailItem => {
                return !emailItem.IsSpam;
            }).ToList();
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

        private void OnEmailTimerFinished() 
        {
            int addEmailCount = 1;
            int addSpamCount = UnityEngine.Random.value > 0.5 ? 1 : 0;

            if((_gameTimer.ElapsedTime / _gameTimer.Duration) >= 0.9) {
                addEmailCount++;
            }

            int addTotalEmails = addEmailCount + addSpamCount;

            for(var i = 0; i < addTotalEmails; i++)
            {
                var randSpam = _spamEmails[UnityEngine.Random.Range(0, _spamEmails.Count)];
                var randomTask = _taskEmails[UnityEngine.Random.Range(0, _taskEmails.Count)];
                
                if(addSpamCount > 0 && addEmailCount > 0) 
                {
                    bool chooseSpam = UnityEngine.Random.value > 0.5 ? true : false;
                    if(chooseSpam) {
                        _inboxState.AddInboxItem(randSpam);
                        addSpamCount--;
                    } else {
                        _inboxState.AddInboxItem(randomTask);
                        addEmailCount--;
                    }

                    continue;
                }

                if(addEmailCount > 0)
                {
                    _inboxState.AddInboxItem(randomTask);
                    addEmailCount--;
                    continue;
                }

                _inboxState.AddInboxItem(randSpam);
                addSpamCount--;
            }

            _emailTimer.ResetTimer();
            _emailTimer.StartTimer();
        }
    }
}
