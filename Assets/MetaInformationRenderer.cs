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

    public void loadMetaData(PageMetaData pageData)
    {
        StartCoroutine(initializePanel(topPanel, pageData.TopElement));
        StartCoroutine(initializePanel(bottomPanel, pageData.BottomElement));
        StartCoroutine(initializePanel(sidePanel, pageData.SideElement));
    }

    IEnumerator initializePanel(GameObject panel, ElementMetaData elementData)
    {

        if(elementData.Type == ElementType.NONE)
        {
            panel.SetActive(false);
            yield break;
        }
        panel.SetActive(true);
        panel.GetComponent<Renderer>().enabled = true;
        panel.GetComponentInChildren<TextMesh>().text = "";
        WWW www=null;
        if (elementData.Type != ElementType.VIDEO && elementData.Type != ElementType.ASSET)
        {
            www = new WWW(elementData.Url);
            yield return www;
        }
        switch (elementData.Type)
        {
            case ElementType.IMAGE:
                panel.GetComponent<Renderer>().material.mainTexture = www.texture;
                break;
            case ElementType.AUDIO:
                panel.GetComponentInChildren<TextMesh>().text = elementData.Url;
                panel.GetComponent<AudioSource>().clip = www.GetAudioClip();
                panel.GetComponent<AudioSource>().Play();
                break;
            case ElementType.VIDEO: //TODO find out why audio is not working
                VideoPlayer vp = panel.GetComponent<VideoPlayer>();
                AudioSource audios = panel.GetComponent<AudioSource>();
                vp.url = elementData.Url;
                vp.EnableAudioTrack(0, true);
                vp.SetTargetAudioSource(0, audios);
                vp.Prepare();
                vp.Play();
                audios.Play();
                //panel.GetComponent<Renderer>().material.mainTexture = www.GetMovieTexture();
                //panel.GetComponent<AudioSource>().clip = www.GetMovieTexture().audioClip;
                break;
            case ElementType.TEXT:
                panel.GetComponentInChildren<TextMesh>().text = www.text;
                break;
            case ElementType.ASSET:
                panel.GetComponent<MeshRenderer>().enabled = false;
                FindObjectOfType<ARMediator>().LoadTrackedObject("redfox", elementData.Url, panel.transform);
                break;
        }
    }
}
