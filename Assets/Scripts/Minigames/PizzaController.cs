using UnityEngine;

namespace BML.Scripts
{
    public class PizzaController : MonoBehaviour
    {
        public void OnDropTopping(Draggable draggable)
        {
            MinigameTaskEvent minigameTaskEvent = draggable.GetComponent<MinigameTaskEvent>();
            if (minigameTaskEvent != null)
            {
                bool successful = minigameTaskEvent.TryRaiseSuccess();
                if (!successful)
                    draggable.Reset();
            }
        }
    }
}