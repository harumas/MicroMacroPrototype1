using UnityEngine;

namespace Swallow
{
    public class StoneAlly : MonoBehaviour, IAllyItem
    {
        [SerializeField] private Rigidbody rig;
        [SerializeField] private Collider col;

        public void Initialize()
        {
            col.enabled = false;
        }

        public void Use(bool isBig, Vector3 direction)
        {
            transform.localScale = Vector3.one * (isBig ? 4f : 1f);
            col.enabled = true;
            rig.isKinematic = false;

            if (isBig)
            {
                rig.useGravity = true;
            }
            else
            {
                rig.useGravity = false;
                rig.AddForce(direction * 10f, ForceMode.VelocityChange);
            }
        }

        public void Disable()
        {
            
        }
    }
}