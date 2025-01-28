using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigButton : MonoBehaviour
{
    public bool On = false;
    public GameObject Door;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(1, 1, Mathf.Lerp(transform.localScale.z, (On ? 0.5f : 1), 10 * Time.deltaTime));
        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color((On ? 0.25f : 0), 0,0));
        Door.transform.position = new Vector3(16.125f,1.25f,Mathf.Lerp(Door.transform.position.z, (On ? -2.0625f : -0.125f), 10 * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "WeightedCube")
        {
            On = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "WeightedCube")
        {
            On = false;
        }
    }
}
