using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EmailInbox
{
    [CreateAssetMenu(fileName = "Email Item", menuName = "BML/Email Item", order = 0)]
    public class EmailItem : ScriptableObject
    {
        #region Inspector

        public string FromAddress;
        // public string ToAddress;
        public string Subject;
        public string Body;

        public bool IsSpam;
        
        [ShowIf("$IsSpam")]
        [Required] public int SpamPenalty;
        
        [HideIf("$IsSpawm")]
        [Required] public string MinigameSceneName;
        
        #if UNITY_EDITOR
        [HideIf("$IsSpam")]
        [Required] public SceneAsset MinigameScene;
        #endif

        #endregion

        #region Unity lifecycle

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (MinigameScene != null)
                MinigameSceneName = MinigameScene.name;
        }
        #endif

        #endregion
    }
}