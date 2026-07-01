using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneSkip : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad = "tutorial Level 0 ALPHA";

    [Header("Timer")]
    [SerializeField] private bool useTimer = true;
    [SerializeField] private float cutsceneLength = 23f;

    private bool isSkipping;

    private void Start()
    {
        if (useTimer)
        {
            Invoke(nameof(LoadNextScene), cutsceneLength);
        }
    }

    private void Update()
    {   // press k to skip
        if (Input.GetKeyDown(KeyCode.K))
        {
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        if (isSkipping)
        {
            return;
        }

        isSkipping = true;
        SceneManager.LoadScene(sceneToLoad);
    }
}
