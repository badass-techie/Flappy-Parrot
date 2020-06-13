using UnityEngine;

public class RotateSkybox : MonoBehaviour{
    public float rotationsPerSecond;   //skybox rotation speed

    // Update is called once per frame
    void Update(){
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationsPerSecond); //skybox rotation
    }
}
