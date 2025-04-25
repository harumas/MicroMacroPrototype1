using UnityEngine;

namespace ScaleShooter
{
    public class ShootPointVisualizer : MonoBehaviour
    {
        [SerializeField] private GameObject shootPoint;

        public void Visualize(Vector3 point)
        {
            shootPoint.SetActive(true);
            shootPoint.transform.position = Camera.main.WorldToScreenPoint(point);
        }
        
        public void Hide()
        {
            shootPoint.SetActive(false);
        }
    }
}