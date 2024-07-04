using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class scenemanager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Key 1 pressed. Loading 'fixed screen' scene.");
                SceneManager.LoadScene("fixed screen");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Key 2 pressed. Loading 'head tracking' scene.");
            SceneManager.LoadScene("head_track_screen");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            
            SceneManager.LoadScene("move with person");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneManager.LoadScene("project on palm");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SceneManager.LoadScene("up from palm");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SceneManager.LoadScene("wrist");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SceneManager.LoadScene("pinch");
        }else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene("scene manager");
        }
        
    }
}
