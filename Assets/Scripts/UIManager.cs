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

    public bool menuScreenBool;

    [Header ("InGame UI")]
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
    public Slider avoidTeamsRadiusSlider;
    [SerializeField]
    public Slider avoidTeamsIntensitySlider;

    [SerializeField]
    public Button resetButton;
    [SerializeField]
    public Button ingameQuitButton;

    public GameObject tabToggle;

    private bool optionsMenuToggle = true;

    [Header("Menu UI")]
    [SerializeField]
    public GameObject menuScreen;

    public Toggle oneToggle;
    public Toggle twoToggle;
    public Toggle threeToggle;

    public Slider numberofBoidsSlider;
    public GameObject numberofBoidsText;
    public GameObject invalidText;

    public Button startButton;
    public Button menuQuitButton;

    private void Start()
    {
        Reset();

        menuScreenBool = true;
        menuScreen.SetActive(true);
        optionsMenu.SetActive(false);
        tabToggle.SetActive(false);

        resetButton.onClick.AddListener(Reset);
        menuQuitButton.onClick.AddListener(QuitButton);
        ingameQuitButton.onClick.AddListener(InGameQuitButton);

        numberofBoidsSlider.onValueChanged.AddListener(ChangeText);

        startButton.onClick.AddListener(StartButton);

    }

    private void StartButton()
    {
        if(!oneToggle.isOn && !twoToggle.isOn && !threeToggle.isOn)
        {
            invalidText.SetActive(true);
        }

        else
        {
            menuScreen.SetActive(false);
            menuScreenBool = false;
            tabToggle.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !menuScreenBool)
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

    private void ChangeText(float input)
    {
        numberofBoidsText.GetComponent<Text>().text = input.ToString();

    }

    private void QuitButton()
    {
        Application.Quit();
    }

    private void InGameQuitButton()
    {
        menuScreenBool = true;
        menuScreen.SetActive(true);
        optionsMenu.SetActive(false);
        tabToggle.SetActive(false);

        oneToggle.isOn = false;
        twoToggle.isOn = false;
        threeToggle.isOn = false;

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

        avoidTeamsRadiusSlider.SetValueWithoutNotify(5);
        avoidTeamsIntensitySlider.SetValueWithoutNotify(ReverseIntensitySliders(0.0003f));
    }

    private float ReverseIntensitySliders(float input)
    {
        return Mathf.Sqrt(input / 0.0001f);
    }
}
