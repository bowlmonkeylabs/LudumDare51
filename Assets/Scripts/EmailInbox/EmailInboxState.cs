﻿using System;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EmailInbox
{
    [CreateAssetMenu(fileName = "Email Inbox", menuName = "BML/Email Inbox", order = 0)]
    public class EmailInboxState : ScriptableObject
    {
        #region Inspector

        [InlineEditor] public EmailItem[] InboxItems;

        public int TotalInboxItems => InboxItems.Length;
        public int InboxTaskItems => InboxItems.Count(e => !e.IsSpam);
        public int InboxSpamItems => InboxItems.Count(e => e.IsSpam);

        #endregion

        #region Unity lifecycle



        #endregion
    }
}