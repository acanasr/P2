using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            PickUpEffect(other.gameObject);
        }
    }

    void PickUpEffect(GameObject pickup)
    {
        bool l_CanDoEffect;
        try
        {
            if(pickup.GetComponent<PickUpClass>().CanDoEffect()) l_CanDoEffect = true;
            else l_CanDoEffect = false;
            
        }
        catch (System.Exception)
        {

            Debug.Log("Tu objeto " + pickup.name + " no hereda de la clase PickUpClass, por lo tanto no tiene un efecto pick up");
            return;
        }

        if (l_CanDoEffect)
        {
            pickup.GetComponent<PickUpClass>().DoEffect();

            pickup.GetComponent<PickUpClass>().DisableObject();
        }
    }
}
