using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public TextMeshProUGUI cashText;

    void Start()
    {
        GridManager.OnCashUpdated.AddListener(UpdateCashText);
    }

    //Update the Cash Amount
    public void UpdateCashText(float amount)
    {
        cashText.text = $"Cash: ${amount:F2}";
    }
}
