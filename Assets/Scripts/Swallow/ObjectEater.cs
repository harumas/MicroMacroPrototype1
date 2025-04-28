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
        }

        private void Update()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].transform.position = Vector3.Slerp(items[i].transform.position,
                    itemPivot.position + (-transform.forward * (itemInterval * i + offset)),
                    Time.deltaTime * 4f);
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

                items.Add(Instantiate(eatItemPrefabs[item.ItemId], eatPivot.position, Quaternion.identity));
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