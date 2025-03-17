using System.Collections.Generic;
using Items.ItemVariations;
using UnityEngine;

namespace Items.Pools
{
    public class ItemProjectilePool : MonoBehaviour
    {
        private Queue<ItemProjectile> _objectPool = new();
        private ItemProjectile _prefab;

        public void Initialize(ItemProjectile prefab, int initialSize)
        {
            _prefab = prefab;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewPoolObject();
            }
        }

        private ItemProjectile CreateNewPoolObject()
        {
            ItemProjectile newObject = Instantiate(_prefab);
            newObject.transform.SetParent(transform);
            newObject.gameObject.SetActive(false);
            _objectPool.Enqueue(newObject);
            return newObject;
        }

        public ItemProjectile GetFromPool(Vector3 position, Quaternion rotation)
        {
            ItemProjectile objectToUse;

            if (_objectPool.Count == 0)
            {
                objectToUse = CreateNewPoolObject();
            }
            else
            {
                objectToUse = _objectPool.Dequeue();
            }

            objectToUse.transform.position = position;
            objectToUse.transform.rotation = rotation;
            objectToUse.gameObject.SetActive(true);

            return objectToUse;
        }

        public void ReturnToPool(ItemProjectile objectToReturn)
        {
            objectToReturn.gameObject.SetActive(false);
            _objectPool.Enqueue(objectToReturn);
        }

        private void OnDestroy()
        {
            foreach (ItemProjectile obj in _objectPool)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }

            _objectPool.Clear();
        }
    }
}