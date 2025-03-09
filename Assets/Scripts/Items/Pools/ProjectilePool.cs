using System.Collections.Generic;
using UnityEngine;

namespace Items.Pools
{
    public class ProjectilePool : MonoBehaviour
    {
        private Queue<GameObject> _objectPool = new Queue<GameObject>();
        private GameObject _prefab;

        public void Initialize(GameObject prefab, int initialSize)
        {
            _prefab = prefab;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewPoolObject();
            }
        }

        private GameObject CreateNewPoolObject()
        {
            GameObject newObject = Instantiate(_prefab);
            newObject.transform.SetParent(transform);
            newObject.SetActive(false);
            _objectPool.Enqueue(newObject);
            return newObject;
        }

        public GameObject GetFromPool(Vector3 position, Quaternion rotation)
        {
            GameObject objectToUse;

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
            objectToUse.SetActive(true);

            return objectToUse;
        }

        public void ReturnToPool(GameObject objectToReturn)
        {
            objectToReturn.SetActive(false);
            _objectPool.Enqueue(objectToReturn);
        }

        private void OnDestroy()
        {
            foreach (GameObject obj in _objectPool)
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