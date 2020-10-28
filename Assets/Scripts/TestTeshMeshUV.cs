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

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        text.faceDilate = u;
        text.outlineWidth = v;
    }
}
