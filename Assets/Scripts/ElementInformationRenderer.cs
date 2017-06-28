using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ElementInformationRenderer : MonoBehaviour, IInputClickHandler
{
    private const string ASSET_BUNDLE_NAME = "redfox";
    public bool activeElement = false;
    public ElementMetaData currentMetaData;

    public void Update()
    {
        if (currentMetaData.Type == ElementType.ASSET)
            GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.LogError("I WAS CLICKED!");
        activeElement = !activeElement;
        if(activeElement)
        {
            switch (currentMetaData.Type)
            {
                case ElementType.IMAGE:
                    break;
                case ElementType.AUDIO:
                    GetComponent<AudioSource>().Play();
                    break;
                case ElementType.VIDEO: //TODO find out why audio is not working
                    VideoPlayer vp = GetComponent<VideoPlayer>();
                    AudioSource audios = GetComponent<AudioSource>();
                    vp.Play();
                    audios.Play();
                    break;
                case ElementType.TEXT:
                    //GetComponentInChildren<TextMesh>().text = www.text;
                    break;
                case ElementType.ASSET:
                    //GetComponent<MeshRenderer>().enabled = false;
                    //FindObjectOfType<ARMediator>().LoadTrackedObject("redfox", elementData.Url, transform);
                    break;
            }
        }
        else
        {
            switch (currentMetaData.Type)
            {
                case ElementType.IMAGE:
                    break;
                case ElementType.AUDIO:
                    GetComponent<AudioSource>().Stop();
                    break;
                case ElementType.VIDEO: //TODO find out why audio is not working
                    VideoPlayer vp = GetComponent<VideoPlayer>();
                    AudioSource audios = GetComponent<AudioSource>();
                    vp.Stop();
                    audios.Stop();
                    break;
                case ElementType.TEXT:
                    //GetComponentInChildren<TextMesh>().text = www.text;
                    break;
                case ElementType.ASSET:
                    //GetComponent<MeshRenderer>().enabled = false;
                    //FindObjectOfType<ARMediator>().LoadTrackedObject("redfox", elementData.Url, transform);
                    break;
            }
        }
    }

    public IEnumerator initializePanel(ElementMetaData elementData)
    {
        cleanPanel();
        currentMetaData = elementData;
        WWW www = null;
        if (elementData.Type != ElementType.VIDEO && elementData.Type != ElementType.ASSET)
        {
            www = new WWW(elementData.Url);
            yield return www;
        }
        switch (elementData.Type)
        {
            case ElementType.IMAGE:
                GetComponent<Renderer>().material.mainTexture = www.texture;
                break;
            case ElementType.AUDIO:
                GetComponentInChildren<TextMesh>().text = elementData.Url;
                GetComponent<AudioSource>().clip = www.GetAudioClip();
                GetComponent<AudioSource>().Play();
                break;
            case ElementType.VIDEO: //TODO find out why audio is not working
                VideoPlayer vp = GetComponent<VideoPlayer>();
                AudioSource audios = GetComponent<AudioSource>();
                vp.url = elementData.Url;
                vp.EnableAudioTrack(0, true);
                vp.SetTargetAudioSource(0, audios);
                vp.Prepare();
                vp.Play();
                audios.Play();
                break;
            case ElementType.TEXT:
                GetComponentInChildren<TextMesh>().text = www.text;
                break;
            case ElementType.ASSET:
                GetComponent<MeshRenderer>().enabled = false;
                FindObjectOfType<ARMediator>().LoadTrackedObject(ASSET_BUNDLE_NAME, elementData.Url, transform);
                break;
        }
    }

    void cleanPanel()
    {
        activeElement = true;
        GetComponent<Renderer>().enabled = true;
        GetComponentInChildren<TextMesh>().text = "";
        GetComponent<Renderer>().material.mainTexture = null;
        GetComponent<AudioSource>().clip = null;
        GetComponent<VideoPlayer>().clip = null;
        if (transform.childCount>1)
            Destroy(transform.GetChild(1).gameObject);
    }
}
