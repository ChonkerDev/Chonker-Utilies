using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChonkerNoteTaker : MonoBehaviour
{
    #if UNITY_EDITOR
    [TextAreaAttribute]
    public string Notes;
    #endif
}
