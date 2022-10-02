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

        [InlineEditor, ReadOnly] public EmailInstancePayload? SelectedItem;

        [SerializeField] private IntVariable _totalInboxItems;
        [SerializeField] private IntVariable _taskInboxItems;
        [SerializeField] private IntVariable _spamInboxItems;

        public int TotalInboxItems => InboxItems.Count;
        public int TaskInboxItems => InboxItems.Count(e => !e.IsSpam);
        public int SpamInboxItems => InboxItems.Count(e => e.IsSpam);

        #endregion

        #region Events

        public delegate void UpdateInboxItems();
        public event UpdateInboxItems OnUpdateInboxItems;

        #endregion

        #region Unity lifecycle

        public void MonoBehaviourAwake()
        {
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
            OnUpdateInboxItems?.Invoke();
        }

        public void RemoveInboxItem(int index)
        {
            InboxItems.RemoveAt(index);
            UpdateCounts();
            OnUpdateInboxItems?.Invoke();
        }

        public void ClearInboxItems()
        {
            InboxItems.Clear();
            SelectedItem = null;
            UpdateCounts();
            OnUpdateInboxItems?.Invoke();
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