using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndMenu : MonoBehaviour
{

    public TextMeshProUGUI cashText;
    public TextMeshProUGUI timeText;

    // Start is called before the first frame update
    void Start()
    {
        GridManager.OnCashUpdated.AddListener(FinalCashText);
        GridManager.OnTimeUpdated.AddListener(FinalTimeStamp);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FinalCashText(float amount)
    {

        cashText.text = $"${amount:F2}";
    }

    private void FinalTimeStamp(float amount)
    {
        int minutes = (int)(amount / 60);

        timeText.text = minutes + " minutes and " + $"{amount%60:F2}";
    }
}
