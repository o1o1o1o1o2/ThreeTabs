using System;
using System.Text;
using Client.Dogs.Models;
using Client.Libs.UI.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Dogs.UI
{
    internal sealed class DogBreedDetailsPopupScreenView : UIScreenView
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Button[] _closeButtons;

        public event Action CloseClicked;
        private readonly StringBuilder _sb = new();

        private void Start()
        {
            foreach (var closeButton in _closeButtons)
            {
                closeButton.onClick.AddListener(() => CloseClicked?.Invoke());
            }
        }

        public void SetDetails(DogBreedDetailsModel details)
        {
            _title.text = details.Name;
            _description.text = BuildDescription(details);
        }

        private string BuildDescription(DogBreedDetailsModel details)
        {
            _sb.Clear();
            _sb.AppendLine(details.Description);
            _sb.AppendLine();
            _sb.AppendLine($"Life Span: {details.LifeMin} - {details.LifeMax} years");
            _sb.AppendLine($"Male Weight: {details.MaleWeightMin} - {details.MaleWeightMax} kg");
            _sb.AppendLine($"Female Weight: {details.FemaleWeightMin} - {details.FemaleWeightMax} kg");
            _sb.AppendLine($"Hypoallergenic: {(details.Hypoallergenic ? "Yes" : "No")}");
            return _sb.ToString();
        }
    }
}