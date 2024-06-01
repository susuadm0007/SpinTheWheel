using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHandler : MonoBehaviour
{
    public GameObject wheelArrow;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Arrow"))
        {
            AudioManager.instance.Play("Count");
            //StartCoroutine(RotateArrow(wheelArrow));

        }
    }

    IEnumerator RotateArrow(GameObject arrow)
    {
        //arrow.transform.rotation = Quaternion.identity;
        Quaternion startRotation = arrow.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, 60f);
        Debug.Log("Start: "+startRotation+" Target: "+targetRotation);
        float elapsedTime1 = 0f;
        float duration1 = 0.1f;

        while (elapsedTime1 < duration1)
        {
            elapsedTime1 += Time.deltaTime;
            arrow.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime1 / duration1);
            yield return null;
        }
      
        arrow.transform.rotation = targetRotation;

        //yield return new WaitForSeconds(0.5f);


        //arrow.transform.rotation = Quaternion.identity;
        float duration = 0.5f;
        float elapsedTime = 0f;
        startRotation = targetRotation;
        targetRotation = Quaternion.identity;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            arrow.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            yield return null;
        }
        arrow.transform.rotation = targetRotation;
    }
}
