using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	[SerializeField]
	private Transform m_TargetDirection;

	[SerializeField]
	private CharacterMovement m_TargetMovement;

	[SerializeField]
	private float m_JumpForce = 10.0f;

	private void Update()
    {
		Vector3 targetInput = Vector3.zero;

		Vector3 forward = new Vector3(m_TargetDirection.forward.x, 0.0f, m_TargetDirection.forward.z).normalized;
		Vector3 right = new Vector3(m_TargetDirection.right.x, 0.0f, m_TargetDirection.right.z).normalized;
		
		if (Input.GetKey(KeyCode.W))
		{
			targetInput += forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			targetInput -= forward;
		}
		if (Input.GetKey(KeyCode.A))
		{
			targetInput -= right;
		}
		if (Input.GetKey(KeyCode.D))
		{
			targetInput += right;
		}

		if (Input.GetKey(KeyCode.Space))
		{
			m_TargetMovement.ApplyJump(m_JumpForce);
		}

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}

		if (Cursor.lockState == CursorLockMode.Locked)
		{
			bool invertY = false;
			Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));

			float mouseSensitivity = 1.0f;
			Vector3 targetRotation = m_TargetDirection.eulerAngles;
			targetRotation.x += mouseMovement.y * mouseSensitivity; // pitch
			targetRotation.y += mouseMovement.x * mouseSensitivity; // yaw
			
			// Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
			//var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
			float rotationLerpTime = 0.01f;
			float rotationLerpPct = 1.0f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
			
			m_TargetDirection.eulerAngles = Vector3.Lerp(m_TargetDirection.eulerAngles, targetRotation, rotationLerpPct);
		}

		m_TargetMovement.AddMovement3D(targetInput);
	}
}
