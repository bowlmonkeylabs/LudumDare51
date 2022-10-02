﻿using System;
using System.Collections.Generic;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.Events;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace EmailInbox
{
    public class UiInboxController : MonoBehaviour
    {
        #region Inspector

        [Required, SerializeField] private EmailInboxState _inboxState;
        [Required, SerializeField] private Transform _listContainer;
        // [Required, SerializeField] private GameObject _emailItemPreviewPrefab;

        [Required, SerializeField] private DynamicGameEvent _onOpenEmail;
        [Required, SerializeField] private DynamicGameEvent _onCloseEmail;

        [ShowInInspector, ReadOnly] private List<UiInboxItemController> _children = new List<UiInboxItemController>();

        #endregion

        #region Unity lifecycle

        private void Awake()
        {
            _inboxState.MonoBehaviourAwake();
            
            // Discover child email preview child objects
            _children.Clear();
            for (int i = 0; i < _listContainer.childCount; i++)
            {
                var child = _listContainer.GetChild(i);
                var item = child.GetComponent<UiInboxItemController>();
                if (item == null) continue;

                item.EmailData = null;
                _children.Add(item);
            }
            
            
            RenderList();
            _inboxState.OnUpdateInboxItems += RenderList;
            _onOpenEmail.Subscribe(OnOpenEmailDynamic);
            _onCloseEmail.Subscribe(RemoveEmailDynamic);
        }

        private void OnDestroy()
        {
            _inboxState.MonoBehaviourOnDestroy();
            
            _inboxState.OnUpdateInboxItems -= RenderList;
            _onOpenEmail.Unsubscribe(OnOpenEmailDynamic);
            _onCloseEmail.Unsubscribe(RemoveEmailDynamic);
        }

        #endregion
        
        #region UI control
        
        private void RenderList()
        {
            bool inboxItemsExceedsAvailableItems = (_inboxState.InboxItems.Count > _children.Count);
            if (inboxItemsExceedsAvailableItems)
            {
                throw new Exception($"Not enough list items to render all emails. Please add more.");
            }

            for (int i = 0; i < _children.Count; i++)
            {
                
                var child = _children[i];
                var emailData = (i < _inboxState.InboxItems.Count)
                    ? _inboxState.InboxItems[i]
                    : null;
                child.EmailData = emailData;
                
                var isCurrentlySelected = (_inboxState.SelectedItem.HasValue &&
                                           _inboxState.SelectedItem.Value.InstanceId ==
                                           child.gameObject.GetInstanceID());
                child.IsSelected = isCurrentlySelected;
            }
        }
        
        #endregion
        
        private void OnOpenEmailDynamic(object previousValue, object currentValue)
        {
            var payload = currentValue as EmailInstancePayload?;
            if (payload == null) return;

            OnOpenEmail(payload.Value);
        }

        private void OnOpenEmail(EmailInstancePayload instancePayload)
        {
            _inboxState.SelectedItem = instancePayload;
        }

        private void RemoveEmailDynamic(object previousValue, object currentValue)
        {
            var payload = currentValue as RemoveEmailInstancePayload?;
            if (payload == null) return;

            RemoveEmail(payload.Value);
        }

        private void RemoveEmail(RemoveEmailInstancePayload emailInstanceData)
        {
            if (_inboxState.SelectedItem.HasValue
                && _inboxState.SelectedItem.Value.InstanceId == emailInstanceData.EmailInstance.InstanceId)
            {
                _inboxState.SelectedItem = null;
            }
            
            var childInstanceIndex =
                _children.FindIndex(child => child.gameObject.GetInstanceID() == emailInstanceData.EmailInstance.InstanceId);
            if (childInstanceIndex < 0)
            {
                Debug.Log($"EmailInboxState RemoveInboxItem {emailInstanceData.EmailInstance.InstanceId} No index found");
                return;
            }
            
            _inboxState.RemoveInboxItem(childInstanceIndex, emailInstanceData.CountAsFinishedItem);
        }
    }
}