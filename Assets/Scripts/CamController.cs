using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamController : MonoBehaviour
{
    [Header("Gameobject References")]
    [SerializeField] private GameObject player;
    [SerializeField] private InputActionReference togglePivotPoint;
    //[SerializeField] private CinemachineCamera vcam;
    

    [Header("Spherecast stuff")]
    [SerializeField] private float sphereCastRadius;
    [SerializeField] private float sphereCastDistance;
    [SerializeField] private LayerMask lockonLayer;

    private Transform lockedTarget;
    private Transform playerPivotPoint;

    private Camera mainCamera;

    private PlayerController playerController;
    private LookControls lookControls;

    private bool coroutineRunning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lookControls = player.GetComponent<LookControls>();
        playerController = player.GetComponent<PlayerController>();

        mainCamera = Camera.main;

        playerPivotPoint = player.GetComponentInChildren<Transform>();
        lockedTarget = playerPivotPoint.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (togglePivotPoint.action.IsPressed() && coroutineRunning == false)
        {
            coroutineRunning = true;
            StartCoroutine(ToggleCameraTarget());
        }
    }

    private IEnumerator ToggleCameraTarget()
    {
        RaycastHit hit;
        Debug.Log("TAB PRESSED");

        if (Physics.SphereCast(playerPivotPoint.transform.position, sphereCastRadius, mainCamera.transform.forward, out hit, sphereCastDistance, lockonLayer))
        {
            Debug.Log(hit.transform);
            //locked onto enemy
            if (hit.transform != lockedTarget)
            {
                Debug.Log("Swapping target");
                lockedTarget = hit.transform;

                playerController.ToggleWorldDirection("locked");

                lookControls.mouseEnabled = false;
                lookControls.lockTarget = lockedTarget;
                

                yield return new WaitForSeconds(2f);
            }
            //unlocked
            else
            {
                Debug.Log("Returning to player");

                lockedTarget = playerPivotPoint.transform;

                playerController.ToggleWorldDirection("unlocked");

                lookControls.mouseEnabled = true;
                lookControls.lockTarget = lockedTarget;

                yield return new WaitForSeconds(2f);
            }
        }
        coroutineRunning = false;
    }
}
