using System;
using System.Collections.Generic;
using Actions;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed = 0;
        [SerializeField] private float m_RunSpeed = 0;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten = 0;
        [SerializeField] private float m_JumpSpeed = 0;
        [SerializeField] private float m_StickToGroundForce = 0;
        [SerializeField] private float m_GravityMultiplier = 0;
        [SerializeField] private MouseLook m_MouseLook = null;
        [SerializeField] private bool m_UseFovKick = false;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob = false;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval = 0;
        private List<AudioClip> m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound = null;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound = null;           // the sound played when character touches back on ground.

		private Dictionary<AppConstants.SpaceNames, List<string>> footStepSounds = new Dictionary<AppConstants.SpaceNames, List<string>>
		{
			{ AppConstants.SpaceNames.FOOTPATH, new List<string> { "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/Footsteps/Wood/1", "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/Footsteps/Wood/2", "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/Footsteps/Wood/3", "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/Footsteps/Wood/4", "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/Footsteps/Wood/5", "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/Footsteps/Wood/6", "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/Footsteps/Wood/7" } },
			{ AppConstants.SpaceNames.UNSPECIFIED, new List<string> { "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/Footsteps/Ground/1", "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/Footsteps/Ground/2", "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/Footsteps/Ground/3", "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/Footsteps/Ground/4" } }
		};

		private Dictionary<AppConstants.SpaceNames, List<AudioClip>> m_footStepSounds = new Dictionary<AppConstants.SpaceNames, List<AudioClip>>();

		private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;

        private AudioSource m_AudioSource; // this is used for footsteps
		private bool no_bump_this_frame = true;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }
        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            Dispatch.handlers.Remove(typeof(Actions.User.OrientationResetPressed));
            Dispatch.handlers.Remove(typeof(Actions.User.UserEnteredSpace));

            Dispatch.registerHandler(typeof(Actions.User.OrientationResetPressed), this.ResetView);
            Dispatch.registerHandler(typeof(Actions.User.UserEnteredSpace), this.UpdateFootsteps);
        }
        // Use this for initialization
        private void Start()
        {
			foreach(var pair in footStepSounds)
			{
				m_footStepSounds[pair.Key] = new List<AudioClip>();
				foreach(var st in pair.Value)
				{
					var clip = Resources.Load(st) as AudioClip;
					if (clip != null)
					{
						m_footStepSounds[pair.Key].Add(clip); 
					}
					else
					{
						throw new System.NullReferenceException("Could not find resource: " + st);
					}
				}
			}
			m_FootstepSounds = m_footStepSounds[AppConstants.SpaceNames.UNSPECIFIED];


			m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
			Dispatch.registerHandler(typeof(Actions.User.OrientationResetPressed), this.ResetView);
			Dispatch.registerHandler(typeof(Actions.User.UserEnteredSpace), this.UpdateFootsteps);
		}

		private AppState UpdateFootsteps(Base action, AppState state)
		{
			var action_ = action as Actions.User.UserEnteredSpace;

			if (action_ == null)
			{
				throw new System.ArgumentException("Incorrect action routed to: "
					+ GetType().ToString()
					//+ " method: " + GetType().DeclaringMethod.ToString()
					+ " on GameObject: " + gameObject.name);
			}

			switch(action_.buildingName)
			{
				case AppConstants.SpaceNames.FOOTPATH:
					m_FootstepSounds =  m_footStepSounds[action_.buildingName];
					break;
				default:
					m_FootstepSounds = m_footStepSounds[AppConstants.SpaceNames.UNSPECIFIED];
					break;
			}
			return state;
		}


		// Update is called once per frame
		private void Update()
        {
			no_bump_this_frame = true;
			RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

		private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

			if (desiredMove.magnitude > 0.4f)
			{
				Dispatch.dispatch(new Actions.User.SignificantMovement());
			}

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }

		private List<AudioClip> GetFootStepArrayForSpace(AppConstants.SpaceNames space)
		{
			if(footStepSounds.ContainsKey(space))
			{
				return m_footStepSounds[space];
			}
			return m_footStepSounds[AppConstants.SpaceNames.UNSPECIFIED];
		}

        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array, excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Count);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);

            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }

		public AppState ResetView(Actions.Base action, AppState state)
		{
			m_MouseLook.Reset();
			return state;
		}

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
			if (no_bump_this_frame && (m_CharacterController.velocity != Vector3.zero) && Vector3.Dot(hit.normal, Vector3.up) < 0.7f)
			{
				//Debug.Log(hit.normal);
				no_bump_this_frame = false;
				Dispatch.dispatch(new Actions.User.OnBump(hit));
			}
			Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
