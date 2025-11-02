using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpenPipeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public bool occupied = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateNextPipePosition()
    {
        occupied = true;
        GadgetPurchase.waitingForLocation = false;
        GadgetPurchase.locationSelected = true;
        //GadgetPurchase.nextPipeLocation = transform.position;
        GadgetPurchase.nextPipe = gameObject;
        GadgetPurchase.recntlyClickedGButton.FinalPurchase();
        StartCoroutine(ClickEffect());
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        SFXManager.Instance.PlaySFX("select");
        transform.localScale = new Vector3(1f, 1f, 1f) * 1.2f;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
    private IEnumerator ClickEffect()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        yield return new WaitForSecondsRealtime(0.06f);
        transform.localScale = new Vector3(1f, 1f, 1f) * 1.2f;
    }
}
