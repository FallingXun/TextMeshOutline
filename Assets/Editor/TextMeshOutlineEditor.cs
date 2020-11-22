using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TextMeshOutlineEditor : Editor 
{
    [MenuItem("TextMeshText/Random")]
    public static void RandomOutline()
    {
        var root = Selection.activeGameObject;
        if (root == null)
        {
            return;
        }
        TestTeshMeshUV[] tests = root.GetComponentsInChildren<TestTeshMeshUV>(true);
        for (int i = 0; i < tests.Length; i++)
        {
            float u = Random.Range(0f, 1f);
            float v = Random.Range(0f, 1f);
            tests[i].text.faceDilate = u;
            tests[i].text.outlineWidth = v;
            tests[i].text.effectColorFloat = new Vector4(u, v, u, 1f);
        }
    }
}
