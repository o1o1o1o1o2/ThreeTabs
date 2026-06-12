using System;
using System.Collections.Generic;
using Client.Dogs.Models;
using Client.Libs.Pool;
using Client.Libs.UI.Internal;
using TMPro;
using UnityEngine;

namespace Client.Dogs.UI
{
    internal class DogsTabScreenView : UIScreenView
    {
        [Header("Templates")]
        [SerializeField] private DogBreedItemView _itemTemplate;
        [SerializeField] private Transform _itemsContainer;

        [Header("UI")]
        [SerializeField] private TMP_Text _dogsStatusText;

        private SimplePool<DogBreedItemView> _itemsPool;
        private readonly List<DogBreedItemView> _activeItems = new();

        public event Action<DogBreedModel> DogBreedSelected;

        protected override void InitializeView()
        {
            base.InitializeView();
            _itemsPool = new SimplePool<DogBreedItemView>(_itemTemplate, _itemsContainer);
        }

        protected override void OnDestroy()
        {
            _itemsPool?.Clear();
            base.OnDestroy();
        }

        public void SetDogsStatus(string status)
        {
            _dogsStatusText.text = status;
            _dogsStatusText.gameObject.SetActive(!string.IsNullOrEmpty(status));
        }

        public void SetDogBreeds(IReadOnlyList<DogBreedModel> breeds)
        {
            // Return all active items to the pool
            foreach (var item in _activeItems)
            {
                item.Clicked -= OnItemClicked;
                _itemsPool.Return(item);
            }
            _activeItems.Clear();

            // Get new items from the pool and set them up
            foreach (var breed in breeds)
            {
                var newItem = _itemsPool.Get();
                newItem.transform.SetParent(_itemsContainer, false);
                newItem.Setup(breed);
                newItem.Clicked += OnItemClicked;
                _activeItems.Add(newItem);
            }
        }

        private void OnItemClicked(DogBreedModel model)
        {
            DogBreedSelected?.Invoke(model);
        }
    }
}