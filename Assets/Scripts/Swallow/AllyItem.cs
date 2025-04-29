using UnityEngine;

namespace Swallow
{
    public interface IAllyItem
    {
        void Initialize();
        void Use(bool isBig, Vector3 direction);
        void Disable();
    }
}