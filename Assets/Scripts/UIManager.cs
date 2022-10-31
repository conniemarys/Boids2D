using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject flockSettings;
    public GameObject avoidSettings;
    public GameObject alignSettings;

    [SerializeField]
    public Slider numBoidsSlider;

    [SerializeField]
    public Toggle flockToggle;
    [SerializeField]
    public Toggle avoidToggle;
    [SerializeField]
    public Toggle alignToggle;

    [SerializeField]
    public Slider flockRadiusSlider;
    [SerializeField]
    public Slider flockIntensitySlider;

    [SerializeField]
    public Slider avoidRadiusSlider;
    [SerializeField]
    public Slider avoidIntensitySlider;

    [SerializeField]
    public Slider alignRadiusSlider;
    [SerializeField]
    public Slider alignIntensitySlider;

    [SerializeField]
    public Button resetButton;
    [SerializeField]
    public Button quitButton;

    private bool optionsMenuToggle = true;

    private void Start()
    {
        Reset();

        resetButton.onClick.AddListener(Reset);
        quitButton.onClick.AddListener(QuitButton);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(optionsMenuToggle)
            {
                optionsMenu.SetActive(false);
                optionsMenuToggle = false;
            }
            else if(!optionsMenuToggle)
            {
                optionsMenu.SetActive(true);
                optionsMenuToggle = true;
            }
        }

    }

    private void QuitButton()
    {
        Application.Quit();
    }

    private void Reset()
    {
        numBoidsSlider.SetValueWithoutNotify(60);

        flockToggle.isOn = true;
        alignToggle.isOn = true;
        avoidToggle.isOn = true;

        avoidRadiusSlider.SetValueWithoutNotify(1);
        avoidIntensitySlider.SetValueWithoutNotify(ReverseIntensitySliders(0.1f));

        alignRadiusSlider.SetValueWithoutNotify(5);
        alignIntensitySlider.SetValueWithoutNotify(ReverseIntensitySliders(0.08f));

        flockRadiusSlider.SetValueWithoutNotify(5);
        flockIntensitySlider.SetValueWithoutNotify(ReverseIntensitySliders(0.0003f));
    }

    private float ReverseIntensitySliders(float input)
    {
        return Mathf.Sqrt(input / 0.0001f);
    }
}
