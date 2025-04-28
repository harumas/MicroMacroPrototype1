using UnityEngine;

namespace Swallow
{
    public class EatableItem : MonoBehaviour
    {
        [SerializeField] private int itemId;
        
        public int ItemId => itemId;
    }
}