using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chonker.FPS_Controller.Camera.Animation_Curve;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

public class ChonkerCameraAnimationCurveManager : MonoBehaviour
{
    [SerializeField] private Transform cameraShakeParentContainer;
    [SerializeField] private ChonkerAnimationCurvePlayer cameraShakeOneShotContainer;
    [Space]
    [SerializeField] private ChonkerCameraAnimationCurveData blankCurveData;
    [SerializeField] private ChonkerCameraAnimationCurveData oneShotHitReactionCurve;
    
    ChonkerCameraAnimationCurveData currentCurveData;
    private bool isTransitioning;
    private float currentNormalizedTime;


    private void Awake() {
        currentCurveData = blankCurveData;
    }

    private void Update() {
        if (!isTransitioning && currentCurveData != null) {
            applyCurveData(1);
        }
        else {
            applyCurveData(1);
        }
    }

    public void setNormalizedUpdateTime(float normalizedTime) {
        currentNormalizedTime = normalizedTime;
    }

    public void appleOneShotCurve(ChonkerCameraAnimationCurveData curveData, float evaluationTime, float positionAmplitudeModifier, float rotationAmplituideModifer) {
        cameraShakeOneShotContainer.playOneShot(curveData, evaluationTime, positionAmplitudeModifier, rotationAmplituideModifer);
    }

    public void playOneShotHitReaction() {
        appleOneShotCurve(oneShotHitReactionCurve, .3f, 1, 1);
    }

    private void applyCurveData(float transitionWeight) {
        CurveEvaluation curveEvaluation = currentCurveData.evaluateCurve(currentNormalizedTime, transitionWeight);
        Vector3 positionEval = curveEvaluation.getPositionData();
        cameraShakeParentContainer.localPosition = positionEval;

        Vector3 rotationEval = curveEvaluation.getRotationData();
        cameraShakeParentContainer.localEulerAngles = rotationEval;
    }

    private void applyLerpedCurveData(CurveEvaluation lerpedCurve) {
        Vector3 positionEval = lerpedCurve.getPositionData();
        cameraShakeParentContainer.localPosition = positionEval;

        Vector3 rotationEval = lerpedCurve.getRotationData();
        rotationEval.x *= rotationEval.x;
        rotationEval.y *= rotationEval.y;
        cameraShakeParentContainer.localEulerAngles = rotationEval;
    }


    public void updateCurve(ChonkerCameraAnimationCurveData newCurve, float transitionTime = .5f) {
        StopAllCoroutines();
        StartCoroutine(updateCurveCoroutine(newCurve, transitionTime));
    }

    private IEnumerator updateCurveCoroutine(ChonkerCameraAnimationCurveData newCurve, float transitionTime) {
        isTransitioning = true;
        float timer = 0;
        if (!newCurve) {
            newCurve = blankCurveData;
        }
        
        CurveEvaluation newCurveEvaluation;
        CurveEvaluation oldCurveEvaluation;

        while (timer < 1) {
            timer += Time.deltaTime / transitionTime;
            oldCurveEvaluation = currentCurveData.evaluateCurve(currentNormalizedTime, timer);
            newCurveEvaluation = newCurve.evaluateCurve(currentNormalizedTime, timer);



            applyLerpedCurveData(
                oldCurveEvaluation.lerpCurves(newCurveEvaluation,
                    timer));
            yield return null;
        }

        currentCurveData = newCurve;

        isTransitioning = false;
    }
    

#if UNITY_EDITOR

    [CustomEditor(typeof(ChonkerCameraAnimationCurveManager))]
    public class ChonkerCameraAnimationCurveManagerEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            

            serializedObject.ApplyModifiedProperties();
        }

        
    }

#endif
}