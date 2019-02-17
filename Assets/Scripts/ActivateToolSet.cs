using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateToolSet : MonoBehaviour {

    public float scaleFactor = 0.4f;
    GameObject cursor;
    
    bool isActive;

    public List<SpriteRenderer> rend;
    
	// Use this for initialization
	void Start () {
        cursor = GameObject.Find("DefaultCursor");
        transform.localScale *= scaleFactor;
	}

    void ActivateThis()
    {
        //Debug.Log("Activate");
        foreach (SpriteRenderer spriteRend in rend) {
            spriteRend.enabled = true;
        }
    }

    void DeactivateThis()
    {
        //Debug.Log("DeActivate");
        foreach (SpriteRenderer spriteRend in rend) {
            spriteRend.enabled = false;
        }
    }

    private void OnEnable()
    {
        NRSRManager.ObjectFocused += ActivateThis;
        NRSRManager.ObjectUnFocused += DeactivateThis;
    }

    private void OnDisable()
    {
        NRSRManager.ObjectFocused -= ActivateThis;
        NRSRManager.ObjectUnFocused -= DeactivateThis;
    }
}
