using System;
using BML.ScriptableObjectCore.Scripts.Variables;
using TMPro;
using UnityEngine;

namespace BML.Scripts.UI
{
    public class UiTextIntFormatter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private string _numberFormat = "N0";
        [SerializeField] private string _stringFormat = "{0}";
        [SerializeField] private IntVariable _variable;

        private void Awake()
        {
            UpdateText();
            _variable.Subscribe(UpdateText);
        }

        private void OnDestroy()
        {
            _variable.Unsubscribe(UpdateText);
        }

        protected string GetFormattedValue()
        {
            string numberString = _variable.Value.ToString(_numberFormat);
            return String.Format(_stringFormat, numberString);
        }

        protected void UpdateText()
        {
            _text.text = GetFormattedValue();
        }
    }
}