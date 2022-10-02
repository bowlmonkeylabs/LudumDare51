using System;
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
            _onCloseEmail.Subscribe(RemoveEmailDynamic);
        }

        private void OnDestroy()
        {
            _inboxState.OnUpdateInboxItems -= RenderList;
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
            }
        }
        
        #endregion

        private void RemoveEmailDynamic(object previousValue, object currentValue)
        {
            var payload = currentValue as UiInboxItemController.OpenEmailPayload?;
            if (payload == null) return;
            
            RemoveEmail(payload.Value);
        }

        private void RemoveEmail(UiInboxItemController.OpenEmailPayload emailData)
        {
            var childInstanceIndex =
                _children.FindIndex(child => child.gameObject.GetInstanceID() == emailData.InstanceId);
            if (childInstanceIndex < 0)
            {
                Debug.Log($"EmailInboxState RemoveInboxItem {emailData.InstanceId} No index found");
                return;
            }
            
            _inboxState.RemoveInboxItem(childInstanceIndex);
        }
    }
}