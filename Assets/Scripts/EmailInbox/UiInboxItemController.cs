using System;
using BML.ScriptableObjectCore.Scripts.Events;
using IKVM.Reflection;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace EmailInbox
{
    public class UiInboxItemController : MonoBehaviour
    {
        #region Inspector

        public EmailItem EmailData
        {
            get => _emailData;
            set
            {
                _emailData = value;
                RenderEmailData();
            }
        }
        [SerializeField, ReadOnly] EmailItem _emailData;

        [Required, SerializeField] private DynamicGameEvent _openEmail;
        [Required, SerializeField] private TMP_Text _textFromAddress;
        [Required, SerializeField] private TMP_Text _textSubject;

        #endregion

        #region Unity lifecycle

        #endregion
        
        #region Email data

        private void RenderEmailData()
        {
            var showThisObject = (_emailData != null);
            this.gameObject.SetActive(showThisObject);
            
            if (!showThisObject) return;

            _textFromAddress.text = _emailData.FromAddress;
            _textSubject.text = _emailData.Subject;
        }

        public struct OpenEmailPayload
        {
            public int InstanceId;
            public EmailItem EmailData;
        }
        public void OpenEmail()
        {
            var payload = new OpenEmailPayload
            {
                InstanceId = this.gameObject.GetInstanceID(),
                EmailData = _emailData,
            };
            _openEmail.Raise(payload);
        }
        
        #endregion

    }
}