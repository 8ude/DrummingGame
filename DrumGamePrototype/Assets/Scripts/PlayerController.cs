using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using Beat;

public class PlayerController : MonoBehaviour {


	private bool gameStarted = false;

    public InputEvaluator inputEvaluator;

    public float returnToMiddleTime;
    float returnTimer;

    public GameObject burstParticle;
    public GameObject destroyParticle;
    public AudioClip kickSound, snareSound, hatSound, lTomSound, hTomSound;
    AudioSource mySource;

    public SongManager songManager;

	private bool isDoubleTouching = false;

    public float jumpHeight;
    public float dropHeight;

    public float neutralHeight;
    public float dashLeftDistance;
    public float dashRightDistance;
    [SerializeField]float burstCooldown, jumpCooldown, diveCooldown, leftCoolDown, rightCoolDown;

    public Vector3 startPos;
    double nextEigthCue;

    bool returning;

    public GameObject level;
    LevelParent levelParent;

    public enum drumTriggers { Kick, Snare, LowTom, HiTom, HiHat };


    public class DrumTriggerPress<T> {
        private readonly T[] _triggerPressArray = new T[(int)drumTriggers.HiHat + 1];

        public void Clear() {
            Array.Clear(_triggerPressArray, 0, (int)drumTriggers.HiHat + 1);
        }

        public T this[drumTriggers i] {

            get {
                return _triggerPressArray[(int)i];
            }
            set {
                _triggerPressArray[(int)i] = value;
            }
        }
    }

    DrumTriggerPress<bool> triggersPressedThisFrame = new DrumTriggerPress<bool>();
    DrumTriggerPress<bool> triggersDoublePressedThisWindow = new DrumTriggerPress<bool>();
    public DrumTriggerPress<bool> triggersPressedThisWindow = new DrumTriggerPress<bool>();

    double nextBeat;

    void Awake() {
        level = GameObject.FindWithTag("Level");
        levelParent = level.GetComponent<LevelParent>();
        songManager = SongManager.instance;

        mySource = GetComponent<AudioSource>();
    }

	// Use this for initialization
	void Start () {
        returning = false;
        burstCooldown = 0f;
        jumpCooldown = 0f;
        diveCooldown = 0f;
        leftCoolDown = 0f;
        rightCoolDown = 0f;
	}

    // Update is called once per frame
    private void FixedUpdate() {
        ProcessInputs();
        burstCooldown += Time.fixedDeltaTime;
        jumpCooldown += Time.fixedDeltaTime;
        diveCooldown += Time.fixedDeltaTime;
        leftCoolDown += Time.fixedDeltaTime;
        rightCoolDown += Time.fixedDeltaTime;

	}


    public void ProcessInputs() {
        if (Input.GetButtonDown("drum_far_right") && rightCoolDown >= Clock.Instance.SixteenthLength()){
            Debug.Log("drum_far_right");
            transform.DOKill();
            Sequence DashRightSequence = DOTween.Sequence();
            DashRightSequence.Append(transform.DOMoveX(dashRightDistance, Clock.Instance.ThirtySecondLength()).SetEase(Ease.InFlash));
            DashRightSequence.AppendInterval(Clock.Instance.SixteenthLength());
            DashRightSequence.Append(transform.DOMoveX(0f, Clock.Instance.ThirtySecondLength()).SetEase(Ease.InFlash));
            DashRightSequence.Play();
            mySource.PlayOneShot(hTomSound);

            triggersPressedThisFrame[drumTriggers.HiTom] = true;

            if (inputEvaluator.withinWindow) {
                triggersPressedThisWindow[drumTriggers.HiTom] = true;
            }

            levelParent.RotateRight();

            rightCoolDown = 0f;

		}

        if (Input.GetButtonDown("drum_right") && jumpCooldown >= Clock.Instance.SixteenthLength()) {
			//Debug.Log("drum_right");

            //Jump

            transform.DOKill();
            Sequence jumpSequence = DOTween.Sequence();
            jumpSequence.Append(transform.DOMoveY(jumpHeight, Clock.Instance.SixteenthLength()).SetEase(Ease.OutCubic));
            jumpSequence.AppendInterval(Clock.Instance.EighthLength());
            jumpSequence.Append(transform.DOMoveY(neutralHeight, Clock.Instance.EighthLength()).SetEase(Ease.OutQuad));
            jumpSequence.Play();
            mySource.PlayOneShot(snareSound);

            triggersPressedThisFrame[drumTriggers.Snare] = true;

            if (inputEvaluator.withinWindow) {
                triggersPressedThisWindow[drumTriggers.Snare] = true;
            }

            jumpCooldown = 0f;




		}
        if (Input.GetButtonDown("drum_left") && burstCooldown >= Clock.Instance.SixteenthLength()) {

            //HI HAT INPUT

            Instantiate(burstParticle, transform.position, Quaternion.identity);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2, 1, QueryTriggerInteraction.Collide);

            foreach (Collider collider in hitColliders) {
                if (collider.gameObject.tag == "burst target") {
                    Instantiate(destroyParticle, collider.transform.position, Quaternion.identity);
                }
            }

            transform.DOScaleY(0.5f, Clock.Instance.SixteenthLength()).OnComplete(ResetScale);
            mySource.PlayOneShot(hatSound);

            //Cache inputs for evaluation
            triggersPressedThisFrame[drumTriggers.HiHat] = true;

            if (inputEvaluator.withinWindow) {
                triggersPressedThisWindow[drumTriggers.HiHat] = true;

            }

            burstCooldown = 0f;
            
			
			Debug.Log("drum_left");
		}
        if (Input.GetButtonDown("drum_far_left") && leftCoolDown >= Clock.Instance.SixteenthLength()) {
            
            transform.DOKill();
            Sequence DashLeftSequence = DOTween.Sequence();
            DashLeftSequence.Append(transform.DOMoveX(dashLeftDistance, Clock.Instance.ThirtySecondLength()).SetEase(Ease.InFlash));
            DashLeftSequence.AppendInterval(Clock.Instance.SixteenthLength());
            DashLeftSequence.Append(transform.DOMoveX(0f, Clock.Instance.ThirtySecondLength()).SetEase(Ease.InFlash));

            DashLeftSequence.Play();
            mySource.PlayOneShot(lTomSound);

            triggersPressedThisFrame[drumTriggers.LowTom] = true;

            if (inputEvaluator.withinWindow) {
                triggersPressedThisWindow[drumTriggers.LowTom] = true;
            }

            leftCoolDown = 0f;

            levelParent.RotateLeft();
			
			Debug.Log("drum_far_left");
		}
        if (Input.GetButtonDown("drum_kick") && diveCooldown >= Clock.Instance.SixteenthLength()) {

            transform.DOKill();
            Sequence smashSequence = DOTween.Sequence();
            smashSequence.Append(transform.DOMoveY(dropHeight, Clock.Instance.SixteenthLength()).SetEase(Ease.OutExpo));
            Camera.main.DOShakePosition(0.2f, 1f, 200, 90f, true);
            smashSequence.Append(transform.DOMoveY(neutralHeight, Clock.Instance.ThirtySecondLength()).SetEase(Ease.OutBounce));
            smashSequence.Append(transform.DOScaleY(0.2f, Clock.Instance.SixteenthLength()));
            smashSequence.Append(transform.DOScaleY(1f, Clock.Instance.SixteenthLength()).SetEase(Ease.OutBounce));

            smashSequence.Append(transform.DOShakeScale(0.2f, 1f, 20, 150f, false));
            smashSequence.Play();
            mySource.PlayOneShot(kickSound);

            triggersPressedThisFrame[drumTriggers.Kick] = true;

			if (inputEvaluator.withinWindow) {
                triggersPressedThisWindow[drumTriggers.Kick] = true;
			}

            diveCooldown = 0f;
            Debug.Log("drum_kick");
        }

    }

    public void ResetScale() {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}