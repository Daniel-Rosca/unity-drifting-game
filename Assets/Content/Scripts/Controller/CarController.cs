using System;
using Cinemachine;
using Content.Scripts.Data;
using Content.Scripts.Manager;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

namespace Content.Scripts.Controller
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarController : MonoBehaviour
    {
        [FormerlySerializedAs("IsInputEnabled")]
        [Header("CAR")]
        // [Range(20, 190)] 
        // [SerializeField] private int maxSpeed = 90;
        [SerializeField] private bool isInputEnabled = true;
        [SerializeField] public SerializableCarData carData;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private GameObject carObject;

        [Range(10, 120)] 
        [SerializeField] private int maxReverseSpeed = 45;
        //[Range(1, 10)] 
        //[SerializeField] private int accelerationMultiplier = 2;

        [Space(10)] [Range(10, 45)] [SerializeField]
        private int maxSteeringAngle = 27;

        [Range(0.1f, 1f)] 
        [SerializeField] private float steeringSpeed = 0.5f;

        [Space(10)] [Range(100, 600)] 
        [SerializeField] private int brakeForce = 350;

        [Range(1, 10)] 
        [SerializeField] private int decelerationMultiplier = 2;
        [Range(1, 10)] 
        [SerializeField] private int handbrakeDriftMultiplier = 5;
        [Space(10)] 
        [SerializeField] private Vector3 bodyMassCenter;

        [Space(20)] 
        [Header("WHEELS")] 
        [Space(10)] 
        [SerializeField] private GameObject frontLeftMesh;
        [SerializeField] private WheelCollider frontLeftCollider;
        [Space(10)] 
        [SerializeField] private GameObject frontRightMesh;
        [SerializeField] private WheelCollider frontRightCollider;
        [Space(10)] 
        [SerializeField] private GameObject rearLeftMesh;
        [SerializeField] private WheelCollider rearLeftCollider;
        [Space(10)] 
        [SerializeField] private GameObject rearRightMesh;
        [SerializeField] private WheelCollider rearRightCollider;

        [Space(20)] 
        [Header("EFFECTS")] 
        [Space(10)] 
        
        [SerializeField]
        private bool useEffects;

        [SerializeField] private ParticleSystem rearLeftWheelParticleSystem;
        [SerializeField] private ParticleSystem rearRightWheelParticleSystem;

        [Space(10)] 
        [SerializeField] private TrailRenderer rearLeftWheelTireSkid;
        [SerializeField] private TrailRenderer rearRightWheelTireSkid;

        [Space(20)] 
        [Header("UI")] 
        [Space(10)] 
        [SerializeField] private bool useUI;

        // [SerializeField] private TextMeshProUGUI carSpeedText;

        [Space(20)] 
        [Header("Sounds")] 
        [Space(10)] 
        [SerializeField] private bool useSounds;

        [SerializeField] private AudioSource carEngineSound;
        [SerializeField] private AudioSource tireScreechSound;

        [Space(20)] 
        private ScoreManager _scoreManager;

        #region Private fields

        private Rigidbody _carRigidbody;

        private float _steeringAxis;
        private float _throttleAxis;
        private float _driftingAxis;
        private float _localVelocityZ;
        private float _localVelocityX;
        private float _initialCarEngineSoundPitch;
        private float _carSpeed;
        private float _scorePoints;
        private float _upgradedSpeed;
        private float _upgradedAcceleration;

        private bool _deceleratingCar;
        private bool _touchControlsSetup;
        private bool _isDrifting;
        private bool _isTractionLocked;

        private WheelFrictionCurve _frontLeftWheelFriction;
        private float _frontLeftWheelExtremumSlip;

        private WheelFrictionCurve _frontRightWheelFriction;
        private float _frontTightWheelExtremumSlip;

        private WheelFrictionCurve _rearLeftWheelFriction;
        private float _rearLeftWheelExtremumSlip;

        private WheelFrictionCurve _rearRightWheelFriction;
        private float _rearRightWheelExtremumSlip;

        #endregion

        private PhotonView _photonView;
        
        public float CurrentSpeed { get; private set; }
        public float CurrentAcceleration { get; private set; }
        public bool IsInputEnabled
        {
            get => isInputEnabled;
            set => isInputEnabled = value;
        }

        public SerializableCarData CarData => carData;

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            _scoreManager = FindObjectOfType<ScoreManager>();
            SetCarData(carData);
            CurrentSpeed = carData.baseSpeed;
            CurrentAcceleration = carData.baseAcceleration;
            isInputEnabled = true;

            if (_photonView.IsMine)
            {
                virtualCamera.gameObject.SetActive(true);
            }
            
            _carRigidbody = gameObject.GetComponent<Rigidbody>();
            _carRigidbody.centerOfMass = bodyMassCenter;

            _frontLeftWheelFriction = new WheelFrictionCurve();

            var sidewaysFriction = frontLeftCollider.sidewaysFriction;
            _frontLeftWheelFriction.extremumSlip = sidewaysFriction.extremumSlip;
            _frontLeftWheelExtremumSlip = sidewaysFriction.extremumSlip;
            _frontLeftWheelFriction.extremumValue = sidewaysFriction.extremumValue;
            _frontLeftWheelFriction.asymptoteSlip = sidewaysFriction.asymptoteSlip;
            _frontLeftWheelFriction.asymptoteValue = sidewaysFriction.asymptoteValue;
            _frontLeftWheelFriction.stiffness = sidewaysFriction.stiffness;
            _frontRightWheelFriction = new WheelFrictionCurve();

            var friction = frontRightCollider.sidewaysFriction;
            _frontRightWheelFriction.extremumSlip = friction.extremumSlip;
            _frontTightWheelExtremumSlip = friction.extremumSlip;
            _frontRightWheelFriction.extremumValue = friction.extremumValue;
            _frontRightWheelFriction.asymptoteSlip = friction.asymptoteSlip;
            _frontRightWheelFriction.asymptoteValue = friction.asymptoteValue;
            _frontRightWheelFriction.stiffness = friction.stiffness;

            _rearLeftWheelFriction = new WheelFrictionCurve();
            var sidewaysFriction1 = rearLeftCollider.sidewaysFriction;
            _rearLeftWheelFriction.extremumSlip = sidewaysFriction1.extremumSlip;
            _rearLeftWheelExtremumSlip = sidewaysFriction1.extremumSlip;
            _rearLeftWheelFriction.extremumValue = sidewaysFriction1.extremumValue;
            _rearLeftWheelFriction.asymptoteSlip = sidewaysFriction1.asymptoteSlip;
            _rearLeftWheelFriction.asymptoteValue = sidewaysFriction1.asymptoteValue;
            _rearLeftWheelFriction.stiffness = sidewaysFriction1.stiffness;

            _rearRightWheelFriction = new WheelFrictionCurve();
            var friction1 = rearRightCollider.sidewaysFriction;
            _rearRightWheelFriction.extremumSlip = friction1.extremumSlip;
            _rearRightWheelExtremumSlip = friction1.extremumSlip;
            _rearRightWheelFriction.extremumValue = friction1.extremumValue;
            _rearRightWheelFriction.asymptoteSlip = friction1.asymptoteSlip;
            _rearRightWheelFriction.asymptoteValue = friction1.asymptoteValue;
            _rearRightWheelFriction.stiffness = friction1.stiffness;

            if (carEngineSound != null)
            {
                _initialCarEngineSoundPitch = carEngineSound.pitch;
            }

            if (useUI)
            {
                InvokeRepeating(nameof(ManageUI), 0f, 0.1f);
            }
            // else
            // {
            //     if (carSpeedText != null)
            //     {
            //         carSpeedText.text = "0";
            //     }
            // }

            if (useSounds)
            {
                InvokeRepeating(nameof(CarSounds), 0f, 0.1f);
            }
            else
            {
                if (carEngineSound != null)
                {
                    carEngineSound.Stop();
                }

                if (tireScreechSound != null)
                {
                    tireScreechSound.Stop();
                }
            }

            if (useEffects) return;
            if (rearLeftWheelParticleSystem != null)
            {
                rearLeftWheelParticleSystem.Stop();
            }

            if (rearRightWheelParticleSystem != null)
            {
                rearRightWheelParticleSystem.Stop();
            }

            if (rearLeftWheelTireSkid != null)
            {
                rearLeftWheelTireSkid.emitting = false;
            }

            if (rearRightWheelTireSkid != null)
            {
                rearRightWheelTireSkid.emitting = false;
            }
        }

        private void Update()
        {
            _carSpeed = 2 * PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60 / 1000;

            _localVelocityX = transform.InverseTransformDirection(_carRigidbody.velocity).x;

            _localVelocityZ = transform.InverseTransformDirection(_carRigidbody.velocity).z;

            if (!_photonView.IsMine) return;
            GetInputs();

            HandleBrake();

            if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
            {
                ThrottleOff();
            }

            if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.Space) &&
                !_deceleratingCar)
            {
                InvokeRepeating(nameof(DecelerateCar), 0f, 0.1f);
                _deceleratingCar = true;
            }

            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && _steeringAxis != 0f)
            {
                ResetSteeringAngle();
            }
            
            AnimateWheelMeshes();

        }

        private void HandleBrake()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                CancelInvoke(nameof(DecelerateCar));
                _deceleratingCar = false;
                Handbrake();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                RecoverTraction();
            }
        }

        private void GetInputs()
        {
            if (!isInputEnabled)
            {
                return;
            }
            if (Input.GetKey(KeyCode.W))
            {
                CancelInvoke(nameof(DecelerateCar));
                _deceleratingCar = false;
                GoForward();
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                TurnLeft();
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                CancelInvoke(nameof(DecelerateCar));
                _deceleratingCar = false;
                GoReverse();
            }

            if (Input.GetKey(KeyCode.D))
            {
                TurnRight();
            }
        }

        public void ManageUI()
        {
            if (!useUI) return;
            try
            {
                // var absoluteCarSpeed = Abs(_carSpeed);
                // carSpeedText.text = $"Speed: {RoundToInt(absoluteCarSpeed).ToString()}";
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

            if (_carSpeed <= 0f)
            {
                return;
            }

            if (!_isDrifting) return;
            _scorePoints += Time.deltaTime * 1f;
                
            _scoreManager.AddDriftingScore(_scorePoints);
        }
        public void CarSounds()
        {
            switch (useSounds)
            {
                case true:
                    try
                    {
                        if (carEngineSound != null)
                        {
                            var engineSoundPitch =
                                _initialCarEngineSoundPitch + (Abs(_carRigidbody.velocity.magnitude) / 25f);
                            carEngineSound.pitch = engineSoundPitch;
                        }

                        if (_isDrifting || (_isTractionLocked && Abs(_carSpeed) > 12f))
                        {
                            if (!tireScreechSound.isPlaying)
                            {
                                tireScreechSound.Play();
                            }
                        }
                        else if (!_isDrifting && (!_isTractionLocked || Abs(_carSpeed) < 12f))
                        {
                            tireScreechSound.Stop();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning(ex);
                    }

                    break;
                case false:
                {
                    if (carEngineSound != null && carEngineSound.isPlaying)
                    {
                        carEngineSound.Stop();
                    }

                    if (tireScreechSound != null && tireScreechSound.isPlaying)
                    {
                        tireScreechSound.Stop();
                    }

                    break;
                }
            }
        }

        private void TurnLeft()
        {
            _steeringAxis -= Time.deltaTime * 10f * steeringSpeed;
            if (_steeringAxis < -1f)
            {
                _steeringAxis = -1f;
            }

            var steeringAngle = _steeringAxis * maxSteeringAngle;
            frontLeftCollider.steerAngle = Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
            frontRightCollider.steerAngle = Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
        }
        
        private void TurnRight()
        {
            _steeringAxis += Time.deltaTime * 10f * steeringSpeed;
            if (_steeringAxis > 1f)
            {
                _steeringAxis = 1f;
            }

            var steeringAngle = _steeringAxis * maxSteeringAngle;
            frontLeftCollider.steerAngle = Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
            frontRightCollider.steerAngle = Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
        }

        private void ResetSteeringAngle()
        {
            _steeringAxis = _steeringAxis switch
            {
                < 0f => _steeringAxis + (Time.deltaTime * 10f * steeringSpeed),
                > 0f => _steeringAxis - (Time.deltaTime * 10f * steeringSpeed),
                _ => _steeringAxis
            };

            if (Abs(frontLeftCollider.steerAngle) < 1f)
            {
                _steeringAxis = 0f;
            }

            var steeringAngle = _steeringAxis * maxSteeringAngle;
            frontLeftCollider.steerAngle = Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
            frontRightCollider.steerAngle = Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
        }

        private void AnimateWheelMeshes()
        {
            try
            {
                frontLeftCollider.GetWorldPose(out var flwPosition, out var flwRotation);
                frontLeftMesh.transform.position = flwPosition;
                frontLeftMesh.transform.rotation = flwRotation;

                frontRightCollider.GetWorldPose(out var frwPosition, out var frwRotation);
                frontRightMesh.transform.position = frwPosition;
                frontRightMesh.transform.rotation = frwRotation;

                rearLeftCollider.GetWorldPose(out var rlwPosition, out var rlwRotation);
                rearLeftMesh.transform.position = rlwPosition;
                rearLeftMesh.transform.rotation = rlwRotation;

                rearRightCollider.GetWorldPose(out var rrwPosition, out var rrwRotation);
                rearRightMesh.transform.position = rrwPosition;
                rearRightMesh.transform.rotation = rrwRotation;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        
        private void GoForward()
        {
            if (Abs(_localVelocityX) > 2.5f)
            {
                _isDrifting = true;
                DriftCarPS();
            }
            else
            {
                _isDrifting = false;
                DriftCarPS();
            }

            _throttleAxis += (Time.deltaTime * 3f);
            if (_throttleAxis > 1f)
            {
                _throttleAxis = 1f;
            }

            if (_localVelocityZ < -1f)
            {
                Brakes();
            }
            else
            {
                if (RoundToInt(_carSpeed) < (carData.baseSpeed + carData.upgradedSpeed))
                {
                    frontLeftCollider.brakeTorque = 0;
                    frontLeftCollider.motorTorque = (carData.baseAcceleration + carData.upgradedAcceleration) * 50f * _throttleAxis;
                    frontRightCollider.brakeTorque = 0;
                    frontRightCollider.motorTorque = (carData.baseAcceleration + carData.upgradedAcceleration) * 50f * _throttleAxis;
                    rearLeftCollider.brakeTorque = 0;
                    rearLeftCollider.motorTorque = (carData.baseAcceleration + carData.upgradedAcceleration) * 50f * _throttleAxis;
                    rearRightCollider.brakeTorque = 0;
                    rearRightCollider.motorTorque = (carData.baseAcceleration + carData.upgradedAcceleration) * 50f * _throttleAxis;
                }
                else
                {
                    frontLeftCollider.motorTorque = 0;
                    frontRightCollider.motorTorque = 0;
                    rearLeftCollider.motorTorque = 0;
                    rearRightCollider.motorTorque = 0;
                }
            }
        }
        
        private void GoReverse()
        {
            if (Abs(_localVelocityX) > 2.5f)
            {
                _isDrifting = true;
                DriftCarPS();
            }
            else
            {
                _isDrifting = false;
                DriftCarPS();
            }

            _throttleAxis -= (Time.deltaTime * 3f);
            if (_throttleAxis < -1f)
            {
                _throttleAxis = -1f;
            }

            if (_localVelocityZ > 1f)
            {
                Brakes();
            }
            else
            {
                if (Abs(RoundToInt(_carSpeed)) < maxReverseSpeed)
                {
                    frontLeftCollider.brakeTorque = 0;
                    frontLeftCollider.motorTorque = (carData.baseAcceleration + carData.upgradedAcceleration) * 50f * _throttleAxis;
                    frontRightCollider.brakeTorque = 0;
                    frontRightCollider.motorTorque = (carData.baseAcceleration + carData.upgradedAcceleration) * 50f * _throttleAxis;
                    rearLeftCollider.brakeTorque = 0;
                    rearLeftCollider.motorTorque = (carData.baseAcceleration + carData.upgradedAcceleration) * 50f * _throttleAxis;
                    rearRightCollider.brakeTorque = 0;
                    rearRightCollider.motorTorque = (carData.baseAcceleration + carData.upgradedAcceleration) * 50f * _throttleAxis;
                }
                else
                {
                    frontLeftCollider.motorTorque = 0;
                    frontRightCollider.motorTorque = 0;
                    rearLeftCollider.motorTorque = 0;
                    rearRightCollider.motorTorque = 0;
                }
            }
        }

        private void ThrottleOff()
        {
            frontLeftCollider.motorTorque = 0;
            frontRightCollider.motorTorque = 0;
            rearLeftCollider.motorTorque = 0;
            rearRightCollider.motorTorque = 0;
        }

        public void DecelerateCar()
        {
            if (Abs(_localVelocityX) > 2.5f)
            {
                _isDrifting = true;
                DriftCarPS();
            }
            else
            {
                _isDrifting = false;
                DriftCarPS();
            }

            if (_throttleAxis != 0f)
            {
                switch (_throttleAxis)
                {
                    case > 0f:
                        _throttleAxis -= Time.deltaTime * 10f;
                        break;
                    case < 0f:
                        _throttleAxis += Time.deltaTime * 10f;
                        break;
                }

                if (Abs(_throttleAxis) < 0.15f)
                {
                    _throttleAxis = 0f;
                }
            }

            _carRigidbody.velocity *= 1f / (1f + 0.025f * decelerationMultiplier);
            frontLeftCollider.motorTorque = 0;
            frontRightCollider.motorTorque = 0;
            rearLeftCollider.motorTorque = 0;
            rearRightCollider.motorTorque = 0;
            if (_carRigidbody.velocity.magnitude < 0.25f)
            {
                _carRigidbody.velocity = Vector3.zero;
                CancelInvoke(nameof(DecelerateCar));
            }
        }

        private void Brakes()
        {
            frontLeftCollider.brakeTorque = brakeForce;
            frontRightCollider.brakeTorque = brakeForce;
            rearLeftCollider.brakeTorque = brakeForce;
            rearRightCollider.brakeTorque = brakeForce;
        }
        private void Handbrake()
        {
            CancelInvoke(nameof(RecoverTraction));
            _driftingAxis += (Time.deltaTime);
            var secureStartingPoint = _driftingAxis * _frontLeftWheelExtremumSlip * handbrakeDriftMultiplier;

            if (secureStartingPoint < _frontLeftWheelExtremumSlip)
            {
                _driftingAxis = _frontLeftWheelExtremumSlip / (_frontLeftWheelExtremumSlip * handbrakeDriftMultiplier);
            }

            if (_driftingAxis > 1f)
            {
                _driftingAxis = 1f;
            }

            if (Abs(_localVelocityX) > 2.5f)
            {
                _isDrifting = true;
            }
            else
            {
                _isDrifting = false;
            }

            if (_driftingAxis < 1f)
            {
                _frontLeftWheelFriction.extremumSlip =
                    _frontLeftWheelExtremumSlip * handbrakeDriftMultiplier * _driftingAxis;
                frontLeftCollider.sidewaysFriction = _frontLeftWheelFriction;

                _frontRightWheelFriction.extremumSlip =
                    _frontTightWheelExtremumSlip * handbrakeDriftMultiplier * _driftingAxis;
                frontRightCollider.sidewaysFriction = _frontRightWheelFriction;

                _rearLeftWheelFriction.extremumSlip =
                    _rearLeftWheelExtremumSlip * handbrakeDriftMultiplier * _driftingAxis;
                rearLeftCollider.sidewaysFriction = _rearLeftWheelFriction;

                _rearRightWheelFriction.extremumSlip =
                    _rearRightWheelExtremumSlip * handbrakeDriftMultiplier * _driftingAxis;
                rearRightCollider.sidewaysFriction = _rearRightWheelFriction;
            }

            _isTractionLocked = true;
            DriftCarPS();
        }

        private void DriftCarPS()
        {
            if (useEffects)
            {
                try
                {
                    if (_isDrifting)
                    {
                        rearLeftWheelParticleSystem.Play();
                        rearRightWheelParticleSystem.Play();
                    }
                    else if (!_isDrifting)
                    {
                        rearLeftWheelParticleSystem.Stop();
                        rearRightWheelParticleSystem.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }

                try
                {
                    if ((_isTractionLocked || Abs(_localVelocityX) > 5f) && Abs(_carSpeed) > 12f)
                    {
                        rearLeftWheelTireSkid.emitting = true;
                        rearRightWheelTireSkid.emitting = true;
                    }
                    else
                    {
                        rearLeftWheelTireSkid.emitting = false;
                        rearRightWheelTireSkid.emitting = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
            }
            else if (!useEffects)
            {
                if (rearLeftWheelParticleSystem != null)
                {
                    rearLeftWheelParticleSystem.Stop();
                }

                if (rearRightWheelParticleSystem != null)
                {
                    rearRightWheelParticleSystem.Stop();
                }

                if (rearLeftWheelTireSkid != null)
                {
                    rearLeftWheelTireSkid.emitting = false;
                }

                if (rearRightWheelTireSkid != null)
                {
                    rearRightWheelTireSkid.emitting = false;
                }
            }
        }

        private void RecoverTraction()
        {
            _isTractionLocked = false;
    
            _driftingAxis -= Time.deltaTime / 1.5f;
            _driftingAxis = Max(_driftingAxis, 0f);
    
            var driftSlip = handbrakeDriftMultiplier * _driftingAxis;

            AdjustWheelFriction(frontLeftCollider, _frontLeftWheelFriction, _frontLeftWheelExtremumSlip * driftSlip);
            AdjustWheelFriction(frontRightCollider, _frontRightWheelFriction, _frontTightWheelExtremumSlip * driftSlip);
            AdjustWheelFriction(rearLeftCollider, _rearLeftWheelFriction, _rearLeftWheelExtremumSlip * driftSlip);
            AdjustWheelFriction(rearRightCollider, _rearRightWheelFriction, _rearRightWheelExtremumSlip * driftSlip);

            if (_frontLeftWheelFriction.extremumSlip < _frontLeftWheelExtremumSlip)
            {
                ResetWheelFriction(frontLeftCollider, _frontLeftWheelFriction, _frontLeftWheelExtremumSlip);
                ResetWheelFriction(frontRightCollider, _frontRightWheelFriction, _frontTightWheelExtremumSlip);
                ResetWheelFriction(rearLeftCollider, _rearLeftWheelFriction, _rearLeftWheelExtremumSlip);
                ResetWheelFriction(rearRightCollider, _rearRightWheelFriction, _rearRightWheelExtremumSlip);
                _driftingAxis = 0f;
            }

            if (_frontLeftWheelFriction.extremumSlip > _frontLeftWheelExtremumSlip)
            {
                Invoke(nameof(RecoverTraction), Time.deltaTime);
            }
        }

        private static void AdjustWheelFriction(WheelCollider wheelCollider, WheelFrictionCurve frictionCurve, float slip)
        {
            frictionCurve.extremumSlip = slip;
            wheelCollider.sidewaysFriction = frictionCurve;
        }

        private static void ResetWheelFriction(WheelCollider wheelCollider, WheelFrictionCurve frictionCurve, float defaultSlip)
        {
            frictionCurve.extremumSlip = defaultSlip;
            wheelCollider.sidewaysFriction = frictionCurve;
        }
        
        public void SetCarData(SerializableCarData data)
        {
            carData = data;
            CurrentSpeed = data.baseSpeed;
            CurrentAcceleration = data.baseAcceleration;
        }
        
        public void ApplyUpgrades(float upgradedSpeed, float upgradedAcceleration)
        {
            _upgradedSpeed = upgradedSpeed;
            _upgradedAcceleration = upgradedAcceleration;
        }
    }
}