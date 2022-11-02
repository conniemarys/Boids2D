using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject OptionsMenu;
    public GameObject FlockSettings;
    public GameObject AvoidSettings;
    public GameObject AlignSettings;

    public bool MenuScreenBool;

    [Header ("InGame UI")]
    [SerializeField]
    public Slider NumBoidsSlider;

    [SerializeField]
    public Toggle FlockToggle;
    [SerializeField]
    public Toggle AvoidToggle;
    [SerializeField]
    public Toggle AlignToggle;

    [SerializeField]
    public Slider FlockRadiusSlider;
    [SerializeField]
    public Slider FlockIntensitySlider;

    [SerializeField]
    public Slider AvoidRadiusSlider;
    [SerializeField]
    public Slider AvoidIntensitySlider;

    [SerializeField]
    public Slider AlignRadiusSlider;
    [SerializeField]
    public Slider AlignIntensitySlider;

    [SerializeField]
    public Slider AvoidTeamsRadiusSlider;
    [SerializeField]
    public Slider AvoidTeamsIntensitySlider;

    [SerializeField]
    public Slider AvoidObstaclesIntensitySlider;

    [SerializeField]
    public Button ResetButton;
    [SerializeField]
    public Button IngameQuitButton;

    public GameObject TabToggle;

    private bool OptionsMenuToggle = true;

    [Header("Menu UI")]
    [SerializeField]
    public GameObject MenuScreen;

    public Toggle OneToggle;
    public Toggle TwoToggle;
    public Toggle ThreeToggle;

    public Slider NumberofBoidsSlider;
    public GameObject NumberofBoidsText;
    public GameObject InvalidText;

    public Button StartButton;
    public Button MenuQuitButton;

    private void Start()
    {
        Reset();

        MenuScreenBool = true;
        MenuScreen.SetActive(true);
        OptionsMenu.SetActive(false);
        TabToggle.SetActive(false);

        ResetButton.onClick.AddListener(Reset);
        MenuQuitButton.onClick.AddListener(QuitButton);
        IngameQuitButton.onClick.AddListener(InGameQuitButton);

        NumberofBoidsSlider.onValueChanged.AddListener(ChangeText);

        StartButton.onClick.AddListener(OnStartButton);

    }

    private void OnStartButton()
    {
        if(!OneToggle.isOn && !TwoToggle.isOn && !ThreeToggle.isOn)
        {
            InvalidText.SetActive(true);
        }

        else
        {
            InvalidText.SetActive(false);
            MenuScreen.SetActive(false);
            MenuScreenBool = false;
            TabToggle.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !MenuScreenBool)
        {
            if(OptionsMenuToggle)
            {
                OptionsMenu.SetActive(false);
                OptionsMenuToggle = false;
            }
            else if(!OptionsMenuToggle)
            {
                OptionsMenu.SetActive(true);
                OptionsMenuToggle = true;
            }
        }

    }

    private void ChangeText(float input)
    {
        NumberofBoidsText.GetComponent<Text>().text = input.ToString();

    }

    private void QuitButton()
    {
        Application.Quit();
    }

    private void InGameQuitButton()
    {
        MenuScreenBool = true;
        MenuScreen.SetActive(true);
        OptionsMenu.SetActive(false);
        TabToggle.SetActive(false);

        OneToggle.isOn = false;
        TwoToggle.isOn = false;
        ThreeToggle.isOn = false;

    }

    private void Reset()
    {
        NumBoidsSlider.SetValueWithoutNotify(60);

        FlockToggle.isOn = true;
        AlignToggle.isOn = true;
        AvoidToggle.isOn = true;

        AvoidRadiusSlider.SetValueWithoutNotify(1);
        AvoidIntensitySlider.SetValueWithoutNotify(ReverseIntensitySliders(0.1f));

        AlignRadiusSlider.SetValueWithoutNotify(5);
        AlignIntensitySlider.SetValueWithoutNotify(ReverseIntensitySliders(0.08f));

        FlockRadiusSlider.SetValueWithoutNotify(5);
        FlockIntensitySlider.SetValueWithoutNotify(ReverseIntensitySliders(0.0003f));

        AvoidTeamsRadiusSlider.SetValueWithoutNotify(5);
        AvoidTeamsIntensitySlider.SetValueWithoutNotify(ReverseIntensitySliders(0.0003f));

        AvoidObstaclesIntensitySlider.SetValueWithoutNotify(ReverseIntensitySliders(0.15f));
    }

    private float ReverseIntensitySliders(float input)
    {
        return Mathf.Sqrt(input / 0.0001f);
    }
}
