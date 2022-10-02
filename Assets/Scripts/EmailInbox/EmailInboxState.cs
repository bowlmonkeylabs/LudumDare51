using System;
using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EmailInbox
{
    [CreateAssetMenu(fileName = "Email Inbox", menuName = "BML/Email Inbox", order = 0)]
    public class EmailInboxState : ScriptableObject
    {
        #region Inspector

        [InlineEditor] public List<EmailItem> InboxItems;

        public int TotalInboxItems => InboxItems.Count;
        public int InboxTaskItems => InboxItems.Count(e => !e.IsSpam);
        public int InboxSpamItems => InboxItems.Count(e => e.IsSpam);

        #endregion

        #region Unity lifecycle



        #endregion
    }
}