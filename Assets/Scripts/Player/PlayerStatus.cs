using DG.Tweening;
using Enemy;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Player
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField] private Status status;
        [SerializeField] private float gaugeSpeed = 3f;
        [SerializeField] private RectTransform hpBar;
        [SerializeField] private Image hpBackground;

        private float defaultScale;
        private Tween currentTween;

        private void Start()
        {
            defaultScale = hpBar.localScale.x;
            status.OnDamage += Damage;
            status.OnDeath += Damage;
        }

        private void Update()
        {
            Vector3 localScale = hpBar.localScale;
            localScale.x = Mathf.Lerp(localScale.x, (status.Hp / status.MaxHp) * defaultScale, Time.deltaTime * gaugeSpeed);
            hpBar.localScale = localScale;

            // デバッグ用リセット
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        public void Damage()
        {
            currentTween?.Kill();
            
            Color color = hpBackground.color;
            color.a = 0f;
            hpBackground.color = color;
            
            currentTween = hpBackground.DOFade(0.3f, 0.2f).SetLoops(2, LoopType.Yoyo);

            if (status.Hp == 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}