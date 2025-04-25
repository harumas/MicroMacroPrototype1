using Constants;
using Enemy;
using UnityEngine;

namespace Cross
{
    public class Stone : MonoBehaviour
    {
        [SerializeField] private float damage = 20f;
        public bool DoDestroy;

        private void OnCollisionEnter(Collision other)
        {
            if (DoDestroy)
                return;
            
            if (other.gameObject.CompareTag(Tag.Player))
            {
                Status playerStatus = other.gameObject.GetComponent<Status>();
                playerStatus.Damage(damage);
            }

            if (other.gameObject.CompareTag(Tag.Player) ||
                other.gameObject.CompareTag(Tag.Ground))
            {
                Destroy(gameObject);
            }
        }
    }
}