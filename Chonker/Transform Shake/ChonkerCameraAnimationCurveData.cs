using UnityEditor;
using UnityEngine;

namespace Chonker.FPS_Controller.Camera.Animation_Curve
{
    public class ChonkerCameraAnimationCurveData : ScriptableObject
    {
#if UNITY_EDITOR
        [MenuItem("Chonker/Create Scriptable Object/Animation/Animation Curve Data")]
        private static void createAnimationCurveData() {
            ChonkerEditorAssetUtility.createScriptableObject<ChonkerCameraAnimationCurveData>();
        }
#endif
        private const float basePositionAmplitude = .1f;
        private const float baseRotationAmplitude = 30;
        public AnimationCurve xPosCurve = generateDefaultCurve();
        [Range(0, 1)] public float xPosAmplitudeMultiplier = 1;
        public AnimationCurve yPosCurve = generateDefaultCurve();
        [Range(0, 1)] public float yPosAmplitudeMultiplier = 1;
        public AnimationCurve xRotCurve = generateDefaultCurve();
        [Range(0, 1)] public float xRotAmplitudeMultiplier = 1;
        public AnimationCurve yRotCurve = generateDefaultCurve();
        [Range(0, 1)] public float yRotAmplitudeMultiplier = 1;


        public CurveEvaluation evaluateCurve(float normalizedTime, float weight) {
            return new CurveEvaluation(
                xPos: Mathf.Lerp(0, xPosCurve.Evaluate(normalizedTime) * xPosAmplitudeMultiplier * basePositionAmplitude, weight),
                yPos: Mathf.Lerp(0, yPosCurve.Evaluate(normalizedTime) * yPosAmplitudeMultiplier * basePositionAmplitude, weight),
                xRot: Mathf.Lerp(0, -xRotCurve.Evaluate(normalizedTime) * xRotAmplitudeMultiplier * baseRotationAmplitude, weight),
                yRot: Mathf.Lerp(0, yRotCurve.Evaluate(normalizedTime) * yRotAmplitudeMultiplier * baseRotationAmplitude, weight));
        }

        private static AnimationCurve generateDefaultCurve() {
            AnimationCurve ac = AnimationCurve.Linear(0, 0, 1, 0);
            ac.postWrapMode = WrapMode.Loop;
            return ac;
        }

        public static CurveEvaluation lerpCurves(
            CurveEvaluation curve0,
            CurveEvaluation curve1, float alpha) {
            return curve0.lerpCurves(curve1, alpha);
        }
    }

    public struct CurveEvaluation
    {
        private Vector3 localPos;
        private Vector3 localRot;

        public CurveEvaluation(float xPos, float yPos, float xRot, float yRot) {
            localPos = new Vector3(xPos, yPos);
            localRot = new Vector3(xRot, yRot);
        }

        public Vector3 getPositionData() {
            return localPos;
        }

        public Vector3 getRotationData() {
            return localRot;
        }

        public CurveEvaluation lerpCurves(CurveEvaluation newCurve, float alpha) {
            Vector3 newPos = Vector3.Lerp(localPos, newCurve.getPositionData(), alpha);
            Vector3 newRot = Vector3.Lerp(localRot, newCurve.getRotationData(), alpha);

            return new CurveEvaluation(newPos.x, newPos.y, newRot.x, newRot.y);
        }
    }
}