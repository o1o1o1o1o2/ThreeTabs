using System.Collections.Generic;
using UnityEngine;

namespace Client.Libs.Pool
{
    public class SimplePool<T> where T : MonoBehaviour
    {
        private readonly T _template;
        private readonly Transform _poolContainer;
        private readonly Queue<T> _pool = new();

        public SimplePool(T template, Transform poolContainer, int initialSize = 0)
        {
            _template = template;
            _poolContainer = poolContainer;

            for (var i = 0; i < initialSize; i++)
            {
                var item = CreateItem();
                _pool.Enqueue(item);
            }
        }

        public T Get()
        {
            var item = _pool.Count > 0 ? _pool.Dequeue() : CreateItem();
            item.gameObject.SetActive(true);
            return item;
        }

        public void Return(T item)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(_poolContainer, false);
            _pool.Enqueue(item);
        }

        private T CreateItem()
        {
            var item = Object.Instantiate(_template, _poolContainer, false);
            item.gameObject.SetActive(false);
            return item;
        }

        public void Clear()
        {
            foreach (var item in _pool)
            {
                Object.Destroy(item.gameObject);
            }

            _pool.Clear();
        }
    }
}