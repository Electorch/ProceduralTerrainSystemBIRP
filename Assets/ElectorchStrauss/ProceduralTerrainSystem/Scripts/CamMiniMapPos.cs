using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMiniMapPos : MonoBehaviour
{
    [SerializeField]private Transform mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(mainCamera.transform.position.x, 100, mainCamera.transform.position.z - 2436);
    }
}
