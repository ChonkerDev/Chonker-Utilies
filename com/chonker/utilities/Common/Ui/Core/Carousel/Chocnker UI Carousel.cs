using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChocnkerUICarousel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textObject;
    [SerializeField] private TextMeshProUGUI titleObject;
    [Space]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color highlightColor = Color.white;
    [SerializeField] private Color clickedColor = Color.white;
    [SerializeField] private string titleText;

    private List<string> options;
    private int currentIndex;

    [HideInInspector] public UnityEvent onCycleChange;

    private void Start() {
        foreach (var componentsInChild in GetComponentsInChildren<Button>()) {
            ColorBlock colorBlock = new ColorBlock() {
                normalColor = defaultColor,
                highlightedColor = highlightColor,
                pressedColor = clickedColor,
                colorMultiplier = 1,
            };
            componentsInChild.colors = colorBlock;
        }
        
        

        refreshDisplay();
    }

    public void setOption(string optionText) {
        for (var i = 0; i < options.Count; i++) {
            if (optionText == options[i]) {
                currentIndex = i;
                refreshDisplay();
                return;
            }
        }

        throw new Exception();
    }

    public string getCurrentOption() {
        return options[currentIndex];
    }

    public void initializeOptions(List<string> options) {
        this.options = options;
        refreshDisplay();
    }

    public void onLeftArrowClick() {
        currentIndex--;
        if (currentIndex < 0) {
            currentIndex = options.Count - 1;
        }

        onCycleChange.Invoke();
        refreshDisplay();
    }

    public void onRightArrowClick() {
        currentIndex++;
        if (currentIndex >= options.Count) {
            currentIndex = 0;
        }

        onCycleChange.Invoke();
        refreshDisplay();
    }

    private void refreshDisplay() {
        textObject.text = options[currentIndex];
    }

    private void OnValidate() {
        titleObject.text = titleText;
    }
}