using System;
using System.Collections.Generic;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.Events;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EmailInbox
{
    public class UiInboxController : MonoBehaviour
    {
        #region Inspector

        [Required, SerializeField] private EmailInboxState _inboxState;
        [Required, SerializeField] private Transform _listContainer;
        [Required, SerializeField] private GameObject _emailItemPreviewPrefab;

        [Required, SerializeField] private DynamicGameEvent _onOpenEmail;
        [Required, SerializeField] private DynamicGameEvent _onCloseEmail;

        [Required, SerializeField] private EventSystem _eventSystem;

        [ShowInInspector, ReadOnly] private List<UiInboxItemController> _children = new List<UiInboxItemController>();

        #endregion

        #region Unity lifecycle

        private void Awake()
        {
            Debug.Log("UiInboxController Awake");
            _inboxState.MonoBehaviourAwake();
            
            // Discover child email preview child objects
            // _children.Clear();
            // for (int i = 0; i < _listContainer.childCount; i++)
            // {
            //     var child = _listContainer.GetChild(i);
            //     var item = child.GetComponent<UiInboxItemController>();
            //     if (item == null) continue;
            //
            //     item.EmailData = null;
            //     _children.Add(item);
            // }
            // GENERATE CHILDREN AT RUNTIME INSTEAD FOR UNIQUE INSTANCE IDS
            for (int i = 0; i < _listContainer.childCount; i++)
            {
                var child = _listContainer.GetChild(i);
                GameObject.Destroy(child.gameObject);
            }
            _children.Clear();
            for (int i = 0; i < _inboxState.InboxItems.Count; i++)
            {
                var newChild = GameObject.Instantiate(_emailItemPreviewPrefab, _listContainer);
                var newChildController = newChild.GetComponent<UiInboxItemController>();
                newChildController.EmailData = _inboxState.InboxItems[i];
                newChildController.IsSelected = false;
                _children.Add(newChildController);
            }
            
            _inboxState.OnAddInboxItems += OnAddInboxItems;
            _inboxState.OnSelectionChanged += SetSelectedInboxItem;
            _onOpenEmail.Subscribe(OnOpenEmailDynamic);
            _onCloseEmail.Subscribe(RemoveEmailDynamic);
        }

        private void OnDestroy()
        {
            _inboxState.MonoBehaviourOnDestroy();
            
            _inboxState.OnAddInboxItems -= OnAddInboxItems;
            _inboxState.OnSelectionChanged -= SetSelectedInboxItem;
            _onOpenEmail.Unsubscribe(OnOpenEmailDynamic);
            _onCloseEmail.Unsubscribe(RemoveEmailDynamic);
        }

        #endregion
        
        #region UI control
        
        private void OnAddInboxItems()
        {
            int numAdded = _inboxState.InboxItems.Count - _children.Count;
            if (numAdded <= 0) return;

            for (int i = _children.Count; i < _inboxState.InboxItems.Count; i++)
            {
                var newChild = GameObject.Instantiate(_emailItemPreviewPrefab, _listContainer);
                var newChildController = newChild.GetComponent<UiInboxItemController>();
                newChildController.EmailData = _inboxState.InboxItems[i];
                newChildController.IsSelected = false;
                _children.Add(newChildController);
            }
        }
        
        #endregion
        
        private void OnOpenEmailDynamic(object previousValue, object currentValue)
        {
            var payload = currentValue as EmailInstancePayload;
            if (payload == null) return;

            OnOpenEmail(payload);
        }

        private void OnOpenEmail(EmailInstancePayload instancePayload)
        {
            _inboxState.SelectedItem = instancePayload;
            // SetSelectedInboxItem();
        }

        private void RemoveEmailDynamic(object previousValue, object currentValue)
        {
            var payload = currentValue as RemoveEmailInstancePayload;
            if (payload == null) return;

            Debug.Log($"Removing email ({payload.EmailInstance.EmailData.Subject}) {payload.EmailInstance.InstanceId}");
            RemoveEmail(payload);
        }

        private void RemoveEmail(RemoveEmailInstancePayload emailInstanceData)
        {
            var childInstanceIndex =
                _children.FindIndex(child => child.GetInstanceID() == emailInstanceData.EmailInstance.InstanceId);
            if (childInstanceIndex < 0)
            {
                Debug.Log($"EmailInboxState RemoveInboxItem {emailInstanceData.EmailInstance.InstanceId} No index found");
                return;
            }
            
            Debug.Log($"Removing email ({emailInstanceData.EmailInstance.EmailData.Subject}) {emailInstanceData.EmailInstance.InstanceId} | Index {childInstanceIndex} | Inbox count count {_inboxState.InboxItems.Count}");
            
            var child = _children[childInstanceIndex];

            bool isCurrentlySelected = (_inboxState.SelectedItem != null
                                        && _inboxState.SelectedItem.InstanceId == child.GetInstanceID());
            if (isCurrentlySelected)
            {
                _inboxState.SelectedItem = null;
            }
            
            _children.RemoveAt(childInstanceIndex);
            GameObject.Destroy(child.gameObject);
            _inboxState.RemoveInboxItem(childInstanceIndex, emailInstanceData.CountAsFinishedItem);
        }

        private void SetSelectedInboxItem() 
        {
            bool anySelected = false;
            
            for (int i = 0; i < _children.Count; i++)
            {
                
                var child = _children[i];
                
                var isCurrentlySelected = (_inboxState.SelectedItem != null &&
                                           _inboxState.SelectedItem.InstanceId ==
                                           child.GetInstanceID());
                child.IsSelected = isCurrentlySelected;
                if (isCurrentlySelected)
                {
                    anySelected = true;
                    _eventSystem.SetSelectedGameObject(child.gameObject);
                }
                child.UpdateColor();
            }
            
            if (!anySelected)
            {
                _eventSystem.SetSelectedGameObject(null);
            }
            
        }
    }
}