using System;
using System.Collections;
using Chonker.FPS_Controller.Camera.Animation_Curve;
using UnityEngine;

public class ChonkerTransformShakeCurvePlayer : MonoBehaviour
{
    [SerializeField] private ChonkerTransformShakeCurveData sampleCurveData;
    [SerializeField] private bool testCurveData;

    private void Update() {
        if (testCurveData) {
            testCurveData = false;
            playOneShot(sampleCurveData, .3f, 1, 1);
        }
    }

    public void playOneShot(ChonkerTransformShakeCurveData curveData, float evaluationTime,float positionAmplitudeModifier = 1f, float rotationAmplitudeModifier = 1f) {
        StartCoroutine(
            playOneShotCurve(curveData, evaluationTime, positionAmplitudeModifier, rotationAmplitudeModifier));
    }
    
    private IEnumerator playOneShotCurve(ChonkerTransformShakeCurveData curveData, float evaluationTime, float positionAmplitudeModifier = 1f, float rotationAmplitudeModifier = 1f) {
        CurveEvaluation curveEvaluation;
        float timer = 0;
        while (timer < 1) {
            timer += Time.deltaTime / evaluationTime;
            curveEvaluation = curveData.evaluateCurve(timer, 1);
            transform.localPosition = curveEvaluation.getPositionData() * positionAmplitudeModifier;
            transform.localEulerAngles = curveEvaluation.getRotationData() * rotationAmplitudeModifier;
            yield return null;
        }
        
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;


    }
}
