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

public class ChonkerTransformShakeAnimationCurveManager : MonoBehaviour
{
    [SerializeField] private Transform cameraShakeParentContainer;
    [SerializeField] private ChonkerTransformShakeCurvePlayer cameraShakeOneShotContainer;
    [Space]
    [SerializeField] private ChonkerTransformShakeCurveData blankCurveData;
    [SerializeField] private ChonkerTransformShakeCurveData oneShotHitReactionCurve;
    
    ChonkerTransformShakeCurveData currentCurveData;
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

    public void appleOneShotCurve(ChonkerTransformShakeCurveData curveData, float evaluationTime, float positionAmplitudeModifier, float rotationAmplituideModifer) {
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


    public void updateCurve(ChonkerTransformShakeCurveData newCurve, float transitionTime = .5f) {
        StopAllCoroutines();
        StartCoroutine(updateCurveCoroutine(newCurve, transitionTime));
    }

    private IEnumerator updateCurveCoroutine(ChonkerTransformShakeCurveData newCurve, float transitionTime) {
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

    [CustomEditor(typeof(ChonkerTransformShakeAnimationCurveManager))]
    public class ChonkerCameraAnimationCurveManagerEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            

            serializedObject.ApplyModifiedProperties();
        }

        
    }

#endif
}