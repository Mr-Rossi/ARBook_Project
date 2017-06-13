using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRayCaster : MonoBehaviour
{

    private Camera c;
    public Image panel;
    public float TimerMax = 5f;
    private float currentTimer = 0f;
    private Transform currentlyLookingAt;

	// Use this for initialization
	void Start ()
	{
	    c = GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = c.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
	    RaycastHit hit;
	    if (Physics.Raycast(ray, out hit))
	    {
	        if (hit.transform == currentlyLookingAt)
	        {
	            if (currentTimer >= TimerMax)    //we already disabled this one, we're done
	                return;
                currentTimer += Time.deltaTime;
	            panel.fillAmount = currentTimer/TimerMax;
	            if (currentTimer >= TimerMax)
	            {
                    /*Browser b = hit.transform.GetComponent<Browser>();
                    if (b != null)
                        Destroy(b);*/
                    //hit.transform.gameObject.SetActive(false);
                }
	        }
	        else
	        {
	            currentlyLookingAt = hit.transform;
	            currentTimer = Time.deltaTime;
                panel.fillAmount = currentTimer / TimerMax;
            }
	        Debug.Log("I'm looking at " + hit.transform.name);
	        
	    }
	    else
	    {
	        currentTimer = 0f;
	        currentlyLookingAt = null;
            panel.fillAmount = currentTimer / TimerMax;
        };
    }
}
