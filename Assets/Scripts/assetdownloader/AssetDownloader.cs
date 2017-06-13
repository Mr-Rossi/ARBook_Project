using System.Collections;

using AssetBundles;
using UnityEngine;


public class AssetDownloader : MonoBehaviour
{
	public delegate void GameObjectLoadedCallback(GameObject go);

	string _assetURI = "127.0.0.1";
	bool _manifestLoaded = false;

	public string AssetURI
	{
		get { return _assetURI; }
		set
		{
			_assetURI = value;
			AssetBundleManager.SetSourceAssetBundleURL(_assetURI);
			_manifestLoaded = false;
			StartCoroutine(Initialize());
		}
	}

	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	IEnumerator Initialize()
	{
		// Initialize AssetBundleManifest which loads the AssetBundleManifest object.
		var request = AssetBundleManager.Initialize();

		if (request != null)
			yield return StartCoroutine(request);

		Debug.Log("<color=green>Finished loading assetbundle manifest.</color>");
		_manifestLoaded = true;
	}

	IEnumerator InstantiateGameObjectAsync(string assetBundleName, string assetName, GameObjectLoadedCallback cb)
	{
		// Wait for manifest to be loaded before
		// fetching any assets.
		while (!_manifestLoaded)
		{
			yield return false;
		}

		// Load asset from assetBundle.
		AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
		if (request == null)
		{
			Debug.Log("<color=red>Failed loading " + assetName + " from AssetBundle " + assetBundleName + "</color>");
			yield break;
		}
		yield return StartCoroutine(request);

		// Get the Asset.
		GameObject prefab = request.GetAsset<GameObject>();

		// Instantiate the Asset, or log an error.
		if (prefab != null)
		{
			//cb(Instantiate(prefab));
			Debug.Log("<color=green>Loaded " + assetName + " from " + assetBundleName + "</color>");
			cb(prefab);
		}
		else
		{
			Debug.Log("<color=red>Failed to GetAsset from request</color>");
		}
	}

	///<summary>Loads a GameObject from the given endpoint. If loading is successful, cb is called with the
	///GameObject prefab as its argument.</summary>
	public void LoadGameObjectAsync(string assetBundleName, string assetName, GameObjectLoadedCallback cb)
	{
		StartCoroutine(InstantiateGameObjectAsync(assetBundleName, assetName, cb));
	}
}