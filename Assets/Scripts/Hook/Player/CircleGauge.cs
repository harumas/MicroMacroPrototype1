using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class CircleGauge : MonoBehaviour
    {
        [SerializeField] private Image circleImage;
        [SerializeField] private Image background;
        [SerializeField] private Color[] gaugeColors;
        [SerializeField] private Transform playerTransform;

        private void Start()
        {
            circleImage.fillAmount = 0f;
        }

        private void Update()
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(playerTransform.position);
            transform.position = screenPoint + new Vector3(40f, -60f, 0f);
        }

        public void SetGauge(float scale, float interval)
        {
            float realScale = scale + interval;
            circleImage.fillAmount = realScale;

            int index = Mathf.FloorToInt((gaugeColors.Length - 1) * realScale);
            circleImage.color = gaugeColors[index];
        }

        public void ShowBackground(bool isShow)
        {
            circleImage.enabled = isShow;
            background.enabled = isShow;
        }
    }
}