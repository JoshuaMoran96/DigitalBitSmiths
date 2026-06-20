using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class changescenesOnTimer : MonoBehaviour
{

    // should aid with loading game scenes after cutscene.

    public float changeTime;
    public string sceneName;
    private void Update()
    {
        changeTime -= Time.deltaTime;
        if(changeTime <= 0)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
       

}
