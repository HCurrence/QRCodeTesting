using QRTracking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    public QRCodesManager codesManager;
    public TextMeshPro statusText;

    public void StartScan()
    {
        codesManager.StartQRTracking();
        statusText.text = "Started QRCode Tracking";
    }

    public void StopScan()
    {
        codesManager.StopQRTracking();
        statusText.text = "Stopped QRCode Tracking";
    }
}
