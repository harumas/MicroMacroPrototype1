using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Player
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField] private float hp;
        [SerializeField] private float maxHp = 100f;
        [SerializeField] private float gaugeSpeed = 3f;
        [SerializeField] private RectTransform hpBar;
        [SerializeField] private Image hpBackground;

        private float defaultScale;

        private void Start()
        {
            hp = maxHp;
            defaultScale = hpBar.localScale.x;
        }

        private void Update()
        {
            Vector3 localScale = hpBar.localScale;
            localScale.x = Mathf.Lerp(localScale.x, (hp / maxHp) * defaultScale, Time.deltaTime * gaugeSpeed);
            hpBar.localScale = localScale;
        }

        public void Damage(float damage)
        {
            hp -= damage;
            hp = Mathf.Clamp(hp, 0, maxHp);
            hpBackground.DOFade(0.3f, 0.2f).SetLoops(2, LoopType.Yoyo);

            if (hp == 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}