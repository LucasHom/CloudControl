using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ReplayButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(EmphasizeObject.Emphasize(gameObject, Vector3.one, 1.1f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f) * 1.05f;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnClick()
    {
        StartCoroutine(ClickEffect());
    }

    private IEnumerator ClickEffect()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        yield return new WaitForSecondsRealtime(0.06f);
        transform.localScale = new Vector3(1f, 1f, 1f) * 1.05f;
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
