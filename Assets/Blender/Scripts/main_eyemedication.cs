using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using BNG;
using Unity.VisualScripting;
using UnityEngine.Video;
using TMPro;

public class main_eyemedication : MonoBehaviour
{


    [Header("Scripts")]
    public StartKeyHoldController startKeyHoldController;
    public WaterTapController waterTapController;
    public MedicalGloveController gloveController;
    public Destroyer destroyer;

    [Header("Description")]
    public Transform _descriptionPanel;
    public GameObject GloveInfo;
    public GameObject WaterInfo;
    public GameObject EyeDropBottleInfo;
    public GameObject DropingInfo;



    public Grabbable eyeDropGrab; // assign in Inspector
    public GameObject eyeTarget;  // assign in Inspector (could be a small trigger collider)


    [Header("Medical Items & Triggers")]
    public GameObject _gloveBox;
    




    [Header("Annotations")]

    public List<GameObject> Annotations;


    [Serializable]
    class Steps
    {
        [SerializeField]
        public VideoClip _clip;

        [SerializeField]
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

    public enum VenipuntureSteps
    {
        startTrigger,
        handwash,
        glovesTrigger,
        takeEyeDrop,
        applyEyeDrop,
        endTrigger,
    }


    private VenipuntureSteps activeStep = 0;

    public main_eyemedication(GameObject waterInfo)
    {
        WaterInfo = waterInfo;
    }

    public void Annotator(int index)
    {
        if (Annotations == null || Annotations.Count == 0)
        {
            Debug.LogWarning("The list of GameObjects is empty or not set.");
            return;
        }

        if (index < 0 || index >= Annotations.Count)
        {
            Debug.LogWarning("Index out of range.");
            return;
        }

        for (int i = 0; i < Annotations.Count; i++)
        {
            Annotations[i].SetActive(i == index);
        }
    }
    public void Next()
    {
        if (m_StepList == null || m_StepList.Count == 0)
        {
            Debug.LogError("Step List is empty! Please assign steps in the Inspector.");
            return;
        }

        _descriptionVideoPlayer.Stop();
        m_CurrentStepIndex = (m_CurrentStepIndex + 1) % m_StepList.Count;
        _descriptionVideoPlayer.clip = m_StepList[m_CurrentStepIndex]._clip;
        _descriptionVideoPlayer.Play();
        _descriptionText.text = m_StepList[m_CurrentStepIndex]._description;
    }



    private void Start()
    {
        VenipuntureManager(0);

        _gloveBox.GetComponent<Collider>().enabled = false;
        GloveInfo.SetActive(false);
        WaterInfo.SetActive(false);
        EyeDropBottleInfo.SetActive(false);
        DropingInfo.SetActive(false);

        // Disable grabbing initially
        if (eyeDropGrab != null)
            eyeDropGrab.enabled = false;
    }


    public void Update()
    {
        if (InputBridge.Instance.BButtonDown || Input.GetKeyDown(KeyCode.H))
        {
            _descriptionPanel.gameObject.SetActive(!_descriptionPanel.gameObject.activeSelf);
        }
    }


    public void VenipuntureManager(int newStep = -1)
    {

        if (activeThread != null)
        {
            StopCoroutine(activeThread);
        }
        if (newStep == -1)
        {
            activeStep++;
        }
        else
        {

            activeStep = (VenipuntureSteps)newStep;
        }

        switch (activeStep)
        {
            case VenipuntureSteps.startTrigger:

                Debug.Log(activeStep);
                Debug.Log(m_CurrentStepIndex);
                StartTriggerVerifier();
                break;

            case VenipuntureSteps.handwash:
                WaterInfo.SetActive(true);
                Debug.Log(m_CurrentStepIndex);
                Annotator(0);
                Debug.Log(activeStep);
                
                HandwashTriggerVerifier();
                break;


            case VenipuntureSteps.glovesTrigger:
                Annotator(1);
                WaterInfo.SetActive(false);
                GloveInfo.SetActive(true);
                Debug.Log(m_CurrentStepIndex);
                _gloveBox.GetComponent<Collider>().enabled = true;
                Debug.Log(activeStep);
                glovesTriggerVerifier();
                
                break;


            case VenipuntureSteps.takeEyeDrop:
                Debug.Log("Step: Take the eye drop bottle");

                Annotator(2); // show annotation
                GloveInfo.SetActive(false);
                WaterInfo.SetActive(false);
                EyeDropBottleInfo.SetActive(true);

                // Enable grabbing
                if (eyeDropGrab != null)
                    eyeDropGrab.enabled = true;

                // Start verification coroutine
                activeThread = TakeEyeDropVerifierAction();
                StartCoroutine(activeThread);
                break;


            case VenipuntureSteps.applyEyeDrop:
                Debug.Log("Step: Apply eye drops");

                Annotator(3); // annotation for apply step
                GloveInfo.SetActive(false);
                WaterInfo.SetActive(false);
                EyeDropBottleInfo.SetActive(false);
                DropingInfo.SetActive(true);

                // Optionally enable interaction with the eyeTarget
                if (eyeTarget != null)
                    eyeTarget.GetComponent<Collider>().enabled = true;

                // Start coroutine to verify application
                activeThread = ApplyEyeDropVerifierAction();
                StartCoroutine(activeThread);
                break;



        }


    }







    public void StartTriggerVerifier()
    {
        activeThread = StartTriggerVerifierAction();
        StartCoroutine(StartTriggerVerifierAction());

    }

    private IEnumerator StartTriggerVerifierAction()
    {
        while (true)
        {
            if (startKeyHoldController.isTriggered)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        Next();

        VenipuntureManager();


    }






    private void HandwashTriggerVerifier()
    {
        activeThread = HandwashTriggerVerifierAction();
        StartCoroutine(HandwashTriggerVerifierAction());

    }

    private IEnumerator HandwashTriggerVerifierAction()
    {
        while (true)
        {
            if (waterTapController.isTriggered)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        /*waterTapController.gameObject.GetComponent<BoxCollider>().enabled = false;*/

        destroyer.DestroyComp(waterTapController.gameObject.GetComponent<WaterTapController>());
        Next();
        VenipuntureManager();

    }






    private void glovesTriggerVerifier()
    {
        activeThread = glovesTriggerverifierAction();
        StartCoroutine(glovesTriggerverifierAction());

    }

    private IEnumerator glovesTriggerverifierAction()
    {
        while (true)
        {
            if (gloveController.isTriggered)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        destroyer.DestroyComp(gloveController.gameObject.GetComponent<MedicalGloveController>());
        Next();
        VenipuntureManager();
    }

    private IEnumerator TakeEyeDropVerifierAction()
    {
        while (true)
        {
            if (eyeDropGrab != null && eyeDropGrab.BeingHeld)
            {
                Debug.Log("Eye drop bottle picked!");

                
                break; // exit loop
            }

            yield return new WaitForFixedUpdate();
        }

        Next();                // move to next description step
        VenipuntureManager();  // move to applyEyeDrop step
    }


    private IEnumerator ApplyEyeDropVerifierAction()
    {
        bool applied = false;

        while (!applied)
        {
            // Replace this with your real check
            // Example: player drops liquid into the eye collider
            if (/* player applies drops to eyeTarget */ false)
            {
                applied = true;
                Debug.Log("Eye drops applied!");

                // Disable collider to prevent repeated application
                if (eyeTarget != null)
                    eyeTarget.GetComponent<Collider>().enabled = false;
            }

            yield return new WaitForFixedUpdate();
        }

        Next();
        VenipuntureManager(); // go to next step
    }

}
