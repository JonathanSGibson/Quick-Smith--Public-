using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jack's code
public class PlayerInventory : MonoBehaviour
{
    public Object heldObject;
    public GameObject handPosition;

    public void Clear()
    {
        if (heldObject != null)
        {
            Destroy(heldObject.gameObject);
            heldObject = null;
        }
    }

    private void OnDisable()
    {
        Clear();
    }
}
//End of Jack's code
