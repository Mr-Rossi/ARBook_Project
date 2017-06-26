using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MetaInformationRenderer : MonoBehaviour
{

    public GameObject topPanel;
    public GameObject bottomPanel;
    public GameObject sidePanel;
    private PageMetaData pageData;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void loadMetaData(PageMetaData pageData, bool isLeft = true)
    {
        if(isLeft)
            sidePanel.transform.localPosition = new Vector3(12,0,0);
        else
            sidePanel.transform.localPosition = new Vector3(-12, 0, 0);
        StartCoroutine(initializePanel(topPanel, pageData.TopElement));
        StartCoroutine(initializePanel(bottomPanel, pageData.BottomElement));
        StartCoroutine(initializePanel(sidePanel, pageData.SideElement));
    }
    IEnumerator initializePanel(GameObject panel, ElementMetaData elementData)
    {
        Debug.Log("initializing panel?");
        if (elementData.Type == ElementType.NONE)
        {
            panel.SetActive(false);
            yield break;
        }
        panel.SetActive(true);
        ElementInformationRenderer EIRenderer = panel.GetComponent<ElementInformationRenderer>();
        yield return EIRenderer.initializePanel(elementData);

    }

}
