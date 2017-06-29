/*===============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Vuforia;


/// <summary>
/// A custom handler which uses the vuMarkManager.
/// </summary>
public class VuMarkHandler : MonoBehaviour
{
    private List<Texture> mTextures;
    public string ebookPath;
    public string pageName;
    public string metaName;
    //public string ebookMetaDataPath;
    public int mTexOffset=0;
    public List<VuMarkAbstractBehaviour> updatedBehaviours;
    public MetaInformationRenderer metaRenderer;

    #region PRIVATE_MEMBER_VARIABLES

    private PanelShowHide mIdPanel;
    private VuMarkManager mVuMarkManager;
    private VuMarkTarget mClosestVuMark;
    private VuMarkTarget mCurrentVuMark;

    private EbookMetaData ebookMetaData;
    #endregion // PRIVATE_MEMBER_VARIABLES


    #region UNTIY_MONOBEHAVIOUR_METHODS

    void Start()
    {
        mIdPanel = GetComponent<PanelShowHide>();

        // register callbacks to VuMark Manager
        mVuMarkManager = TrackerManager.Instance.GetStateManager().GetVuMarkManager();
        mVuMarkManager.RegisterVuMarkDetectedCallback(OnVuMarkDetected);
        mVuMarkManager.RegisterVuMarkLostCallback(OnVuMarkLost);

        mTextures = new List<Texture>();
        int pageCount = 0;

        while (true)
        {
                pageCount++;
                Texture tex = Resources.Load<Texture>(ebookPath + pageName + pageCount);
                if (tex != null)
                    mTextures.Add(tex);
                else
                {
                    break;
                }
        }

        //Debug.Log("Pages" + mTextures.Count);

        //ebookMetaData = JsonUtility.FromJson<EbookMetaData>(File.ReadAllText(ebookMetaDataPath));
        ebookMetaData = JsonUtility.FromJson<EbookMetaData>(Resources.Load<TextAsset>(ebookPath+metaName).text);

        //Debug.Log(ebookMetaData.Pages[0].TopElement.Url+ ebookMetaData.Pages[0].TopElement.Type);

        /*ElementMetaData element1 = new ElementMetaData
        {
            Type = ElementType.IMAGE,
            Url = "http://www.gaiahealthblog.com/wordpress1/wp-content/uploads/2014/03/pride_prejudice.jpg"
        };
        ElementMetaData element2 = new ElementMetaData
        {
            Type = ElementType.IMAGE,
            Url = "http://therustbeltchronicles.com/wp-content/uploads/2016/02/zz2.jpg"
        };
        ElementMetaData element3 = new ElementMetaData
        {
            Type = ElementType.AUDIO,
            Url = "https://upload.wikimedia.org/wikipedia/commons/a/aa/White_noise.ogg"
        };

        PageMetaData page = new PageMetaData { BottomElement = element1, SideElement = element2, TopElement = element3 };
        EbookMetaData ebook = new EbookMetaData { Pages = new PageMetaData[] { page, page } };

        Debug.Log(JsonUtility.ToJson(ebook));
        File.WriteAllText(ebookMetaDataPath, JsonUtility.ToJson(ebook));*/
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.KeypadPlus)) { 
            mTexOffset += 2;
            updatedBehaviours.Clear();
            //shouldUpdateMetaData = true;
        }
        if (Input.GetKeyUp(KeyCode.KeypadMinus)) { 
            mTexOffset -= 2;
            updatedBehaviours.Clear();
            //shouldUpdateMetaData = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            metaRenderer.loadMetaData(ebookMetaData.Pages[0]);
        }

        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                Debug.Log("pos "+Input.touches[0].position);
                if (Input.touches[0].position.x < 200)
                {
                    mTexOffset -= 2;
                    updatedBehaviours.Clear();
                }
                if (Input.touches[0].position.x > 500)
                {
                    mTexOffset += 2;
                    updatedBehaviours.Clear();
                }
            }
        }
        UpdateClosestTarget();
    }

    void OnDestroy()
    {
        // unregister callbacks from VuMark Manager
        mVuMarkManager.UnregisterVuMarkDetectedCallback(OnVuMarkDetected);
        mVuMarkManager.UnregisterVuMarkLostCallback(OnVuMarkLost);
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS



    #region PUBLIC_METHODS

    /// <summary>
    /// This method will be called whenever a new VuMark is detected
    /// </summary>
    public void OnVuMarkDetected(VuMarkTarget target)
    {
        //Debug.Log("New VuMark: " + GetVuMarkString(target));
        updatedBehaviours.Clear();
    }

    /// <summary>
    /// This method will be called whenever a tracked VuMark is lost
    /// </summary>
    public void OnVuMarkLost(VuMarkTarget target)
    {
        Debug.Log("Lost VuMark: " + GetVuMarkString(target));

        if (target == mCurrentVuMark)
            mIdPanel.ResetShowTrigger();
        updatedBehaviours.Clear();
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS
#if false
    void UpdateClosestTarget()
    {
        Camera cam = DigitalEyewearARController.Instance.PrimaryCamera ?? Camera.main;

        float closestDistance = Mathf.Infinity;
        VuMarkAbstractBehaviour closestVuMarkBehaviour = null;

        foreach (var bhvr in mVuMarkManager.GetActiveBehaviours())
        {
            Vector3 worldPosition = bhvr.transform.position;
            Vector3 camPosition = cam.transform.InverseTransformPoint(worldPosition);

            float distance = Vector3.Distance(Vector2.zero, camPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                mClosestVuMark = bhvr.VuMarkTarget;
                closestVuMarkBehaviour = bhvr;
            }
        }

        if (mClosestVuMark != null &&
            mCurrentVuMark != mClosestVuMark)
        {
            var vuMarkId = GetVuMarkString(mClosestVuMark);
            var vuMarkTitle = GetVuMarkDataType(mClosestVuMark);
            var vuMarkImage = GetVuMarkImage(mClosestVuMark);
            
            mCurrentVuMark = mClosestVuMark;
            mIdPanel.Hide();
            StartCoroutine(ShowPanelAfter(0.5f, vuMarkTitle, vuMarkId, vuMarkImage));
            Debug.Log("VuMarkData: '"+vuMarkId+"'");
            if (vuMarkId.Contains("1")) { 
                closestVuMarkBehaviour.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            }
            else
            {
                closestVuMarkBehaviour.GetComponentInChildren<MeshRenderer>().material.color = Color.black;
            }
        }
    }
#else
    void UpdateClosestTarget()
    {
        Camera cam = DigitalEyewearARController.Instance.PrimaryCamera ?? Camera.main;

        //if (updatedBehaviours.Count<2)
        {
            foreach (var bhvr in mVuMarkManager.GetActiveBehaviours())
            {
                if (!updatedBehaviours.Contains(bhvr))
                {
                    Vector3 worldPosition = bhvr.transform.position;
                    mClosestVuMark = bhvr.VuMarkTarget;

                    var vuMarkId = GetVuMarkString(mClosestVuMark);
                    var vuMarkTitle = GetVuMarkDataType(mClosestVuMark);
                    var vuMarkImage = GetVuMarkImage(mClosestVuMark);

                    //Debug.Log("VuMarkBeh:"+bhvr.GetHashCode());
                    //Debug.Log("VDL: " + vuMarkId.Length);
                    //Debug.Log("VuMarkData: '" + vuMarkId[0] + "'" + vuMarkId[1] + "'" + vuMarkId[2] + "'" + vuMarkId[3] +"'");
                    int vuMarkInt;
                    if (Int32.TryParse(vuMarkId, out vuMarkInt))
                    {
                        //bhvr.GetComponentInChildren<BrowserMaterialComponent>().indexOffset = vuMarkInt + mTexOffset;
                        //StartCoroutine(bhvr.GetComponentInChildren<BrowserMaterialComponent>().LoadContent());
                        //bhvr.GetComponentInChildren<MeshRenderer>().enabled = false;
                        bhvr.GetComponentInChildren<MeshRenderer>().material.mainTexture = mTextures[vuMarkInt + mTexOffset];
                        bhvr.GetComponentInChildren<MetaInformationRenderer>().loadMetaData(ebookMetaData.Pages[vuMarkInt + mTexOffset]/*, (vuMarkInt + mTexOffset)%2==0*/);
                        updatedBehaviours.Add(bhvr);
                    }
                }
            }
        }
        
    }
#endif

    private IEnumerator ShowPanelAfter(float seconds, string vuMarkTitle, string vuMarkId, Sprite vuMarkImage)
    {
        yield return new WaitForSeconds(seconds);

        mIdPanel.Show(vuMarkTitle, vuMarkId, vuMarkImage);
    }

    private string GetVuMarkDataType(VuMarkTarget vumark)
    {
        switch (vumark.InstanceId.DataType)
        {
            case InstanceIdType.BYTES:
                return "Bytes";
            case InstanceIdType.STRING:
                return "String";
            case InstanceIdType.NUMERIC:
                return "Numeric";
        }
        return "";
    }

    private string GetVuMarkString(VuMarkTarget vumark)
    {
        switch (vumark.InstanceId.DataType)
        {
            case InstanceIdType.BYTES:
                return vumark.InstanceId.HexStringValue;
            case InstanceIdType.STRING:
                return vumark.InstanceId.StringValue;
            case InstanceIdType.NUMERIC:
                return vumark.InstanceId.NumericValue.ToString();
        }
        return "";
    }

    private Sprite GetVuMarkImage(VuMarkTarget vumark)
    {
        var instanceImg = vumark.InstanceImage;
        if (instanceImg == null)
        {
            Debug.Log("VuMark Instance Image is null.");
            return null;
        }

        // First we create a texture
        Texture2D texture = new Texture2D(instanceImg.Width, instanceImg.Height, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        instanceImg.CopyToTexture(texture);
        texture.Apply();

        // Then we turn the texture into a Sprite
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }

#endregion // PRIVATE_METHODS
}

