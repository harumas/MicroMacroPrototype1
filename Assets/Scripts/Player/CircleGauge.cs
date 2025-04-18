using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class CircleGauge : MonoBehaviour
    {
        [SerializeField] private Image circleImage;
        [SerializeField] private Color[] gaugeColors;
        [SerializeField] private Transform playerTransform;

        private void Start()
        {
            circleImage.fillAmount = 0f;
        }

        private void Update()
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(playerTransform.position);
            circleImage.transform.position = screenPoint + new Vector3(40f, -60f, 0f);
        }

        public void SetGauge(float scale)
        {
            circleImage.fillAmount = scale;

            int index = Mathf.Max(0, Mathf.FloorToInt(gaugeColors.Length * scale) - 1);
            circleImage.color = gaugeColors[index];
        }
    }
}