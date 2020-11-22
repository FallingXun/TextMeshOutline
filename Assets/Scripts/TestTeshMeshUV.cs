using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestTeshMeshUV : MonoBehaviour
{
    public TextMeshProUGUI text;

    public float faceDilate;
    public float outlineWidth;
    public Color32 effectColor = Color.black;

    public float underlayOffsetX = 0f;
    public float underlayOffsetY = 0f;
    public float underlayDilate = 0f;

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        text.faceDilate = faceDilate;
        text.outlineWidth = outlineWidth;
        text.underlayOffsetX = underlayOffsetX;
        text.underlayOffsetY = underlayOffsetY;
        text.underlayDilate = underlayDilate;

        text.effectColorFloat = new Vector4(effectColor.r / 255f, effectColor.g / 255f, effectColor.b / 255f, effectColor.a / 255f);
    }

    [ContextMenu("ShowRadio")]
    public void ShowRadio()
    {
        Debug.Log(text.scaleRatioA);
    }

    private void OnValidate()
    {
        Refresh();
    }
}
