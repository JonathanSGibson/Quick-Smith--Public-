using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Jack's code
public class NoShadowLight : MonoBehaviour
{
    public Light noShadowLight;
    public Light mainLight;

    void Update()
    {
        //set lighting for inside of shop to take the suns values
        //needed as shadows shouldnt happen inside of the shop as there would theorectially be a roof
        noShadowLight.intensity = mainLight.intensity;
        noShadowLight.color = mainLight.color;
    }
}
//end of Jack's code