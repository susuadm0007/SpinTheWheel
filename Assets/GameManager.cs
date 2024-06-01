using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
//using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject wheel;
    public GameObject spinButton;
    public GameObject wheel2;
    public GameObject spinButton2;
    public GameObject percentagePanel;
    public GameObject percentagePanel2;

    public GameObject circle1;
    public GameObject circle2;
    public GameObject circle3;
    public GameObject circle4;
    public GameObject panel1slider1Text;
    public GameObject panel1slider2Text;
    public GameObject panel1slider3Text;
    public GameObject panel1slider4Text;
    public GameObject panel1slider5Text;
    public GameObject panel1slider6Text;
    public GameObject panel2slider1Text;
    public GameObject panel2slider2Text;
    public GameObject panel2slider3Text;
    public GameObject panel2slider4Text;
    public GameObject panel2slider5Text;
    public GameObject panel2slider6Text;
    public GameObject panel2slider7Text;
    public GameObject panel2slider8Text;

    public GameObject confetti;
    public GameObject confetti2;
    public GameObject confetti3;
   // public GameObject confetti3;

    public float spinSpeed = 1.0f;
    public float spinTime = 7.0f;
    private int itemCount = 6;

    public UnityEngine.UI.Slider[] panel1Sliders;
    public UnityEngine.UI.Slider[] panel2Sliders;

    public int sliderNumber = -1;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.Play("Bg");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        Instance = this;
    }
    public void updateHandler1(float value)
    {
        if(value == 0)
        {
            wheel.SetActive(false);
            spinButton.SetActive(false);
            wheel2.SetActive(true);
            spinButton2.SetActive(true);
            itemCount = 6;
        }
        else
        {
            wheel.SetActive(true);
            spinButton.SetActive(true);
            wheel2.SetActive(false);
            spinButton2.SetActive(false);
            itemCount = 8;
        }
    }
    public void updateHandler2(float value)
    {
        if (value == 1)
        {
            circle1.SetActive(false);
            circle2.SetActive(false);
            spinSpeed = 1.0f;
        }
        if (value == 2)
        {
            circle1.SetActive(true);
            circle2.SetActive(false);
            spinSpeed = 2.0f;
        }
        if (value == 3)
        {
            circle1.SetActive(true);
            circle2.SetActive(true);
            spinSpeed = 3.0f;
        }
    }
    public void updateHandler3(float value)
    {
        if (value == 1)
        {
            circle3.SetActive(false);
            circle4.SetActive(false);
            spinTime = 7.0f;
        }
        if (value == 2)
        {
            circle3.SetActive(true);
            circle4.SetActive(false);
            spinTime = 8.0f;
        }
        if (value == 3)
        {
            circle3.SetActive(true);
            circle4.SetActive(true);
            spinTime = 9.0f;
        }
    }

    void sliderController(float value, int sliderNo)
    {
        if(itemCount == 6)
        {
            float redistributedValue = value - panel1Sliders[sliderNo].value;
            float redistributedValuePerSlider = redistributedValue / (panel1Sliders.Length - 1);
            float totalValue = 0f;
            for (int i = 0; i < panel1Sliders.Length; i++)
            {
                if (i != sliderNo)
                {
                    panel1Sliders[i].value += redistributedValuePerSlider;
                }
                totalValue += panel1Sliders[i].value;
            }
            Debug.Log("Total Value: " + totalValue);
            float excess = totalValue - 100;
            if (excess != 0)
            {
                float adjustmentPerSlider = excess / (panel1Sliders.Length - 1);
                for (int i = 0; i < panel1Sliders.Length; i++)
                {
                    if (i != sliderNo) // Skip the changed slider
                    {
                        panel1Sliders[i].value -= adjustmentPerSlider;
                    }
                }
            }
        }
        else
        {
            float redistributedValue = value - panel2Sliders[sliderNo].value;
            float redistributedValuePerSlider = redistributedValue / (panel2Sliders.Length - 1);
            float totalValue = 0f;
            for (int i = 0; i < panel2Sliders.Length; i++)
            {
                if (i != sliderNo)
                {
                    panel2Sliders[i].value += redistributedValuePerSlider;
                }
                totalValue += panel2Sliders[i].value;
            }
            Debug.Log("Total Value: " + totalValue);
            float excess = totalValue - 100;
            if (excess != 0)
            {
                float adjustmentPerSlider = excess / (panel2Sliders.Length - 1);
                for (int i = 0; i < panel2Sliders.Length; i++)
                {
                    if (i != sliderNo) // Skip the changed slider
                    {
                        panel2Sliders[i].value -= adjustmentPerSlider;
                    }
                }
            }
        }

    }
    public void percentageUpdateHandler1(float value)
    {
        if(itemCount == 6)
        {
            if (value > 3 && value < 97)
            {
                panel1slider1Text.SetActive(true);
            }
            else
            {
                panel1slider1Text.SetActive(false);
            }
            panel1slider1Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";
            /*Debug.Log("Value: "+value);*/
            WheelManager.instance.wheelItems[0].dropChance = value;
        }
        else
        {
            if (value > 3 && value < 97)
            {
                panel2slider1Text.SetActive(true);
            }
            else
            {
                panel2slider1Text.SetActive(false);
            }
            panel2slider1Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

            WheelManager.instance.wheelItems[0].dropChance = value;
        }
        if(sliderNumber == 0)
        {
            //sliderController(value, sliderNumber);
        }

    }
    public void percentageUpdateHandler2(float value)
    {
        if (itemCount == 6)
        {
            if (value > 3 && value < 97)
            {
                panel1slider2Text.SetActive(true);
            }
            else
            {
                panel1slider2Text.SetActive(false);
            }
            panel1slider2Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

            WheelManager.instance.wheelItems[1].dropChance = value;
        }
        else
        {
            if (value > 3 && value < 97)
            {
                panel2slider2Text.SetActive(true);
            }
            else
            {
                panel2slider2Text.SetActive(false);
            }
            panel2slider2Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

            WheelManager.instance.wheelItems[1].dropChance = value;
        }
        if (sliderNumber == 1)
        {
            //sliderController(value, sliderNumber);
        }
    }
    public void percentageUpdateHandler3(float value)
    {
        if (itemCount == 6)
        {
            if (value > 3 && value < 97)
            {
                panel1slider3Text.SetActive(true);
            }
            else
            {
                panel1slider3Text.SetActive(false);
            }
            panel1slider3Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

            WheelManager.instance.wheelItems[2].dropChance = value;
        }
        else
        {
            if (value > 3 && value < 97)
            {
                panel2slider3Text.SetActive(true);
            }
            else
            {
                panel2slider3Text.SetActive(false);
            }
            panel2slider3Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

            WheelManager.instance.wheelItems[2].dropChance = value;
        }
        if (sliderNumber == 2)
        {
            //sliderController(value, sliderNumber);
        }
    }
    public void percentageUpdateHandler4(float value)
    {
        if (itemCount == 6)
        {
            if (value > 3 && value < 97)
            {
                panel1slider4Text.SetActive(true);
            }
            else
            {
                panel1slider4Text.SetActive(false);
            }
            panel1slider4Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

            WheelManager.instance.wheelItems[3].dropChance = value;
        }
        else
        {
            if (value > 3 && value < 97)
            {
                panel2slider4Text.SetActive(true);
            }
            else
            {
                panel2slider4Text.SetActive(false);
            }
            panel2slider4Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

            WheelManager.instance.wheelItems[3].dropChance = value;
        }
        if (sliderNumber == 3)
        {
            //sliderController(value, sliderNumber);
        }
    }
    public void percentageUpdateHandler5(float value)
    {
        if (itemCount == 6)
        {
            if (value > 3 && value < 97)
            {
                panel1slider5Text.SetActive(true);
            }
            else
            {
                panel1slider5Text.SetActive(false);
            }
            panel1slider5Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

            WheelManager.instance.wheelItems[4].dropChance = value;
        }
        else
        {
            if (value > 3 && value < 97)
            {
                panel2slider5Text.SetActive(true);
            }
            else
            {
                panel2slider5Text.SetActive(false);
            }
            panel2slider5Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

            WheelManager.instance.wheelItems[4].dropChance = value;
        }
        if (sliderNumber == 4)
        {
            //sliderController(value, sliderNumber);
        }
    }
    public void percentageUpdateHandler6(float value)
    {
        if (itemCount == 6)
        {
            if (value > 3 && value < 97)
            {
                panel1slider6Text.SetActive(true);
            }
            else
            {
                panel1slider6Text.SetActive(false);
            }
            panel1slider6Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

            WheelManager.instance.wheelItems[5].dropChance = value;
        }
        else
        {
            if (value > 3 && value < 97)
            {
                panel2slider6Text.SetActive(true);
            }
            else
            {
                panel2slider6Text.SetActive(false);
            }
            panel2slider6Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

            WheelManager.instance.wheelItems[5].dropChance = value;
        }
        if (sliderNumber == 5)
        {
            //sliderController(value, sliderNumber);
        }
    }
    public void percentageUpdateHandler7(float value)
    {
        if (value > 3 && value < 97)
        {
            panel2slider7Text.SetActive(true);
        }
        else
        {
            panel2slider7Text.SetActive(false);
        }
        panel2slider7Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

        WheelManager.instance.wheelItems[6].dropChance = value;
        if (sliderNumber == 6)
        {
            //sliderController(value, sliderNumber);
        }
    }
    public void percentageUpdateHandler8(float value)
    {
        if (value > 3 && value < 97)
        {
            panel2slider8Text.SetActive(true);
        }
        else
        {
            panel2slider8Text.SetActive(false);
        }
        panel2slider8Text.GetComponent<TextMeshProUGUI>().text = (int)(value+0.5f) + "%";

        WheelManager.instance.wheelItems[7].dropChance = value;
        if (sliderNumber == 7)
        {
            //sliderController(value, sliderNumber);
        }
    }
    public void buttonClick()
    {
        AudioManager.instance.Play("Click");
    }
    public void setSliderNumber(int value)
    {
        sliderNumber = value;
    }
    public void setPercentage()
    {
        if(itemCount == 6)
        {
            percentagePanel.SetActive(true);
        }
        else
        {
            percentagePanel2.SetActive(true);
        }
    }
    public void resetPercentage()
    {
        if (itemCount == 6)
        {
            for (int i = 0; i < panel1Sliders.Length; i++)
            {
                panel1Sliders[i].value = 50;
            }
        }
        else
        {
            for (int i = 0; i < panel2Sliders.Length; i++)
            {
                panel2Sliders[i].value = 50;
            }
        }
    }
}
