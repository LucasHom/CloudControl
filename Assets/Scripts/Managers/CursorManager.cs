using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D crosshairActive;
    [SerializeField] Texture2D crosshairInactive;
    [SerializeField] Texture2D defaultCursor;

    [SerializeField] ShopManager shopManager;

    private bool switchedToCrosshair;

    void Start()
    {
        SetCrosshair(crosshairInactive);
    }

    void Update()
    {
        //if (shopManager.shopContent.activeSelf == false)
        if (Time.timeScale != 0)
        {
            if (switchedToCrosshair == false)
            {
                SetCrosshair(crosshairInactive);

            }
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                SetCrosshair(crosshairActive);
            }

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                SetCrosshair(crosshairInactive);
            }
        }
        else
        {
            ResetCursor();
        }

    }

    void SetCrosshair(Texture2D cursor)
    {
        switchedToCrosshair = true;
        Vector2 hotspot = new Vector2(cursor.width / 2, cursor.height / 2);
        Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
    }

    public void ResetCursor()
    {
        switchedToCrosshair = false;
        Vector2 hotspot = Vector2.zero;
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }
}
