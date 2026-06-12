using System;
using Client.Libs.UI.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Clicker.UI
{
    public class ClickerTabScreenView : UIScreenView
    {
        [SerializeField] private Button _collectButton;
        [SerializeField] private TMP_Text _currencyText;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private ParticleSystem _collectParticles;
        [SerializeField] private AudioSource _collectAudioSource;
        [SerializeField] private AudioClip _collectAudioClip;

        public event Action CollectClicked;

        private void Start() =>
            _collectButton.onClick.AddListener(() => CollectClicked?.Invoke());

        internal void SetClickerState(int currency, int energy, int maxEnergy)
        {
            _currencyText.text = $"{currency}";
            _energyText.text = $"{energy}/{maxEnergy}";
        }

        internal void PlayCollectFx()
        {
            if (_collectParticles != null)
                _collectParticles.Play();

            if (_collectAudioSource != null && _collectAudioClip != null)
                _collectAudioSource.PlayOneShot(_collectAudioClip, 0.35f);
        }
    }
}