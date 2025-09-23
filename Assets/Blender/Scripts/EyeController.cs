using System;
using System.Collections;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    [Header("Renderer & blendshape names")]
    public SkinnedMeshRenderer faceRenderer;
    public string leftBlendName = "Left Eye";
    public string rightBlendName = "Right Eye";
    public string underEyeBlendName = "Key 3";

    [Header("Drop / blink settings")]
    public int dropsNeededPerEye = 3;
    public float closeSpeed = 70f;     
    public float openSpeed = 110f;       
    public float holdClosedTime = 1f; 

    [Header("Blink settings")]
    public float blinkDownTime = 0.5f; 
    public float blinkUpTime = 0.5f;   

    int leftIndex = -1, rightIndex = -1, underEyeIndex = -1;
    int leftCount = 0, rightCount = 0;
    bool leftBusy = false, rightBusy = false;

    void Start()
    {
        if (faceRenderer == null)
        {
            Debug.LogError("EyeController: faceRenderer not assigned!");
            enabled = false;
            return;
        }

        var mesh = faceRenderer.sharedMesh;
        leftIndex = mesh.GetBlendShapeIndex(leftBlendName);
        rightIndex = mesh.GetBlendShapeIndex(rightBlendName);
        underEyeIndex = mesh.GetBlendShapeIndex(underEyeBlendName);
    }

 
    public void RegisterDropLeft()
    {
        if (leftBusy) return;

        leftCount++;
        if (leftCount >= dropsNeededPerEye)
        {
            leftCount = 0;
            leftBusy = true;
            StartCoroutine(RealisticBlinkSequence(leftIndex, () => leftBusy = false));
        }
    }

    public void RegisterDropRight()
    {
        if (rightBusy) return;

        rightCount++;
        if (rightCount >= dropsNeededPerEye)
        {
            rightCount = 0;
            rightBusy = true;
            StartCoroutine(RealisticBlinkSequence(rightIndex, () => rightBusy = false));
        }
    }

   
    public void HandTouchUnderEye()
    {
        if (underEyeIndex >= 0)
            faceRenderer.SetBlendShapeWeight(underEyeIndex, 100f);
    }

    public void HandRemoveUnderEye()
    {
        if (underEyeIndex >= 0)
            faceRenderer.SetBlendShapeWeight(underEyeIndex, 0f);
    }

  
    IEnumerator RealisticBlinkSequence(int blendIndex, Action onComplete)
    {
        if (blendIndex < 0) { onComplete?.Invoke(); yield break; }

        
        while (faceRenderer.GetBlendShapeWeight(blendIndex) < 50f)
        {
            float cur = faceRenderer.GetBlendShapeWeight(blendIndex);
            float next = Mathf.MoveTowards(cur, 50f, closeSpeed * Time.deltaTime);
            faceRenderer.SetBlendShapeWeight(blendIndex, next);
            yield return null;
        }

        
        yield return BlinkOnce(blendIndex, 60f, 100f); 
        yield return BlinkOnce(blendIndex, 60f, 100f); 

       
        faceRenderer.SetBlendShapeWeight(blendIndex, 100f);
        yield return new WaitForSeconds(holdClosedTime);

        
        while (faceRenderer.GetBlendShapeWeight(blendIndex) > 0f)
        {
            float cur = faceRenderer.GetBlendShapeWeight(blendIndex);
            float next = Mathf.MoveTowards(cur, 0f, openSpeed * Time.deltaTime);
            faceRenderer.SetBlendShapeWeight(blendIndex, next);
            yield return null;
        }

        faceRenderer.SetBlendShapeWeight(blendIndex, 0f);
        onComplete?.Invoke();
    }


    IEnumerator BlinkOnce(int blendIndex, float start, float full)
    {
     
        faceRenderer.SetBlendShapeWeight(blendIndex, start);

        
        while (faceRenderer.GetBlendShapeWeight(blendIndex) < full)
        {
            float cur = faceRenderer.GetBlendShapeWeight(blendIndex);
            float next = Mathf.MoveTowards(cur, full, (100f / blinkDownTime) * Time.deltaTime);
            faceRenderer.SetBlendShapeWeight(blendIndex, next);
            yield return null;
        }

       
        while (faceRenderer.GetBlendShapeWeight(blendIndex) > start)
        {
            float cur = faceRenderer.GetBlendShapeWeight(blendIndex);
            float next = Mathf.MoveTowards(cur, start, (100f / blinkUpTime) * Time.deltaTime);
            faceRenderer.SetBlendShapeWeight(blendIndex, next);
            yield return null;
        }
    }
}
