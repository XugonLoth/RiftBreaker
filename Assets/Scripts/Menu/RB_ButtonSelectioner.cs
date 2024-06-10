using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Used in animation event
public class RB_ButtonSelectioner : MonoBehaviour {

    public static RB_ButtonSelectioner Instance;

    void Awake() {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    [SerializeField] List<Button> _mainButtons = new List<Button>();
    [SerializeField] List<Button> _quitButtons = new List<Button>();
    [SerializeField] List<Button> _optionsButtons = new List<Button>();

    public enum BUTTON_TYPE {Main, Quit, Options};

    public void SelectMainButton(int ID) { _mainButtons[ID].Select(); Debug.Log(_mainButtons[ID]); }
    public void SelectQuitButton(int ID) { _quitButtons[ID].Select(); }
    public void SelectOptionsButton(int ID) { _optionsButtons[ID].Select(); }
}
