using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerController))]

public class RespawnBehaviour : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] float respawnTime = 3.15f;
    public UnityEvent OnRespawn = null;

    private Vector3 m_SpawnPosition;
    public Vector3 lastCheckpoint { get; private set; }
    private CheckPoint m_CheckpointActive;

    const string m_ALIVEINPUT = "Gameplay";
    const string m_DEADINPUT = "Dead";

    #region COMPONENTS
    private PlayerController m_PlayerController;
    private Health m_Health;
    #endregion // Components
    
    void Start()
    {
        m_SpawnPosition = transform.position;
        lastCheckpoint = m_SpawnPosition;
        playerInput = GetComponent<PlayerInput>();
        m_PlayerController = GetComponent<PlayerController>();
        m_Health = GetComponent<Health>();
    }

    public void NewCheckpoint(CheckPoint _Checkpoint)
    {
        if (m_CheckpointActive != null)
            m_CheckpointActive.Deactivate();
        lastCheckpoint = _Checkpoint.transform.position;
        m_CheckpointActive = _Checkpoint;
    }
    public void SwitchToDead()
    {
        playerInput.SwitchCurrentActionMap(m_DEADINPUT);
        StartCoroutine(DeathCoroutine());
    }

    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);
        m_PlayerController.OnRespawn();
        m_Health.Start();
        transform.position = lastCheckpoint;
        playerInput.SwitchCurrentActionMap(m_ALIVEINPUT);
        OnRespawn.Invoke();
    }
}
