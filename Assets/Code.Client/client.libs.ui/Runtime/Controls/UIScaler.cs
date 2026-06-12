using UnityEngine;

namespace Client.Libs.UI.Controls {

	[ExecuteInEditMode]
	public class UIScaler : MonoBehaviour {
		public static float GlobalUIScale = 1.0f;
		
		private void Awake() {
			transform.localScale = Vector3.one * GlobalUIScale;
#if UNITY_EDITOR
			_lastScale = transform.localScale.x;
#endif			
		}
		
#if UNITY_EDITOR
		private float _lastScale;
		
		private void Update() {
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (_lastScale != GlobalUIScale) {
				_lastScale = GlobalUIScale;
				transform.localScale = Vector3.one * GlobalUIScale;
			}
		}
#endif
	}
}
