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
        
        [HideIf("$IsSpam")]
        [Required] public SceneAsset MinigameScene;

        #endregion

        #region Unity lifecycle
        
        

        #endregion
    }
}