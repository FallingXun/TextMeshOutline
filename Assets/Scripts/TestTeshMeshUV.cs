using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestTeshMeshUV : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CanvasRenderer m_canvasRenderer;
    public Canvas canvas;

    public float u;
    public float v;
    public Color32 outlineColor = Color.black;

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        text.faceDilate = u;
        text.outlineWidth = v;
        text.outlineColorFloat = new Vector4(outlineColor.r / 255f, outlineColor.g / 255f, outlineColor.b / 255f, outlineColor.a / 255f);
    }

    [ContextMenu("ShowRadio")]
    public void ShowRadio()
    {
        Debug.Log(text.scaleRatioA);
    }
}
