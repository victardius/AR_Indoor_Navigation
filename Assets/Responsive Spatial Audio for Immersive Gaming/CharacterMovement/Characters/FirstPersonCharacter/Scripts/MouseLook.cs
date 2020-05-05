using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 1f;
        public float YSensitivity = 1f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;


        public static Quaternion m_CharacterTargetRot;
        [NonSerialized]
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;

		private float baseMouseX = 0.0f;
		private float baseMouseY = 0.0f;

        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }

		/// <summary>
		/// Generally, the character orientation is a pure function of the current (mouseX, mouseY)
		/// We compute as (mouseX - baseX, mouseY - baseY) instead
		/// This function sets baseX to Mouse X and vice versa so that a new set point is maintained
		/// </summary>
		public void Reset()
		{
			//this.baseMouseX = CrossPlatformInputManager.GetAxis("Mouse X");
			//this.baseMouseY = CrossPlatformInputManager.GetAxis("Mouse Y");
			//Debug.Log("Reset: " + baseMouseX.ToString() + " " + baseMouseY.ToString());
			m_CharacterTargetRot = Quaternion.identity;
			m_CameraTargetRot = Quaternion.identity;
		}

        public void LookRotation(Transform character, Transform camera)
        {
			var X = CrossPlatformInputManager.GetAxis("Mouse X");
			var Y = CrossPlatformInputManager.GetAxis("Mouse Y");
			float yRot = (X - baseMouseX) * XSensitivity;
            float xRot = (Y - baseMouseY) * YSensitivity;
			//Debug.Log(X.ToString() + " " + Y.ToString());

			m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

            if(clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

            if(smooth)
            {
                character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }

            UpdateCursorLock();
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            if (q.w == 0)
            {
                q.w += 0.001f;
            }
            
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
