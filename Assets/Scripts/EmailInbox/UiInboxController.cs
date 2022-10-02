using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EmailInbox
{
    public class UiInboxController : MonoBehaviour
    {
        #region Inspector

        [Required, SerializeField] private EmailInboxState _inboxState;
        [Required, SerializeField] private Transform _listContainer;
        // [Required, SerializeField] private GameObject _emailItemPreviewPrefab;

        [ShowInInspector, ReadOnly] private List<UiInboxItemController> _children = new List<UiInboxItemController>();

        #endregion

        #region Unity lifecycle

        private void Awake()
        {
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
            
            // TODO render at the right time
            RenderList();
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

            for (int i = 0; i < _inboxState.InboxItems.Count; i++)
            {
                var child = _children[i];
                child.EmailData = _inboxState.InboxItems[i];
            }
        }
        
        #endregion
    }
}