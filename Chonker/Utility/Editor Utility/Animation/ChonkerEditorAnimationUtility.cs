using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

public class ChonkerEditorAnimationUtility : MonoBehaviour
{
    #if UNITY_EDITOR
    public static List<ChildAnimatorState> searchForStates(AnimatorController controller) {
        List<ChildAnimatorState> animatorStates = new();
        AnimatorStateMachine animatorStateMachine = controller.layers[2].stateMachine;
        animatorStates.AddRange(animatorStateMachine.states);
        resursiveSearchForStates(ref animatorStates, animatorStateMachine.stateMachines);

        return animatorStates;
    }

    private static void resursiveSearchForStates(ref List<ChildAnimatorState> animatorStates,
        ChildAnimatorStateMachine[] stateMachines) {
        foreach (var childAnimatorStateMachine in stateMachines) {
            animatorStates.AddRange(childAnimatorStateMachine.stateMachine.states);
            resursiveSearchForStates(ref animatorStates, childAnimatorStateMachine.stateMachine.stateMachines);
        }
    }
    #endif
}
