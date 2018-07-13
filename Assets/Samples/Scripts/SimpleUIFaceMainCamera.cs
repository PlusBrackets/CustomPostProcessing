using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleUIFaceMainCamera : MonoBehaviour {

    private void Update()
    {
        if (Camera.main != null)
        {
            Vector3 look = 2*transform.position - Camera.main.transform.position;
            transform.LookAt(look,Vector3.up);
        }
    }
}
