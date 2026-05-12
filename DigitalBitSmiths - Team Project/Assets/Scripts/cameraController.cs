using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class cameraController : MonoBehaviour
{
    //[SerializeField] Transform backgroundImage;
    //[SerializeField] Transform foregroundImage;
    [SerializeField] Transform trackCamera;
    [SerializeField] Camera cameraPrefab;

    [SerializeField] Transform backgroundLayer1, backgroundLayer2, backgroundLayer3;
    [SerializeField] GameObject backgroundContainer;

    float bgDirX;
    float bgDirY;

    float offset = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraPrefab = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {


        backgroundParallax();
        spawnBG();
    }

    void spawnBG() {

        bgDirX = backgroundContainer.transform.position.x;
        
        if (bgDirX == 25) { 
            Destroy(backgroundContainer);
        }
    }

    void backgroundParallax() { // might make this its own script

        bgDirX = trackCamera.position.x;
        bgDirY = trackCamera.position.y;

        //backgroundImage.position = new Vector3(bgDirX, bgDirY, 0);
        
        // Logic

        // Background Layer Positions // PRO TIP: the big the number behind backgroundLayer the closer it is to the player in the z
        // This is evident in the sorting layer: click the backgroundLayer > Layer > AddLayer > Sorting Layers
        backgroundLayer1.position = new Vector3(bgDirX * 0.2f, bgDirY * 0.2f, 0);
        backgroundLayer2.position = new Vector3(bgDirX * 0.5f, bgDirY * 0.5f, 0);
        backgroundLayer3.position = new Vector3(bgDirX * 0.8f, bgDirY * 0.8f, 0);


        //backgroundContainer.position = new Vector3(trackCamera.position.x, trackCamera.position.y, 0); // this should keep it all center with the camera (for now)
    }


    // camera settings
    void cameraSettings() { 
    
    }

}
