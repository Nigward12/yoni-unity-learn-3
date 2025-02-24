using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CameraControlTrigger : MonoBehaviour
{
    public CCTCustomInspectorObjects customInspectorObjects;

    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { 
            if (customInspectorObjects.panCameraOnContact)
            {
                CinemachineCameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance,
                    customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position -
                    triggerCollider.bounds.center).normalized;

            if (customInspectorObjects.cameraSwapMode is SwapMode.SwapHorizontal)
                CinemachineCameraManager.instance.SwapCameraHorizontal(customInspectorObjects.cameraOnLeft,
                    customInspectorObjects.cameraOnRight, exitDirection, customInspectorObjects.leftTarget,
                    customInspectorObjects.rightTarget,customInspectorObjects.customBlendTime);
            else if (customInspectorObjects.cameraSwapMode is SwapMode.SwapVertical)
                CinemachineCameraManager.instance.SwapCameraVertical(customInspectorObjects.cameraOnTop,
                    customInspectorObjects.cameraOnBottom, exitDirection, customInspectorObjects.topTarget,
                    customInspectorObjects.bottomTarget, customInspectorObjects.customBlendTime);
            else if (customInspectorObjects.cameraSwapMode is SwapMode.SwitchTargetHorizontal)
            {
                CinemachineCameraManager.instance.TargetSwitchHorizontal(customInspectorObjects.leftTarget
                    , customInspectorObjects.rightTarget, exitDirection, customInspectorObjects.switchInstantly);
            }

            if (customInspectorObjects.panCameraOnContact)
            {
                CinemachineCameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance,
                    customInspectorObjects.panTime, customInspectorObjects.panDirection, true);
            }
        }
    }
}

[System.Serializable]
public class CCTCustomInspectorObjects
{
    public SwapMode cameraSwapMode = SwapMode.None;

    [HideInInspector] public CinemachineCamera cameraOnLeft;
    [HideInInspector] public CinemachineCamera cameraOnRight;
    [HideInInspector] public CinemachineCamera cameraOnTop;
    [HideInInspector] public CinemachineCamera cameraOnBottom;
    [HideInInspector] public float customBlendTime = -1;

    [HideInInspector] public Transform leftTarget;
    [HideInInspector] public Transform rightTarget;
    [HideInInspector] public Transform topTarget;
    [HideInInspector] public Transform bottomTarget;
    [HideInInspector] public bool switchInstantly = false;

    public bool panCameraOnContact;
    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 2.25f;
    [HideInInspector] public float panTime = 0.35f;

}

public enum SwapMode
{
    None,
    SwapHorizontal,
    SwapVertical,
    SwitchTargetHorizontal,
    SwitchTargetVertical
}

public enum PanDirection
{
    UP,DOWN,LEFT,RIGHT
}

#if UNITY_EDITOR

[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor: Editor
{
    CameraControlTrigger cameraControlTrigger;

    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        CCTCustomInspectorObjects customInspector = cameraControlTrigger.customInspectorObjects;

        EditorGUILayout.LabelField("Swap Mode", EditorStyles.boldLabel);

        customInspector.cameraSwapMode = (SwapMode)EditorGUILayout.EnumPopup("Swap Mode", customInspector.cameraSwapMode);

        switch (customInspector.cameraSwapMode)
        {
            case SwapMode.SwapHorizontal:
                customInspector.cameraOnLeft = (CinemachineCamera)EditorGUILayout.ObjectField("Camera On Left",
                    customInspector.cameraOnLeft, typeof(CinemachineCamera), true);
                customInspector.cameraOnRight = (CinemachineCamera)EditorGUILayout.ObjectField("Camera On Right",
                    customInspector.cameraOnRight, typeof(CinemachineCamera), true);
                customInspector.leftTarget = (Transform)EditorGUILayout.ObjectField("Left Target",
                    customInspector.leftTarget, typeof(Transform), true);
                customInspector.rightTarget = (Transform)EditorGUILayout.ObjectField("Right Target",
                    customInspector.rightTarget, typeof(Transform), true);
                customInspector.customBlendTime = EditorGUILayout.FloatField("optional custom blend time " +
                    "(above or equal to 0)",customInspector.customBlendTime);
                break;

            case SwapMode.SwapVertical:
                customInspector.cameraOnTop = (CinemachineCamera)EditorGUILayout.ObjectField("Camera On Top",
                    customInspector.cameraOnTop, typeof(CinemachineCamera), true);
                customInspector.cameraOnBottom = (CinemachineCamera)EditorGUILayout.ObjectField("Camera On Bottom"
                    , customInspector.cameraOnBottom, typeof(CinemachineCamera), true);
                customInspector.topTarget = (Transform)EditorGUILayout.ObjectField("Top Target",
                    customInspector.topTarget, typeof(Transform), true);
                customInspector.bottomTarget = (Transform)EditorGUILayout.ObjectField("bottom Target",
                    customInspector.bottomTarget, typeof(Transform), true);
                customInspector.customBlendTime = EditorGUILayout.FloatField("optional custom blend time " +
                    "(above or equal to 0)",customInspector.customBlendTime);
                break;

            case SwapMode.SwitchTargetHorizontal:
                customInspector.leftTarget = (Transform)EditorGUILayout.ObjectField("Left Target",
                    customInspector.leftTarget, typeof(Transform), true);
                customInspector.rightTarget = (Transform)EditorGUILayout.ObjectField("Right Target",
                    customInspector.rightTarget, typeof(Transform), true);
                customInspector.switchInstantly = EditorGUILayout.Toggle
                    ("Switch Instantly", customInspector.switchInstantly);
                break;

        }

        EditorGUILayout.Space();
        customInspector.panCameraOnContact = EditorGUILayout.Toggle("Pan Camera On Contact", customInspector.panCameraOnContact);

        if (customInspector.panCameraOnContact)
        {
            customInspector.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction", customInspector.panDirection);
            customInspector.panDistance = EditorGUILayout.FloatField("Pan Distance", customInspector.panDistance);
            customInspector.panTime = EditorGUILayout.FloatField("Pan Time", customInspector.panTime);
        }

        if (GUI.changed)
            EditorUtility.SetDirty(cameraControlTrigger);
    }
#endif
}
