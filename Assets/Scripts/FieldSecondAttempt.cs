using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldSecondAttempt : MonoBehaviour
{

    //====Variables====
    //This class has variables for set-up and all rules surrounding the movement of the boids, many of which are changed by user input


    //All prefabs for boids, obstacles and the Main Menu UI
    [SerializeField]
    public GameObject GreenPrefab;
    public GameObject OrangePrefab;
    public GameObject PurplePrefab;
    public GameObject ObstaclePrefab;
    public GameObject MenuBackground;

    //The width and height of the space the boids occupy
    private float _width = 26.625f;
    private float _height = 16;

    //The number of boids to be spawned, changed by user input on the Main Menu
    private int _spawnBoids;

    //These two lists are searched every frame, one containing all boids and the other all obstacles
    //They are both cleared at the start of a new game
    public static List<BoidController> Boids;
    public static List<Obstacle> Obstacles;

    //User chooses the number of different boid teams present in the game
    [SerializeField]
    public int NumTeams;

    //Toggles for use of rules
    //The first three are available in-game and wiggle in Unity Editor
    [SerializeField]
    [Header("Options")]
    private bool _useFlock = true;
    [SerializeField]
    private bool _useAlignment = true;
    [SerializeField]
    private bool _useSeparation = true;
    [SerializeField]
    private bool _useWiggle = true;

    //The next six blocks contain the radius and weighting for each Rule
    //These sit here and not in BoidRules.cs so they can be changed easily by the player.
    //Values are then fed as arguments into the BoidRules methods.
    [SerializeField]
    [Header("Separation")]
    private float _separationRadius = 5f;
    [SerializeField]
    private float _separationWeighting = 0.05f;

    [SerializeField]
    [Header("Alignment")]
    private float _alignmentRadius = 10f;
    [SerializeField]
    private float _alignmentWeighting;

    [SerializeField]
    [Header("Flock")]
    private float _flockRadius = 10f;
    [SerializeField]
    private float _flockWeighting;

    [SerializeField]
    [Header("Avoiding other Teams")]
    private float _avoidTeamRadius = 5;
    [SerializeField]
    private float _avoidTeamWeighting = 000.3f;

    //_pad declares the distance away from the wall that the rule begins to implement
    //_turn is the severity of the turning angle
    [SerializeField]
    [Header("Bouncing")]
    private float _pad;
    [SerializeField]
    private float _turn;
    [SerializeField]
    private float _bounceWeighting;
    [SerializeField]
    private float _avoidObstacleWeighting;

    [Header("Wiggle")]
    [SerializeField]
    private float _maxWiggle = 1f;
    [SerializeField]
    private int _wiggleFrame = 10;
    [SerializeField]
    private float _wiggleRadius = 10;

    [SerializeField]
    [Header("Other")]
    private float _steeringSpeed = 1000f;
    [SerializeField]
    private float _totalSpeedWeighting = 1f;

    //The following are the X&Y coordinates created each turn by the Rules
    private float _sepX;
    private float _sepY;
    private float _aliX;
    private float _aliY;
    private float _flockX;
    private float _flockY;
    private float _avoidTeamX;
    private float _avoidTeamY;
    private float _bounceX;
    private float _bounceY;
    private float _wiggleX;
    private float _wiggleY;
    private float _obstacleX;
    private float _obstacleY;

    private int _currentFrame;

    private UIManager _uiManager;

    //====Methods=====

    private void Start()
    {
        Boids = new List<BoidController>();
        Obstacles = new List<Obstacle>();

        _uiManager = GetComponent<UIManager>();

        MenuBoids();

        ResetButton();

        AddListeners();
        
    }


    private void SpawnTeams(int numTeams, int numBoids)
    {
        switch (numTeams)
        {
            case 1:
                for (int i = 0; i < numBoids; i++)
                {
                    SpawnBoid(GreenPrefab, BoidController.Team.green);
                }
                break;
            case 2:
                for (int i = 0; i < numBoids / numTeams; i++)
                {
                    SpawnBoid(GreenPrefab, BoidController.Team.green);
                    SpawnBoid(OrangePrefab, BoidController.Team.orange);
                }
                break;
            case 3:
                for (int i = 0; i < numBoids / numTeams; i++)
                {
                    SpawnBoid(GreenPrefab, BoidController.Team.green);
                    SpawnBoid(OrangePrefab, BoidController.Team.orange);
                    SpawnBoid(PurplePrefab, BoidController.Team.purple);
                }
                break;
            default:
                for (int i = 0; i < numBoids; i++)
                {
                    SpawnBoid(GreenPrefab.gameObject, BoidController.Team.green);
                }
                break;
        }
    }


    private void FixedUpdate()
    {
        Advance();


    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SpawnObstacle(mousePosition);
        }
    }


    private void SpawnObstacle(Vector3 mousePosition)
    {
        var varObstacle = Instantiate(ObstaclePrefab);
        varObstacle.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        Obstacle obstacle = varObstacle.GetComponent<Obstacle>();
        Obstacles.Add(obstacle);
    }


    private void Advance()
    {

        foreach(BoidController boid in Boids)
        {
            if (_useSeparation)
            {
                (_sepX, _sepY) = BoidRules.Separation(boid, _separationRadius, _separationWeighting);
            }
            else
            {
                _sepX = 0;
                _sepY = 0;
            }

            if(_useAlignment)
            {
                (_aliX, _aliY) = BoidRules.Alignment(boid, _alignmentRadius, _alignmentWeighting);
            }
            else
            {
                _aliX = 0;
                _aliY = 0;
            }

            if(_useFlock)
            {
                (_flockX, _flockY) = BoidRules.Flock(boid, _flockRadius, _flockWeighting);
            }
            else
            {
                _flockX = 0;
                _flockY = 0;
            }

            if(_useWiggle)
            {
                (_wiggleX, _wiggleY) = Wiggle(boid);
            }
            else
            {
                _wiggleX = 0;
                _wiggleY = 0;
            }

            (_obstacleX, _obstacleY) = BoidRules.AvoidObstacles(boid, Obstacles, _avoidObstacleWeighting);

            if(NumTeams > 1)
            {
                (_avoidTeamX, _avoidTeamY) = BoidRules.AvoidTeams(boid, _avoidTeamRadius, _avoidTeamWeighting);
            }
            else
            {
                _avoidTeamX = 0;
                _avoidTeamY = 0;
            }

            (_bounceX, _bounceY) = BounceOffWalls(boid);

            boid.SpeedX += (_sepX + _aliX + _flockX + _bounceX + _avoidTeamX + _obstacleX) / _totalSpeedWeighting + _wiggleX;
            boid.SpeedY += (_sepY + _aliY + _flockY + _bounceY + _avoidTeamY + _obstacleY) / _totalSpeedWeighting + _wiggleY;
        }
    }


    private void SpawnBoid(GameObject prefab, BoidController.Team team)
    {
        var boidInstance = Instantiate(prefab, transform);
        boidInstance.transform.localPosition += new Vector3(Random.Range(-_width, _width), Random.Range(-_height, _height), 0);
        BoidController boidController = boidInstance.GetComponent<BoidController>();
        boidController.SpeedX = Random.Range(-2, 2);
        boidController.SpeedY = Random.Range(-2, 2);
        boidController.SteeringSpeed = _steeringSpeed;
        boidController.team = team;
        Boids.Add(boidController);
    }


    private void MenuBoids()
    {
        if (Boids.Count > 0)
        {
            for (int i = Boids.Count - 1; i >= 0; i--)
            {
                Destroy(Boids[i].gameObject);
            }

            Boids.Clear();
        }

        if (Obstacles.Count > 0)
        {
            for (int i = Obstacles.Count - 1; i >= 0; i--)
            {
                Destroy(Obstacles[i].gameObject);
            }

            Obstacles.Clear();
        }

        MenuBackground.SetActive(true);
        SpawnTeams(2, 20);
    }


    private void NewGame()
    {
        if (Boids.Count > 0)
        {
            for (int i = Boids.Count - 1; i >= 0; i--)
            {
                Destroy(Boids[i].gameObject);
            }

            Boids.Clear();
        }

        if (Obstacles.Count > 0)
        {
            for (int i = Obstacles.Count - 1; i >= 0; i--)
            {
                Destroy(Obstacles[i].gameObject);
            }

            Obstacles.Clear();
        }

        SpawnTeams(NumTeams, _spawnBoids);

        MenuBackground.SetActive(false);

    }


    private (float bounceX, float bounceY) BounceOffWalls(BoidController boid)
    {
        Vector3 bounceDirection = Vector3.zero;

        if (boid.transform.position.x < -_width + _pad)
        {
            bounceDirection.x += _turn;
        }
        if (boid.transform.position.x > _width - _pad)
        {
            bounceDirection.x -= _turn;
        }
        if (boid.transform.position.y > _height - _pad)
        {
            bounceDirection.y -= _turn;
        }
        if (boid.transform.position.y < -_height + _pad)
        {
            bounceDirection.y += _turn;
        }

        bounceDirection = bounceDirection.normalized * _bounceWeighting;

        return (bounceDirection.x, bounceDirection.y);
    }


    private (float wiggleX, float wiggleY) Wiggle(BoidController boid)
    {
        _wiggleX = 0;
        _wiggleY = 0;

        int wiggleCount = 0;

        foreach (BoidController otherBoid in Boids)
        {
            if (otherBoid == boid)
                continue;
            else if (otherBoid.team != boid.team)
                continue;

            var distance = Vector3.Distance(boid.transform.position, otherBoid.transform.position);

            if (distance < _wiggleRadius)
            {
                wiggleCount++;
            }
        }

        if(wiggleCount > 3)
        {
            if (_currentFrame == _wiggleFrame)
            {
                _wiggleX = Random.Range(-_maxWiggle, _maxWiggle);
                _wiggleY = Random.Range(-_maxWiggle, _maxWiggle);
                _currentFrame = 0;
            }
        }

        _currentFrame++;
        return (_wiggleX, _wiggleY);
    }


    private void ResetButton()
    {
        _spawnBoids = 60;

        _useFlock = true;
        _useAlignment = true;
        _useSeparation = true;

        _separationRadius = 1;
        _separationWeighting = 0.1f;

        _alignmentRadius = 5;
        _alignmentWeighting = 0.08f;

        _flockRadius = 5;
        _flockWeighting = 0.0003f;

        _avoidTeamRadius = 5;
        _avoidTeamWeighting = 0.0003f;

        _avoidObstacleWeighting = 0.15f;
    }

    private void AvoidRadius(float input)
    {
        _separationRadius = input;
    }

    private void AvoidIntensity(float input)
    {
        _separationWeighting = IntensitySliders(input);
    }

    private void AlignRadius(float input)
    {
        _alignmentRadius = input;
    }

    private void AlignIntensity(float input)
    {
        _alignmentWeighting = IntensitySliders(input);
    }

    private void FlockRadius(float input)
    {
        _flockRadius = input;
    }

    private void FlockIntensity(float input)
    {
        _flockWeighting = IntensitySliders(input);
    }

    private void AvoidTeamsRadius(float input)
    {
        _avoidTeamRadius = input;
    }

    private void AvoidTeamsIntensity(float input)
    {
        _avoidTeamWeighting = IntensitySliders(input);
    }

    private void AvoidObstaclesIntensity(float input)
    {
        _avoidObstacleWeighting = IntensitySliders(input);
    }

    private void FlockToggle(bool input)
    {
        _useFlock = input;
    }

    private void AvoidToggle(bool input)
    {
        _useSeparation = input;
    }

    private void AlignToggle(bool input)
    {
        _useAlignment = input;
    }

    private float IntensitySliders(float input)
    {
        return 0.0001f * Mathf.Pow(input, 2);
    }

    private void OneToggle(bool input)
    {
        if(_uiManager.OneToggle.isOn)
        {
            NumTeams = 1;
            _uiManager.TwoToggle.isOn =false;
            _uiManager.ThreeToggle.isOn = false;
            Debug.Log($"One Toggle: Number of Teams: {NumTeams}");
        }
        else
        {
  
        }
    }

    private void TwoToggle(bool input)
    {
        if (_uiManager.TwoToggle.isOn)
        {
            NumTeams = 2;
            _uiManager.OneToggle.isOn = false;
            _uiManager.ThreeToggle.isOn = false;
            Debug.Log($"Two Toggle: Number of Teams: {NumTeams}");
        }
        else
        {
  
        }
    }

    private void ThreeToggle(bool input)
    {
        if (_uiManager.ThreeToggle.isOn)
        {
            NumTeams = 3;
            _uiManager.OneToggle.isOn = false;
            _uiManager.TwoToggle.isOn = false;
            Debug.Log($"Three Toggle: Number of Teams: {NumTeams}");
        }
        else
        {
        
        }
    }

    private void NumBoids(float input)
    {
        _spawnBoids = (int)input;
    }

    private void StartButton()
    {
        if (!_uiManager.OneToggle.isOn && !_uiManager.TwoToggle.isOn && !_uiManager.ThreeToggle.isOn)
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
        _uiManager.FlockToggle.onValueChanged.AddListener(FlockToggle);
        _uiManager.AlignToggle.onValueChanged.AddListener(AlignToggle);
        _uiManager.AvoidToggle.onValueChanged.AddListener(AvoidToggle);

        _uiManager.FlockRadiusSlider.onValueChanged.AddListener(FlockRadius);
        _uiManager.FlockIntensitySlider.onValueChanged.AddListener(FlockIntensity);

        _uiManager.AlignRadiusSlider.onValueChanged.AddListener(AlignRadius);
        _uiManager.AlignIntensitySlider.onValueChanged.AddListener(AlignIntensity);

        _uiManager.AvoidRadiusSlider.onValueChanged.AddListener(AvoidRadius);
        _uiManager.AvoidIntensitySlider.onValueChanged.AddListener(AvoidIntensity);

        _uiManager.AvoidTeamsRadiusSlider.onValueChanged.AddListener(AvoidTeamsRadius);
        _uiManager.AvoidTeamsIntensitySlider.onValueChanged.AddListener(AvoidTeamsIntensity);

        _uiManager.AvoidObstaclesIntensitySlider.onValueChanged.AddListener(AvoidObstaclesIntensity);

        _uiManager.ResetButton.onClick.AddListener(ResetButton);

        _uiManager.OneToggle.onValueChanged.AddListener(OneToggle);
        _uiManager.TwoToggle.onValueChanged.AddListener(TwoToggle);
        _uiManager.ThreeToggle.onValueChanged.AddListener(ThreeToggle);

        _uiManager.NumBoidsSlider.onValueChanged.AddListener(NumBoids);

        _uiManager.StartButton.onClick.AddListener(StartButton);
        _uiManager.IngameQuitButton.onClick.AddListener(MenuBoids);

    }

}
