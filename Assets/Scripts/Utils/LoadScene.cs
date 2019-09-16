using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    public int SceneNumber;
    
    // Start is called before the first frame update
    void Start()
    {

        this.GetComponent<Button>().onClick.AddListener(delegate { SceneLoader.sceneLoader.ChangeScene(SceneNumber); });
    }

}
