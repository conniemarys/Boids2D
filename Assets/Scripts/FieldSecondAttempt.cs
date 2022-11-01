using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldSecondAttempt : MonoBehaviour
{
    [SerializeField]
    public GameObject greenPrefab;
    public GameObject orangePrefab;
    public GameObject purplePrefab;
    public GameObject menuBackground;

    private float width = 26.625f;
    private float height = 16;

    public int spawnBoids;

    public static List<BoidController> boids;

    [SerializeField]
    public int numTeams;

    [SerializeField]
    [Header("Options")]
    private bool useFlock = true;
    [SerializeField]
    private bool useAlignment = true;
    [SerializeField]
    private bool useSeparation = true;
    [SerializeField]
    private bool useWiggle = true;

    [SerializeField]
    [Header("Separation")]
    private float separationRadius = 5f;
    [SerializeField]
    private float separationWeighting = 0.05f;

    [SerializeField]
    [Header("Alignment")]
    private float alignmentRadius = 10f;
    [SerializeField]
    private float alignmentWeighting;

    [SerializeField]
    [Header("Flock")]
    private float flockRadius = 10f;
    [SerializeField]
    private float flockWeighting;

    [SerializeField]
    [Header("Avoiding other Teams")]
    private float avoidTeamRadius;
    [SerializeField]
    private float avoidTeamWeighting;

    [SerializeField]
    [Header("Bouncing")]
    private float pad;
    [SerializeField]
    private float turn;
    [SerializeField]
    private float bounceWeighting;

    [SerializeField]
    [Header("Other")]
    private float steeringSpeed = 1000f;
    [SerializeField]
    private float totalSpeedWeighting = 1f;
    [SerializeField]
    private float maxWiggle = 1f;
    [SerializeField]
    private int wiggleFrame = 10;
    [SerializeField]
    private float wiggleRadius = 10;

    private float sepX;
    private float sepY;
    private float aliX;
    private float aliY;
    private float flockX;
    private float flockY;
    private float avoidTeamX;
    private float avoidTeamY;
    private float bounceX;
    private float bounceY;
    private float wiggleX;
    private float wiggleY;

    private int currentFrame;

    private UIManager uIManager;



    private void Start()
    {
        boids = new List<BoidController>();

        uIManager = GetComponent<UIManager>();

        MenuBoids();

        AddListeners();
        
    }


    private void SpawnTeams(int numTeams, int numBoids)
    {
        switch (numTeams)
        {
            case 1:
                for (int i = 0; i < numBoids; i++)
                {
                    SpawnBoid(greenPrefab, BoidController.Team.green);
                }
                break;
            case 2:
                for (int i = 0; i < numBoids / numTeams; i++)
                {
                    SpawnBoid(greenPrefab, BoidController.Team.green);
                    SpawnBoid(orangePrefab, BoidController.Team.orange);
                }
                break;
            case 3:
                for (int i = 0; i < numBoids / numTeams; i++)
                {
                    SpawnBoid(greenPrefab, BoidController.Team.green);
                    SpawnBoid(orangePrefab, BoidController.Team.orange);
                    SpawnBoid(purplePrefab, BoidController.Team.purple);
                }
                break;
            default:
                for (int i = 0; i < numBoids; i++)
                {
                    SpawnBoid(greenPrefab.gameObject, BoidController.Team.green);
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        Advance();
    }


    private void Advance()
    {

        foreach(BoidController boid in boids)
        {
            if (useSeparation)
            {
                (sepX, sepY) = BoidRules.Separation(boid, separationRadius, separationWeighting);
            }
            else
            {
                sepX = 0;
                sepY = 0;
            }

            if(useAlignment)
            {
                (aliX, aliY) = BoidRules.Alignment(boid, alignmentRadius, alignmentWeighting);
            }
            else
            {
                aliX = 0;
                aliY = 0;
            }

            if(useFlock)
            {
                (flockX, flockY) = BoidRules.Flock(boid, flockRadius, flockWeighting);
            }
            else
            {
                flockX = 0;
                flockY = 0;
            }

            if(useWiggle)
            {
                (wiggleX, wiggleY) = Wiggle(boid);
            }
            else
            {
                wiggleX = 0;
                wiggleY = 0;
            }

            if(numTeams > 1)
            {
                (avoidTeamX, avoidTeamY) = BoidRules.AvoidTeams(boid, avoidTeamRadius, avoidTeamWeighting);
            }
            else
            {
                avoidTeamX = 0;
                avoidTeamY = 0;
            }

            (bounceX, bounceY) = BounceOffWalls(boid);

            boid.SpeedX += (sepX + aliX + flockX + bounceX + avoidTeamX) / totalSpeedWeighting + wiggleX;
            boid.SpeedY += (sepY + aliY + flockY + bounceY + avoidTeamY) / totalSpeedWeighting + wiggleY;
        }
    }



    private void SpawnBoid(GameObject prefab, BoidController.Team team)
    {
        var boidInstance = Instantiate(prefab, transform);
        boidInstance.transform.localPosition += new Vector3(Random.Range(-width, width), Random.Range(-height, height), 0);
        BoidController boidController = boidInstance.GetComponent<BoidController>();
        boidController.SpeedX = Random.Range(-2, 2);
        boidController.SpeedY = Random.Range(-2, 2);
        boidController.SteeringSpeed = steeringSpeed;
        boidController.team = team;
        boids.Add(boidController);
    }

    private void MenuBoids()
    {
        if (boids.Count > 0)
        {
            for (int i = boids.Count - 1; i >= 0; i--)
            {
                Destroy(boids[i].gameObject);
            }

            boids.Clear();
        }

        menuBackground.SetActive(true);
        SpawnTeams(2, 20);
    }


    private void NewGame()
    {
        if (boids.Count > 0)
        {
            for (int i = boids.Count - 1; i >= 0; i--)
            {
                Destroy(boids[i].gameObject);
            }

            boids.Clear();
        }

        SpawnTeams(numTeams, (int)uIManager.numberofBoidsSlider.value);

        menuBackground.SetActive(false);

    }

    private (float bounceX, float bounceY) BounceOffWalls(BoidController boid)
    {
        Vector3 bounceDirection = Vector3.zero;

        if (boid.transform.position.x < -width + pad)
        {
            bounceDirection.x += turn;
        }
        if (boid.transform.position.x > width - pad)
        {
            bounceDirection.x -= turn;
        }
        if (boid.transform.position.y > height - pad)
        {
            bounceDirection.y -= turn;
        }
        if (boid.transform.position.y < -height + pad)
        {
            bounceDirection.y += turn;
        }

        bounceDirection = bounceDirection.normalized * bounceWeighting;

        return (bounceDirection.x, bounceDirection.y);
    }


    private (float wiggleX, float wiggleY) Wiggle(BoidController boid)
    {
        wiggleX = 0;
        wiggleY = 0;

        int wiggleCount = 0;

        foreach (BoidController otherBoid in boids)
        {
            if (otherBoid == boid)
                continue;
            else if (otherBoid.team != boid.team)
                continue;

            var distance = Vector3.Distance(boid.transform.position, otherBoid.transform.position);

            if (distance < wiggleRadius)
            {
                wiggleCount++;
            }
        }

        if(wiggleCount > 3)
        {
            if (currentFrame == wiggleFrame)
            {
                wiggleX = Random.Range(-maxWiggle, maxWiggle);
                wiggleY = Random.Range(-maxWiggle, maxWiggle);
                currentFrame = 0;
            }
        }

        currentFrame++;
        return (wiggleX, wiggleY);
    }


    private void ResetButton()
    {
        spawnBoids = 100;

        useFlock = true;
        useAlignment = true;
        useSeparation = true;

        separationRadius = 1;
        separationWeighting = 0.1f;

        alignmentRadius = 5;
        alignmentWeighting = 0.08f;

        flockRadius = 5;
        flockWeighting = 0.0003f;

        avoidTeamRadius = 5;
        avoidTeamWeighting = 0.0003f;
    }

    private void AvoidRadius(float input)
    {
        separationRadius = input;
    }

    private void AvoidIntensity(float input)
    {
        separationWeighting = IntensitySliders(input);
    }

    private void AlignRadius(float input)
    {
        alignmentRadius = input;
    }

    private void AlignIntensity(float input)
    {
        alignmentWeighting = IntensitySliders(input);
    }

    private void FlockRadius(float input)
    {
        flockRadius = input;
    }

    private void FlockIntensity(float input)
    {
        flockWeighting = IntensitySliders(input);
    }

    private void AvoidTeamsRadius(float input)
    {
        avoidTeamRadius = input;
    }

    private void AvoidTeamsIntensity(float input)
    {
        avoidTeamWeighting = IntensitySliders(input);
    }

    private void FlockToggle(bool input)
    {
        useFlock = input;
    }

    private void AvoidToggle(bool input)
    {
        useSeparation = input;
    }

    private void AlignToggle(bool input)
    {
        useAlignment = input;
    }

    private float IntensitySliders(float input)
    {
        return 0.0001f * Mathf.Pow(input, 2);
    }

    private void OneToggle(bool input)
    {
        if(input)
        {
            numTeams = 1;
            uIManager.twoToggle.isOn =false;
            uIManager.threeToggle.isOn = false;
        }
        else
        {
            numTeams = 0;
        }
    }

    private void TwoToggle(bool input)
    {
        if (input)
        {
            numTeams = 2;
            uIManager.oneToggle.isOn = false;
            uIManager.threeToggle.isOn = false;
        }
        else
        {
            numTeams = 0;
        }
    }

    private void ThreeToggle(bool input)
    {
        if (input)
        {
            numTeams = 3;
            uIManager.oneToggle.isOn = false;
            uIManager.twoToggle.isOn = false;
        }
        else
        {
            numTeams = 0;
        }
    }

    private void NumBoids(float input)
    {
        spawnBoids = (int)input;
    }

    private void StartButton()
    {
        if (!uIManager.oneToggle.isOn && !uIManager.twoToggle.isOn && !uIManager.threeToggle.isOn)
        {
            return;
        }
        else
        {
            NewGame();
        }
    }

    private void AddListeners()
    {
        uIManager.flockToggle.onValueChanged.AddListener(FlockToggle);
        uIManager.alignToggle.onValueChanged.AddListener(AlignToggle);
        uIManager.avoidToggle.onValueChanged.AddListener(AvoidToggle);

        uIManager.flockRadiusSlider.onValueChanged.AddListener(FlockRadius);
        uIManager.flockIntensitySlider.onValueChanged.AddListener(FlockIntensity);

        uIManager.alignRadiusSlider.onValueChanged.AddListener(AlignRadius);
        uIManager.alignIntensitySlider.onValueChanged.AddListener(AlignIntensity);

        uIManager.avoidRadiusSlider.onValueChanged.AddListener(AvoidRadius);
        uIManager.avoidIntensitySlider.onValueChanged.AddListener(AvoidIntensity);

        uIManager.avoidTeamsRadiusSlider.onValueChanged.AddListener(AvoidTeamsRadius);
        uIManager.avoidTeamsIntensitySlider.onValueChanged.AddListener(AvoidTeamsIntensity);

        uIManager.resetButton.onClick.AddListener(ResetButton);

        uIManager.oneToggle.onValueChanged.AddListener(OneToggle);
        uIManager.twoToggle.onValueChanged.AddListener(TwoToggle);
        uIManager.threeToggle.onValueChanged.AddListener(ThreeToggle);

        uIManager.numBoidsSlider.onValueChanged.AddListener(NumBoids);

        uIManager.startButton.onClick.AddListener(StartButton);
        uIManager.ingameQuitButton.onClick.AddListener(MenuBoids);
    }

}
