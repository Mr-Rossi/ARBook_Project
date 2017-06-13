using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Vuforia;

public class ARMediator : MonoBehaviour
	{
		AssetDownloader _assetDownloader;


		void Start()
		{
        

			_assetDownloader = FindObjectOfType<AssetDownloader>();
            SetAssetServer("https://mr-rossi.github.io/ARBook/AssetBundles/");
            //LoadTrackedObject("redfox", "RedFox", null);
		}

		public void SetAssetServer(string uri)
		{
			_assetDownloader.AssetURI = uri;
		}

		public void LoadTrackedObject(string assetBundle, string assetName, Transform parent)
		{
			// "artest", "female_bust_OBJ"
			_assetDownloader.LoadGameObjectAsync(assetBundle, assetName,
				(go) =>
				{
                    Debug.Log("Gameobject loaded:"+go.name);
                    GameObject instance = Instantiate(go);
                    instance.transform.SetParent(parent);
                    instance.transform.localPosition = Vector3.zero;
                    instance.transform.localScale = Vector3.one;

                    //go.transform.SetParent(parent);
                    //SetTrackableGameObject(go);
                });
		}
	}