using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject startPanel; 
    [SerializeField] private GameObject questionPanel; 
    [SerializeField] private Button startButton; 
    private Recorder recorder;

     public IEnumerator started()
    {
        startPanel.SetActive(true); 
        questionPanel.SetActive(false); 

        // Wait until Recorder.selec is not -1
        yield return new WaitUntil(() => Recorder.selec != -1);

        StartQuiz();
    }

    void Start()
    {
        StartCoroutine(started());
    }

    public void StartQuiz()
    {
        startPanel.SetActive(false);
        questionPanel.SetActive(true);
        return ;
    }
}