using System.Collections;
using RootMotion.Dynamics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParkourCamera : MonoBehaviour
{
    public static ParkourCamera Instance;
    public float YAngle;
    private enum FollowMode
    {
        FollowPuppet,
        FollowTransform,
        Stop
    }
    private enum UpdateType
    {
        Update,
        FixedUpdate,
        LateUpdate
    }

    public Vector3 Offset;
    public bool SetOffsetAtStart = true;
    public Vector3 TrickOffset;

    public ParkourSlowMo ParkourSlowMo;
    [Range(0f, 1f)] public float FollowSmooth = 0.7f;
    [Range(0f, 1f)] public float AngleSmooth = 0.7f;

    [SerializeField] private UpdateType _updateType;
    private FollowMode _followMode = FollowMode.FollowPuppet;

    private Transform _head;

    [SerializeField] private PuppetMaster _puppetMaster;
    private float RollLength = 0.7f;
    private bool _fell = false;

    public float SlowTimeForSecondsOnFall = 2f;
    [Range(0f, 1f)] public float SlowTimeChance = 0.5f;

    private Transform _fpsCam, _headTarget;
    [SerializeField] private Vector3 FpsCamRotOffset;
    [SerializeField] private float FpsCamSmoothSpeed;

    public bool LockCamera { get; set; }

    private void Awake()
    {
        Instance = this;
        ParkourSlowMo = GetComponent<ParkourSlowMo>();
        this.LockCamera = false;
    }

    void Start ()
    {
	    if (_puppetMaster == null)
	    {
	        _puppetMaster = FindObjectOfType<PuppetMaster>();
	    }

	    if (SetOffsetAtStart)
	    {
	        Offset = -_puppetMaster.muscles[0].transform.position + transform.position;
        }

        CreateFpsCam();
    }

    private void Update()
    {
        if (_updateType == UpdateType.Update)
            UpdatePosition();
    }
    private void FixedUpdate()
    {
        if (_updateType == UpdateType.FixedUpdate)
            UpdatePosition();
    }
    private void LateUpdate()
    {
        if (_updateType == UpdateType.LateUpdate)
            UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (_followMode == FollowMode.Stop || this.LockCamera) return;

        var rotation = _fpsCam.rotation;
        rotation         = Quaternion.Slerp(rotation, _headTarget.rotation, FpsCamSmoothSpeed *  Time.deltaTime);
        rotation         = Quaternion.Euler(rotation.eulerAngles + FpsCamRotOffset);
        _fpsCam.rotation = rotation;
        _fpsCam.position = _headTarget.position;

        if (_followMode == FollowMode.FollowPuppet)
        {
            Vector3 middle = GetMiddlePos();
            transform.position = Vector3.Lerp(
                middle + Offset + TrickOffset,
                transform.position, FollowSmooth);

            Vector3 lookDir;
            if (_fell)
            {
                lookDir = middle - transform.position;
            }
            else
            {
                lookDir = GetOffsetLookDir(middle);
            }
            Quaternion toRot = Quaternion.FromToRotation(transform.forward, lookDir);
            toRot *= Quaternion.Euler(0f, 0f, YAngle);

            transform.rotation = Quaternion.Lerp(transform.rotation, toRot, AngleSmooth);
        }
        else if (_followMode == FollowMode.FollowTransform)
        {
            if (_head == null) {
                _followMode = FollowMode.Stop;
                return;
            }

            Vector3 middle = GetMiddlePos();

            transform.position = Vector3.Lerp(
                middle + Offset + TrickOffset,
                transform.position, FollowSmooth);

            Vector3 lookDir = _head.position - transform.position;
            Quaternion toRot = Quaternion.FromToRotation(transform.forward, lookDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRot, AngleSmooth);
        }
    }


    private void CreateFpsCam() {
        _headTarget = _puppetMaster.muscles[11].transform;
        _fpsCam     = new GameObject("FPS Cam").transform;
        _fpsCam.gameObject.AddComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("FPS Hide"));
        _fpsCam.gameObject.SetActive(PlayerPrefs.GetInt("FPS_Mode", 0) == 1);
    }

    private Vector3 GetOffsetLookDir(Vector3 mid)
    {
        Vector3 target = mid;
        target.x += Offset.x;
        return target - transform.position;
    }

    private Vector3 GetMiddlePos()
    {
        Vector3 pos = Vector3.zero;

        foreach (var muscle in _puppetMaster.muscles)
        {
            var musclePos = muscle.transform.position;
            pos += musclePos;
        }

        pos /= _puppetMaster.muscles.Length;

        return pos;
    }

    public void OnRoll()
    {
        StartCoroutine(Roll());
    }

    private IEnumerator Roll()
    {
        TrickOffset = new Vector3(0, -1f, 0);
        yield return new WaitForSeconds(1f);
        TrickOffset = new Vector3(0, 0f, 0);
    }
    public void OnJump(float recoveryspeed)
    {
        StartCoroutine(Jump(recoveryspeed));

    }
    private IEnumerator Jump(float recoveryspeed)
    {
        var oldSmooth = FollowSmooth;
        FollowSmooth = 1f;
        while (oldSmooth < FollowSmooth)
        {
            FollowSmooth -= recoveryspeed;
            yield return null;

        }
        FollowSmooth = oldSmooth;
    }

    public void OnLoseBalance()
    {
        _fell = true;
        if (Random.Range(0f, 1f) > SlowTimeChance)
        {
            ParkourSlowMo.SlowFor(SlowTimeForSecondsOnFall);
        }
    }

    public void OnDie()
    {
        print("OnDie camera");
    }

    public void OnRegainBalance()
    {
        _fell = false;
        ParkourSlowMo.UnSlow();
    }

    public void OnHeadLost(Transform head)
    {
        _head = head;
        _followMode = FollowMode.FollowTransform;
    }

    public void OnHeadRegenerated()
    {
        _followMode = FollowMode.FollowPuppet;
        OnRegainBalance();
    }

    public void SwitchSides(bool left)
    {
        if (left && Offset.x > 0
            || !left && Offset.x < 0)
            Offset.x = -Offset.x;
    }


    public void SwitchCam() {
        var active = !_fpsCam.gameObject.activeSelf;
        _fpsCam.gameObject.SetActive(active);
        PlayerPrefs.SetInt("FPS_Mode", active ? 1 : 0);
    }
}
