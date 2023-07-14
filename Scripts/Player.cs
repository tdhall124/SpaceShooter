using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _speedBoost = 2f;

    [SerializeField] private GameObject _laserPrefab;

    [SerializeField] private float _fireRate = 0.5f;
    private float _canFire = -1f;

    private Vector3 _laserOffset;
    [SerializeField] private int _lives = 3;

    private SpawnManager _spawnManager;

    [SerializeField] private GameObject _tripleShotPrefab;
 
    private bool _isTripleShotActive = false;
    //private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;

    // shield visualizer
    [SerializeField] private GameObject _shieldVisualizer;

    [SerializeField] private int score = 0;

    private UIManager _uiManager;

    [SerializeField] private GameObject _leftEngineFire;
    [SerializeField] private GameObject _rightEngineFire;
    //private GameObject[] _engineFires;
    //int _randomPick;

    [SerializeField] private AudioSource _audioSourceLaser;
    [SerializeField] private AudioSource _audioSourceExplosion;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null) Debug.LogError("The SpawnManager is NULL.");

        //_shieldVisualizer = this.transform.GetComponentInChildren<Shield>();

        if (_shieldVisualizer == null) Debug.LogError("The Shield Visualizer is NULL.");

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null) Debug.LogError("UI_Manager is NULL.");

        _laserOffset = new Vector3(0, 1.05f, 0);

        if (_leftEngineFire == null)
            Debug.LogError("The Left_Engine_Fire is NULL.");
        if (_rightEngineFire == null)
            Debug.LogError("The Right_Engine_Fire is NULL.");

        //_leftEngineFire = transform.Find("Left_Engine_Fire").gameObject;
        //_rightEngineFire = transform.Find("Right_Engine_Fire").gameObject;
        //_engineFires = new GameObject[] { _leftEngineFire, _rightEngineFire };
        //_randomPick = -1;
        //_audioSourceLaser = GetComponent<AudioSource>();
        //_audioSourceLaser.clip.name
        if (_audioSourceLaser == null)
            Debug.LogError("Player::Start(): Laser Shot audio source is NULL.");
        if (_audioSourceExplosion == null)
            Debug.LogError("Player::Start(): Explosion audio source is NULL.");
       
    }
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            FireLaser();
    }
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        //transform.Translate(direction * (_isSpeedBoostActive ? _speed * _speedBoost : _speed) * Time.deltaTime);
        transform.Translate(direction * _speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4.66f, 0), 0);
        
        // Cannot clamp this because we need to actually reposition to inverse of x
        if (transform.position.x > 9f)
        {
            transform.position = new Vector3(-9f, transform.position.y, 0);
        }
        else if (transform.position.x < -9f)
        {
            transform.position = new Vector3(9f, transform.position.y, 0);
        }
    }
    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
        }
        _audioSourceLaser.Play();
    }
    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }
        
        _lives--;
        // lives is 2 right engine
        // lives is 1 left engine
        //int _randomPick = Random.Range(0, 1);
        if (_lives == 2)
        {
            _rightEngineFire.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngineFire.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _audioSourceExplosion.Play();
            Destroy(this.gameObject);
        }
        
    }
    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isTripleShotActive = false;
    }
    public void SpeedBoostActive()
    {
        //_isSpeedBoostActive = true;
        _speed *= _speedBoost;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        //_isSpeedBoostActive = false;
        _speed /= _speedBoost;
    }
    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
        StartCoroutine(ShieldPowerDownRoutine());
    }
    IEnumerator ShieldPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isShieldActive = false;
    }
    public void UpdateScore(int points)
    {
        score += points;
        _uiManager.UpdateScoreText(score);
    }
}
