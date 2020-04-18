using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementSettings
{
	[Header("Movement")]
	public float MovementAcceleration = 200.0f;
	public float MovementMaxSpeed = 10.0f;
	public float MovementDrag = 10.0f;

	[Header("Forces")]
	public float ForceMaxSpeed = 1000.0f;
	public float ForceDrag = 0.5f;
	public Vector3 Gravity = new Vector3(0.0f, -9.8f, 0.0f);
}

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
	[SerializeField]
	private MovementSettings m_GroundSettings;

	[SerializeField]
	private MovementSettings m_AirSettings;
	
	private CharacterController m_Controller;
	private Vector3 m_MovementInput;
	private Vector3 m_ImpulseInput;

	private Vector3 m_MovementVelocity;
	private Vector3 m_ForceVelocity;
	private bool m_WasGrounded;


	private void Awake()
	{
		m_Controller = GetComponent<CharacterController>();
	}
	
	// Can be both airborne and grounded for 1 frame
	public bool IsAirborne
	{
		get => !m_Controller.isGrounded || !m_WasGrounded;
	}

	public bool IsGrounded
	{
		get => m_Controller.isGrounded;
	}

	private void Update()
	{
		// Apply movement
		{
			MovementSettings settings = IsAirborne ? m_AirSettings : m_GroundSettings;
			Debug.LogFormat("IsAirborne {0}", IsAirborne);

			// Update movement
			Vector3 frameInput = Vector3.ClampMagnitude(m_MovementInput * settings.MovementAcceleration, settings.MovementAcceleration) * Time.deltaTime;
			m_MovementVelocity = Vector3.ClampMagnitude(m_MovementVelocity + frameInput, settings.MovementMaxSpeed);

			// Update forces
			Vector3 forceFrameInput = settings.Gravity * Time.deltaTime + m_ImpulseInput;
			m_ForceVelocity = Vector3.ClampMagnitude(m_ForceVelocity + forceFrameInput, settings.ForceMaxSpeed);

			m_MovementVelocity = ApplyMovementInternal(m_MovementVelocity);
			m_ForceVelocity = ApplyMovementInternal(m_ForceVelocity);

			// Apply drag
			m_MovementVelocity *= Mathf.Clamp01(1.0f - settings.MovementDrag * Time.deltaTime);
			m_ForceVelocity *= Mathf.Clamp01(1.0f - settings.ForceDrag * Time.deltaTime);

			// Reset
			m_WasGrounded = m_Controller.isGrounded;
			m_MovementInput = Vector3.zero;
			m_ImpulseInput = Vector3.zero;
		}
	}

	private Vector3 ApplyMovementInternal(Vector3 velocity)
	{
		Vector3 startPos = m_Controller.transform.position;
		if (m_Controller.Move(velocity * Time.deltaTime) != CollisionFlags.None)
		{
			Vector3 endPos = m_Controller.transform.position;
			return endPos - startPos;
		}

		return velocity;
	}

	public void AddMovement2D(Vector2 dir, float force = 1.0f)
	{
		Vector2 input = dir.normalized * force;
		m_MovementInput += new Vector3(input.x, 0, input.y);
	}

	public void AddMovement3D(Vector3 dir, float force = 1.0f)
	{
		m_MovementInput += dir.normalized * force;
	}

	public void AddImpulse3D(Vector3 dir, float force = 1.0f)
	{
		m_ImpulseInput += dir.normalized * force;
	}

	public bool ApplyJump(float force)
	{
		// We have a frame where we can maintain airborne movement to mimic bunny hopping
		if (IsGrounded || m_WasGrounded)
		{
			m_ForceVelocity = Vector3.zero;
			AddImpulse3D(Vector3.up, force);
			return true;
		}

		return false;
	}
}
