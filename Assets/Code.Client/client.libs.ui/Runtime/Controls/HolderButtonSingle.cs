using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Client.Libs.UI.Controls {

	public class HolderButtonSingle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

		private const float DurationToMaxSpeed = 3f;
		private const float StartDeltaTime = 0.4f;
		private const float MaxDeltaTime = 0.1f;
		private const float MinDeltaTime = 0.04f;

		[SerializeField] private float _pulseCooldown;
		private float _collectedTime;
		private bool _isHold;

		private bool _isInteractable = true;
		private float _lastUpdateTime;

		private float _timeFromStart;



		private void Reset() {
			_isHold = false;
			_collectedTime = 0;
			_timeFromStart = 0;
		}

		private void Update() =>
            UpdateState(Time.deltaTime);

        private void OnDisable() =>
            Reset();

        public void DoClick() {
			Hold?.Invoke();
			TryInvokeHoldActionWithCooldown();
		}

		public void OnPointerDown(PointerEventData eventData) {
			if (!_isInteractable) return;

			if (_isHold == false) {
				_isHold = true;
				_collectedTime = _timeFromStart = 0;

				Hold?.Invoke();
				TryInvokeHoldActionWithCooldown();
			}
		}

		public void OnPointerUp(PointerEventData eventData) =>
            Reset();

        public event Action Hold;

		public event Action HoldWithCooldown;

		public void ResetActions() {
			Hold = null;
			HoldWithCooldown = null;
		}

		public void SetInteractable(bool interactable) {
			if (_isInteractable == interactable) return;

			_isInteractable = interactable;

			Reset();
		}

		private void TryInvokeHoldActionWithCooldown() {
			if (Time.realtimeSinceStartup - _lastUpdateTime < _pulseCooldown) return;

			_lastUpdateTime = Time.realtimeSinceStartup;

			HoldWithCooldown?.Invoke();
		}

		private void UpdateState(float dt) {
			if (_isHold == false || !_isInteractable) return;

			var delay = CalcDelta();

			_collectedTime += dt;

			while (_collectedTime >= delay) {
				_collectedTime -= delay;
				_timeFromStart += delay;

				Hold?.Invoke();
				TryInvokeHoldActionWithCooldown();

				delay = CalcDelta();
			}
		}

		private float CalcDelta() =>
			Mathf.Approximately(_timeFromStart, 0)
				? StartDeltaTime
				: DOVirtual.EasedValue(MaxDeltaTime, MinDeltaTime, Mathf.Clamp01(_timeFromStart / DurationToMaxSpeed), Ease.InQuad);

	}

}