using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreModule.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Swallow
{
    public class ObjectEater : MonoBehaviour
    {
        [SerializeField] private Transform eatPivot;
        [SerializeField] private Transform itemPivot;
        [SerializeField] private ScalePlayer scalePlayer;
        [SerializeField] private List<GameObject> items;
        [SerializeField] private GameObject[] eatItemPrefabs;
        [SerializeField] private int capacity = 3;
        [SerializeField] private float itemInterval;
        [SerializeField] private float eatRadius;
        [SerializeField] private float eatDuration;
        [SerializeField] private float offset;

        private bool isEating;

        private void Start()
        {
            InputEvent eatEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Eat);
            eatEvent.Started += OnEat;

            InputEvent useEvent = InputActionProvider.CreateEvent(ActionGuid.Player.EaterShoot);
            useEvent.Started += OnShoot;
        }

        private void OnShoot(InputAction.CallbackContext _)
        {
            if (items.Count == 0)
                return;

            bool isBig = scalePlayer.ScaleState == ScaleState.Big;
            items[0].GetComponent<IAllyItem>().Use(isBig, transform.forward);
            items[0].transform.position = transform.position + transform.forward * (transform.localScale.x + 1f);
            items.RemoveAt(0);
        }

        private void Update()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].transform.position = Vector3.Slerp(items[i].transform.position,
                    itemPivot.position + (-transform.forward * (itemInterval * i + offset + transform.localScale.z * 0.5f)),
                    Time.deltaTime * 4f);

                items[i].transform.LookAt(items[i].transform.position + transform.forward);
            }
        }

        private async void OnEat(InputAction.CallbackContext _)
        {
            if (isEating || items.Count >= capacity)
                return;

            eatPivot.transform.localScale = Vector3.one * (eatRadius / transform.localScale.x);
            Collider[] colliders = ArrayPool<Collider>.Shared.Rent(16);
            int count = Physics.OverlapSphereNonAlloc(eatPivot.position, eatRadius * 0.5f, colliders);
            if (count == 0)
            {
                return;
            }

            for (var i = 0; i < count; i++)
            {
                var col = colliders[i];
                if (!col.TryGetComponent(out EatableItem item))
                    continue;

                GameObject obj = Instantiate(eatItemPrefabs[item.ItemId], eatPivot.position, Quaternion.identity);
                obj.GetComponent<IAllyItem>().Initialize();
                items.Add(obj);

                Destroy(col.gameObject);
                break;
            }

            ArrayPool<Collider>.Shared.Return(colliders);

            eatPivot.gameObject.SetActive(true);

            isEating = true;

            await Task.Delay(TimeSpan.FromSeconds(eatDuration), destroyCancellationToken);
            isEating = false;
            eatPivot.gameObject.SetActive(false);
        }
    }
}