using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ShineShaderMaterialInspector : MaterialEditor {
    private string[] keywordIDs = new string[] {
        "SB_SHINE_REVERSE",
        "SB_SHINE_SMOOTH",
        "SB_SHINE_CUBIC"
    };

    private string[] keywordNames = new string[] {
        "Reverse",
        "Smothstep",
        "Cubic Out"
    };

    Material targetMat;
    List<string> keywords;
    bool isDirty;

    public override void OnInspectorGUI() {
        targetMat = target as Material;

        if (!isVisible || targetMat == null)
            return;

        isDirty = false;
        keywords = new List<string>(targetMat.shaderKeywords);

        EditorGUILayout.LabelField("Shine Wave Easing", EditorStyles.boldLabel);

        for (int i = 0; i < keywordIDs.Length; i++)
            ShowKeywordButton(keywordIDs[i], keywordNames[i]);

        if (isDirty) {
            PropertiesChanged();
            targetMat.shaderKeywords = keywords.ToArray();
            EditorUtility.SetDirty(target);
            isDirty = false;
        }

        base.OnInspectorGUI();
    }

    void ShowKeywordButton(string keyword, string keywordName) {
        bool isOn = IsKeywordOn(keyword);

        EditorGUI.BeginChangeCheck();
        isOn = EditorGUILayout.Toggle(keywordName, isOn);

        if (EditorGUI.EndChangeCheck()) {
            if (isOn)
                keywords.Add(keyword);
            else
                keywords.Remove(keyword);

            isDirty = true;
        }
    }

    bool IsKeywordOn(string keyword) {
        for (int i = 0; i < targetMat.shaderKeywords.Length; i++)
            if (targetMat.shaderKeywords[i] == keyword)
                return true;

        return false;
    }
}