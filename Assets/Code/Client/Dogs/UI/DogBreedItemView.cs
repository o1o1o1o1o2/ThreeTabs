using System;
using Client.Dogs.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Dogs.UI
{
    internal class DogBreedItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private Button _button;

        private DogBreedModel _model;

        public event Action<DogBreedModel> Clicked;

        private void OnEnable() =>
            _button.onClick.AddListener(OnClick);

        private void OnDisable() =>
            _button.onClick.RemoveListener(OnClick);

        public void Setup(DogBreedModel model)
        {
            _model = model;
            _nameLabel.text = model.Name;
        }

        private void OnClick() =>
            Clicked?.Invoke(_model);
    }
}