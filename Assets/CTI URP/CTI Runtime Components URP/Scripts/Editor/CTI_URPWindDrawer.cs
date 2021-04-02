using UnityEngine;
using System.Collections;
using UnityEditor;

namespace CTI {

	public class CTI_URPWindDrawer : MaterialPropertyDrawer {

		public override void OnGUI (Rect position, MaterialProperty prop, string label, MaterialEditor editor) {
			
			//	Needed since Unity 2019
			EditorGUIUtility.labelWidth = 0;

			Vector4 vec4value = prop.vectorValue;

			GUILayout.Space(-18);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.BeginVertical();
				vec4value.x = EditorGUILayout.Slider("Primary Strength", vec4value.x, 0.0f, 10.0f);
				vec4value.y = EditorGUILayout.Slider("Scondary Strength", vec4value.y, 0.0f, 10.0f);
				vec4value.z = EditorGUILayout.Slider("Edge Flutter", vec4value.z, 0.0f, 4.0f);
			EditorGUILayout.EndVertical();
			if (EditorGUI.EndChangeCheck ()) {
				prop.vectorValue = vec4value;
			}
		}
	}

}