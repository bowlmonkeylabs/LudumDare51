using System;
using System.Collections.Generic;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EmailInbox
{
    [CreateAssetMenu(fileName = "Email Inbox", menuName = "BML/Email Inbox", order = 0)]
    public class EmailInboxState : ScriptableObject
    {
        #region Inspector

        [InlineEditor] public List<EmailItem> InboxItems;

        [HideInInspector]
        public EmailInstancePayload SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnSelectionChanged?.Invoke();
            }
        }
        [InlineEditor, SerializeField, ReadOnly] private EmailInstancePayload _selectedItem;

        [SerializeField] private IntVariable _totalInboxItems;
        [SerializeField] private IntVariable _taskInboxItems;
        [SerializeField] private IntVariable _spamInboxItems;
        [SerializeField] private IntVariable _finishedInboxItems;

        public int TotalInboxItems => InboxItems.Count;
        public int TaskInboxItems => InboxItems.Count(e => !e.IsSpam);
        public int SpamInboxItems => InboxItems.Count(e => e.IsSpam);

        #endregion

        #region Events

        public delegate void AddInboxItems();
        public event AddInboxItems OnAddInboxItems;
        
        public delegate void RemoveInboxItems();
        public event RemoveInboxItems OnRemoveInboxItems;
        
        public delegate void SelectionChanged();
        public event SelectionChanged OnSelectionChanged;

        #endregion

        #region Unity lifecycle

        public void MonoBehaviourAwake()
        {
            _finishedInboxItems.Value = 0;
            UpdateCounts();
        }

        public void MonoBehaviourOnDestroy()
        {
            SelectedItem = null;
        }

        #endregion

        #region Public interface

        public void AddInboxItem(EmailItem emailItem)
        {
            InboxItems.Add(emailItem);
            UpdateCounts();
            OnAddInboxItems?.Invoke();
        }

        public void RemoveInboxItem(int index, bool countAsFinishedItem)
        {
            if (countAsFinishedItem  && _finishedInboxItems != null)
            {
                _finishedInboxItems.Value += 1;
            }
            
            InboxItems.RemoveAt(index);
            UpdateCounts();
            OnRemoveInboxItems?.Invoke();
        }

        public void ClearInboxItems()
        {
            Debug.Log($"Clear inbox items");
            InboxItems.Clear();
            SelectedItem = null;
            UpdateCounts();
            OnRemoveInboxItems?.Invoke();
        }

        public void UpdateCounts()
        {
            if (_totalInboxItems != null) _totalInboxItems.Value = TotalInboxItems;
            if (_taskInboxItems != null) _taskInboxItems.Value = TaskInboxItems;
            if (_spamInboxItems != null) _spamInboxItems.Value = SpamInboxItems;
        }

        #endregion
    }
}