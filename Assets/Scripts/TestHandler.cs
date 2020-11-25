using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestHandler : MonoBehaviour
{
    public GameObject m_FullTextMesh;
    public GameObject m_FullWordMesh;
    public Button m_ShowFullTextMeshBtn;
    public Button m_FullTextMeshBtn;
    public Button m_ShowFullWordMeshBtn;
    public Button m_FullWordMeshBtn;

    private void Awake()
    {
        m_FullTextMeshBtn.onClick.AddListener(RefreshFullTextMesh);
        m_ShowFullTextMeshBtn.onClick.AddListener(ShowFullTextMesh);
        m_FullWordMeshBtn.onClick.AddListener(RefreshFullWordMesh);
        m_ShowFullWordMeshBtn.onClick.AddListener(ShowFullWordMesh);
    }

    public void ShowFullTextMesh()
    {
        if (m_FullTextMesh.activeSelf == false)
        {
            m_FullTextMesh.SetActive(true);
        }
        if (m_FullWordMesh.activeSelf == true)
        {
            m_FullWordMesh.SetActive(false);
        }
    }

    public void RefreshFullTextMesh()
    {
        
        Refresh(m_FullTextMesh);
    }

    public void ShowFullWordMesh()
    {
        if (m_FullTextMesh.activeSelf == true)
        {
            m_FullTextMesh.SetActive(false);

        }
        if (m_FullWordMesh.activeSelf == false)
        {
            m_FullWordMesh.SetActive(true);
        }
    }

    public void RefreshFullWordMesh()
    {
        Refresh(m_FullWordMesh);
    }

    private void Refresh(GameObject root)
    {
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
            tests[i].text.underlayOffsetX = 0.3f;
            tests[i].text.underlayOffsetY = 0.3f;
            tests[i].text.underlayDilate = u;
            tests[i].text.effectColorFloat = new Vector4(u, v, u, 1f);
        }
    }
}
