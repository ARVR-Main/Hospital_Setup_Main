using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;
using BNG;

/// <summary>
/// Eye medication step-by-step tutorial manager adapted from Venipuncture script.
/// Steps: Start -> Handwash -> Wear Gloves -> Take eye-dropper -> Apply drops -> End
/// Integrates your existing EyeDropSpawner, EyeController, and EyeDropReceiver logic.
/// </summary>
public class EyeMedication : MonoBehaviour
{
    [Header("External Controllers")]
    public StartKeyHoldController startKeyHoldController;
    public WaterTapController waterTapController;
    public MedicalGloveController gloveController;
    public EyeDropSpawner eyeDropSpawner;
    public EyeController eyeController;
    public EyeDropReceiver leftEyeReceiver;
    public EyeDropReceiver rightEyeReceiver;
    public Destroyer destroyer;

    [Header("UI / Description")]
    public Transform _descriptionPanel;
    public GameObject GloveInfo;
    public GameObject WaterInfo;

    [Header("SnapZone Slots / Items")]
    public GameObject EyeDropperSlot;
    public GameObject _eyeDropper;
    public GameObject _gloveBox;

    [Header("Annotations")]
    public List<GameObject> Annotations;

    [Serializable]
    class Steps
    {
        public VideoClip _clip;
        public string _description;
    }

    [SerializeField]
    public VideoPlayer _descriptionVideoPlayer;
    [SerializeField]
    public TextMeshProUGUI _descriptionText;
    [SerializeField]
    List<Steps> m_StepList = new List<Steps>();

    int m_CurrentStepIndex = 0;
    IEnumerator activeThread;

    public enum EyeMedicationSteps
    {
        startTrigger,
        handwash,
        glovesTrigger,
        takeEyeDropperTrigger,
        applyDropsTrigger,
        endTrigger,
    }

    private EyeMedicationSteps activeStep = 0;

    public void Annotator(int index)
    {
        if (Annotations == null || Annotations.Count == 0) return;
        for (int i = 0; i < Annotations.Count; i++)
            Annotations[i].SetActive(i == index);
    }

    public void Next()
    {
        if (_descriptionVideoPlayer != null) _descriptionVideoPlayer.Stop();
        if (m_StepList == null || m_StepList.Count == 0) return;
        m_CurrentStepIndex = (m_CurrentStepIndex + 1) % m_StepList.Count;
        if (_descriptionVideoPlayer != null) { _descriptionVideoPlayer.clip = m_StepList[m_CurrentStepIndex]._clip; _descriptionVideoPlayer.Play(); }
        if (_descriptionText != null) _descriptionText.text = m_StepList[m_CurrentStepIndex]._description;
    }

    private void Start()
    {
        EyeMedicationManager(0);
        if (_eyeDropper != null && _eyeDropper.GetComponent<Grabbable>() != null) _eyeDropper.GetComponent<Grabbable>().enabled = false;
        if (_gloveBox != null && _gloveBox.GetComponent<Collider>() != null) _gloveBox.GetComponent<Collider>().enabled = false;
        if (GloveInfo != null) GloveInfo.SetActive(false);
        if (WaterInfo != null) WaterInfo.SetActive(false);
    }

    private void Update()
    {
        if ((InputBridge.Instance != null && InputBridge.Instance.BButtonDown) || Input.GetKeyDown(KeyCode.H))
            if (_descriptionPanel != null) _descriptionPanel.gameObject.SetActive(!_descriptionPanel.gameObject.activeSelf);
    }

    public void EyeMedicationManager(int newStep = -1)
    {
        if (activeThread != null) { StopCoroutine(activeThread); activeThread = null; }
        if (newStep == -1) activeStep++; else activeStep = (EyeMedicationSteps)newStep;

        switch (activeStep)
        {
            case EyeMedicationSteps.startTrigger: StartTriggerVerifier(); break;
            case EyeMedicationSteps.handwash: Annotator(0); WaterInfo?.SetActive(true); HandwashTriggerVerifier(); break;
            case EyeMedicationSteps.glovesTrigger: Annotator(1); GloveInfo?.SetActive(true); WaterInfo?.SetActive(false); glovesTriggerVerifier(); break;
            case EyeMedicationSteps.takeEyeDropperTrigger: Annotator(2); TakeEyeDropperVerifier(); break;
            case EyeMedicationSteps.applyDropsTrigger: Annotator(3); ApplyDropsVerifier(); break;
            case EyeMedicationSteps.endTrigger: Annotator(4); break;
        }
    }

    private void StartTriggerVerifier() { activeThread = StartTriggerCoroutine(); StartCoroutine(activeThread); }
    private IEnumerator StartTriggerCoroutine() { while (!startKeyHoldController.isTriggered) yield return new WaitForFixedUpdate(); Next(); EyeMedicationManager(); }

    private void HandwashTriggerVerifier() { activeThread = HandwashCoroutine(); StartCoroutine(activeThread); }
    private IEnumerator HandwashCoroutine() { while (!waterTapController.isTriggered) yield return new WaitForFixedUpdate(); destroyer.DestroyComp(waterTapController); Next(); EyeMedicationManager(); }

    private void glovesTriggerVerifier() { activeThread = GlovesCoroutine(); StartCoroutine(activeThread); }
    private IEnumerator GlovesCoroutine() { while (!gloveController.isTriggered) yield return new WaitForFixedUpdate(); destroyer.DestroyComp(gloveController); Next(); EyeMedicationManager(); }

    private void TakeEyeDropperVerifier() { activeThread = TakeDropperCoroutine(); StartCoroutine(activeThread); }
    private IEnumerator TakeDropperCoroutine()
    {
        if (_eyeDropper != null && _eyeDropper.GetComponent<Grabbable>() != null) _eyeDropper.GetComponent<Grabbable>().enabled = true;
        while (_eyeDropper != null && !_eyeDropper.GetComponent<Grabbable>().BeingHeld) yield return new WaitForFixedUpdate();
        Next(); EyeMedicationManager();
    }

    private void ApplyDropsVerifier() { activeThread = ApplyDropsCoroutine(); StartCoroutine(activeThread); }
    private IEnumerator ApplyDropsCoroutine()
    {
        // Wait until left and right eyes received the required drops
        bool leftDone = false, rightDone = false;
        while (!leftDone || !rightDone)
        {
            leftDone = leftEyeReceiver != null && leftEyeReceiver.isActiveAndEnabled;
            rightDone = rightEyeReceiver != null && rightEyeReceiver.isActiveAndEnabled;
            yield return new WaitForFixedUpdate();
        }
        Next(); EyeMedicationManager();
    }
}
