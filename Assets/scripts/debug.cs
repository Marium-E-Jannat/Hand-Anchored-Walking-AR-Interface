using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug : MonoBehaviour
{
    // Start is called before the first frame update
    public int number;
    // Update is called once per frame
    void Update()
    {
       Debug.Log("the selected option is "+number); 
    }
}
