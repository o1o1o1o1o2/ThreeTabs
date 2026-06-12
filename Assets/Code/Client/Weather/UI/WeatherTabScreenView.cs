using Client.Libs.UI.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Weather.UI
{
    internal class WeatherTabScreenView : UIScreenView
    {
        [SerializeField] private TMP_Text _weatherStatusText;
        [SerializeField] private Image _weatherIcon;

        public void SetWeatherStatus(string status) =>
            _weatherStatusText.text = status;

        public void SetWeatherIcon(Texture2D texture)
        {
            if (_weatherIcon == null || texture == null)
                return;

            _weatherIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            _weatherIcon.color = Color.white;
            _weatherIcon.preserveAspect = true;
        }
    }
}