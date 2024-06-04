using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OptionsMenuScript : MonoBehaviour
{
    #region GAME
    //GAME
    public string Username;
    public TMP_InputField UsernameInputBox;

    #endregion

    #region GRAPHICS
    public TMP_Text ResolutionText;
    public TMP_InputField LimitFramesInputBox;
    public int FrameLimit = 60;
    public Slider QualitySlider;
    public Toggle FullscreenToggle;
    public Toggle VsyncToggle;
    public bool Vsync = true;
    public int CurrentResIndex;
    public Resolution[] resolutions;
    public bool fullscreen = true;
    public int CurrentQuality;
    #endregion

    #region AUDIO
    public AudioMixer MainMixer;
    public AudioMixer MusicMixer;
    public AudioMixer SoundsMixer;
    public AudioMixer VoicesMixer;
    public Slider MainSlider;
    public Slider MusicSlider;
    public Slider SoundsSlider;
    public Slider VoicesSlider;
    public float MainVolume, MusicVolume, SoundsVolume, VoicesVolume;
    #endregion

    #region CONTROLS
    public GameObject KeyAssignedWarning;
    public Slider MouseSensivitySlider;
    public float MouseSensivity;

    public KeyCode Forward = KeyCode.W;
    public TMP_Text ForwardText;
    public KeyCode Back = KeyCode.S;
    public TMP_Text BackText;
    public KeyCode Left = KeyCode.A;
    public TMP_Text LeftText;
    public KeyCode Right = KeyCode.D;
    public TMP_Text RightText;
    public KeyCode Sprint = KeyCode.LeftShift;
    public TMP_Text SprintText;
    public KeyCode Sneak = KeyCode.LeftControl;
    public TMP_Text SneakText;
    public KeyCode Action1 = KeyCode.Mouse0;
    public TMP_Text Action1Text;
    public KeyCode Action2 = KeyCode.Mouse1;
    public TMP_Text Action2Text;
    public KeyCode Interact = KeyCode.F;
    public TMP_Text InteractText;
    public KeyCode Inventory = KeyCode.I;
    public TMP_Text InventoryText;
    public KeyCode Map = KeyCode.M;
    public TMP_Text MapText;
    public KeyCode Climb = KeyCode.Space;
    public TMP_Text ClimbText;
    public KeyCode Ability1 = KeyCode.Q;
    public TMP_Text Ability1Text;
    public KeyCode Ability2 = KeyCode.E;
    public TMP_Text Ability2Text;
    public KeyCode Ability3 = KeyCode.R;
    public TMP_Text Ability3Text;
    public KeyCode Item1 = KeyCode.Alpha1;
    public TMP_Text Item1Text;
    public KeyCode Item2 = KeyCode.Alpha2;
    public TMP_Text Item2Text;
    public KeyCode Item3 = KeyCode.Alpha3;
    public TMP_Text Item3Text;
    public KeyCode ActualBind;
    public KeyCode ActualKey;
    public string ActualBindName;

    public bool bind = false;

    #endregion

    public void Start()
    {

        resolutions = Screen.resolutions;
        ResDisplay();
        if (SceneManager.GetActiveScene().name != "MainMenu") { UsernameInputBox.interactable = false; }
    }

    public void OnApplicationQuit()
    {
        SavePrefs();
    }

    IEnumerator WarningOff(float delayTime)
    {
        if (Input.anyKey) { KeyAssignedWarning.SetActive(false); }
        yield return new WaitForSeconds(delayTime);
    }

    IEnumerator BindChangeCoroutine(int bindID)
    {
        while (true)
        {
            if (Input.anyKey) { ActualKey = (KeyCode)FindKey(); ChangeBinding(bindID); }
            yield return new WaitForSeconds(.1f);
        }
    }

    public void ChangeBind(int bindNumber)
    {
        switch (bindNumber)
        {
            case 0: Forward = ActualBind; ForwardText.text = ActualBind.ToString(); break;
            case 1: Back = ActualBind; BackText.text = ActualBind.ToString(); break;
            case 2: Left = ActualBind; LeftText.text = ActualBind.ToString(); break;
            case 3: Right = ActualBind; RightText.text = ActualBind.ToString(); break;
            case 4: Sprint = ActualBind; SprintText.text = ActualBind.ToString(); break;
            case 5: Sneak = ActualBind; SneakText.text = ActualBind.ToString(); break;
            case 6: Action1 = ActualBind; Action1Text.text = ActualBind.ToString(); break;
            case 7: Action2 = ActualBind; Action2Text.text = ActualBind.ToString(); break;
            case 8: Interact = ActualBind; InteractText.text = ActualBind.ToString(); break;
            case 9: Inventory = ActualBind; InventoryText.text = ActualBind.ToString(); break;
            case 10: Map = ActualBind; MapText.text = ActualBind.ToString(); break;
            case 11: Climb = ActualBind; ClimbText.text = ActualBind.ToString(); break;
            case 12: Ability1 = ActualBind; Ability1Text.text = ActualBind.ToString(); break;
            case 13: Ability2 = ActualBind; Ability2Text.text = ActualBind.ToString(); break;
            case 14: Ability3 = ActualBind; Ability3Text.text = ActualBind.ToString(); break;
            case 15: Item1 = ActualBind; Item1Text.text = ActualBind.ToString(); break;
            case 16: Item2 = ActualBind; Item2Text.text = ActualBind.ToString(); break;
            case 17: Item3 = ActualBind; Item3Text.text = ActualBind.ToString(); break;
        }
    }

    public void GetBind(int bindNumber)
    {
        switch (bindNumber)
        {
            case 0: ActualKey = Forward; ActualBindName = "Forward"; break;
            case 1: ActualKey = Back; ActualBindName = "Back"; break;
            case 2: ActualKey = Left; ActualBindName = "Left"; break;
            case 3: ActualKey = Right; ActualBindName = "Right"; break;
            case 4: ActualKey = Sprint; ActualBindName = "Sprint"; break;
            case 5: ActualKey = Sneak; ActualBindName = "Sneak"; break;
            case 6: ActualKey = Action1; ActualBindName = "Action1"; break;
            case 7: ActualKey = Action2; ActualBindName = "Action2"; break;
            case 8: ActualKey = Interact; ActualBindName = "Interact"; break;
            case 9: ActualKey = Inventory; ActualBindName = "Inventory"; break;
            case 10: ActualKey = Map; ActualBindName = "Map"; break;
            case 11: ActualKey = Climb; ActualBindName = "Climb"; break;
            case 12: ActualKey = Ability1; ActualBindName = "Ability1"; break;
            case 13: ActualKey = Ability2; ActualBindName = "Ability2"; break;
            case 14: ActualKey = Ability3; ActualBindName = "Ability3"; break;
            case 15: ActualKey = Item1; ActualBindName = "Item1"; break;
            case 16: ActualKey = Item2; ActualBindName = "Item2"; break;
            case 17: ActualKey = Item3; ActualBindName = "Item3"; break;
        }
    }

    public int FindKey()
    {
        int actualKey = 0;
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(kcode))
            {
                actualKey = (int)kcode;
            }
        }
        return (int)actualKey;
    }

    public void BindChange(int bindID)
    {
        StartCoroutine(BindChangeCoroutine(bindID));
    }

    public void ChangeBinding(int bindID)
    {
        bool isSame = false;
        ActualBind = ActualKey;
        for (int i = 0; i < 18; i++)
        {
            GetBind(i);
            if (ActualKey == ActualBind) { isSame = true; break; }
            else { isSame = false; }
        }
        if (isSame == true) { KeyAssignedWarning.SetActive(true); }
        else
        {
            GetBind(bindID);
            ActualKey = ActualBind;
            ChangeBind(bindID);
            PlayerPrefs.SetInt(ActualBindName, (int)ActualBind); Debug.Log(ActualBindName + "-" + ActualBind + "CHECK" + Forward);
        }
        StopAllCoroutines();
    }

    public void SavePrefs()
    {
        //GAME
        PlayerPrefs.SetString("Username", Username);

        //GRAPHICS
        PlayerPrefs.SetInt("Fullscreen", Convert.ToInt32(fullscreen));
        PlayerPrefs.SetInt("Resolution_width", resolutions[CurrentResIndex].width);
        PlayerPrefs.SetInt("Resolution_height", resolutions[CurrentResIndex].height);
        PlayerPrefs.SetInt("Vsync", Convert.ToInt32(Vsync));
        PlayerPrefs.SetInt("FrameLimit", FrameLimit);
        PlayerPrefs.SetInt("Quality", CurrentQuality);

        //AUDIO
        PlayerPrefs.SetFloat("Volume", MainVolume);
        PlayerPrefs.SetFloat("Volume", MusicVolume);
        PlayerPrefs.SetFloat("Volume", SoundsVolume);
        PlayerPrefs.SetFloat("Volume", VoicesVolume);

        //CONTROLS
        PlayerPrefs.SetFloat("MouseSensivity", MouseSensivity);
        PlayerPrefs.SetInt("Forward", (int)Forward);
        PlayerPrefs.SetInt("Back", (int)Back);
        PlayerPrefs.SetInt("Left", (int)Left);
        PlayerPrefs.SetInt("Right", (int)(Right));
        PlayerPrefs.SetInt("Sprint", (int)(Sprint));
        PlayerPrefs.SetInt("Sneak", (int)(Sneak));
        PlayerPrefs.SetInt("Action1", (int)(Action1));
        PlayerPrefs.SetInt("Action2", (int)(Action2));
        PlayerPrefs.SetInt("Inventory", (int)(Inventory));
        PlayerPrefs.SetInt("Map", (int)(Map));
        PlayerPrefs.SetInt("Climb", (int)(Climb));
        PlayerPrefs.SetInt("Ability1", (int)(Ability1));
        PlayerPrefs.SetInt("Ability2", (int)(Ability2));
        PlayerPrefs.SetInt("Ability3", (int)(Ability3));
        PlayerPrefs.SetInt("Item1", (int)(Item1));
        PlayerPrefs.SetInt("Item2", (int)(Item2));
        PlayerPrefs.SetInt("Item3", (int)(Item3));


        //SAVE
        PlayerPrefs.Save();

    }

    public void LoadPrefs()
    {
        //GAME
        Username = PlayerPrefs.GetString("Username", "Username");
        UsernameInputBox.text = Username;

        //GRAPHICS
        fullscreen = Convert.ToBoolean(PlayerPrefs.GetInt("Fullscreen", 1));
        if (fullscreen == true) { FullscreenToggle.isOn = true; }
        else { FullscreenToggle.isOn = false; }
        CurrentQuality = PlayerPrefs.GetInt("Quality", 1);
        CurrentResIndex = PlayerPrefs.GetInt("Resolution", FindHdResolution(1920, 1080));
        Vsync = Convert.ToBoolean(PlayerPrefs.GetInt("Vsync", 1));
        if (Vsync == true) { VsyncToggle.isOn = true; }
        else { VsyncToggle.isOn = false; }
        FrameLimit = PlayerPrefs.GetInt("FrameLimit", 60);
        LimitFramesInputBox.text = Convert.ToString(PlayerPrefs.GetInt("FrameLimit", 60));
        ResDisplay();

        //AUDIO
        MainMix(PlayerPrefs.GetFloat("Volume", 0));
        MainSlider.value = MainVolume;
        MusicMix(PlayerPrefs.GetFloat("Volume", 0));
        MusicSlider.value = MusicVolume;
        SoundsMix(PlayerPrefs.GetFloat("Volume", 0));
        SoundsSlider.value = SoundsVolume;
        VoicesMix(PlayerPrefs.GetFloat("Volume", 0));
        VoicesSlider.value = VoicesVolume;

        //CONTROLS
        MouseSensivity = PlayerPrefs.GetFloat("MouseSensivity", 3); MouseSensivitySlider.value = MouseSensivity;
        MouseSensivitySlider.value = MouseSensivity;
        Forward = (KeyCode)PlayerPrefs.GetInt("Forward", 119);
        Back = (KeyCode)PlayerPrefs.GetInt("Back", 115);
        Left = (KeyCode)PlayerPrefs.GetInt("Left", 97);
        Right = (KeyCode)PlayerPrefs.GetInt("Right", 100);
        Sprint = (KeyCode)PlayerPrefs.GetInt("Sprint", 304);
        Sneak = (KeyCode)PlayerPrefs.GetInt("Sneak", 306);
        Action1 = (KeyCode)PlayerPrefs.GetInt("Action1", 323);
        Action2 = (KeyCode)PlayerPrefs.GetInt("Action2", 324);
        Interact = (KeyCode)PlayerPrefs.GetInt("Interact", 102);
        Inventory = (KeyCode)PlayerPrefs.GetInt("Inventory", 105);
        Map = (KeyCode)PlayerPrefs.GetInt("Map", 109);
        Climb = (KeyCode)PlayerPrefs.GetInt("Climb", 32);
        Ability1 = (KeyCode)PlayerPrefs.GetInt("Ability1", 113);
        Ability2 = (KeyCode)PlayerPrefs.GetInt("Ability2", 101);
        Ability3 = (KeyCode)PlayerPrefs.GetInt("Ability3", 114);
        Item1 = (KeyCode)PlayerPrefs.GetInt("Item1", 49);
        Item2 = (KeyCode)PlayerPrefs.GetInt("Item2", 50);
        Item3 = (KeyCode)PlayerPrefs.GetInt("Item3", 51);
        //ForwardText.text = Forward.ToString();
        //BackText.text = Back.ToString();
        //LeftText.text = Left.ToString();
        //RightText.text = Right.ToString();
        //SprintText.text = Sprint.ToString();
        //SneakText.text = Sneak.ToString();
        //Action1Text.text = Action1.ToString();
        //Action2Text.text = Action2.ToString();
        //InteractText.text = Interact.ToString();
        //InventoryText.text = Inventory.ToString();
        //MapText.text = Map.ToString();
        //ClimbText.text = Climb.ToString();
        //Ability1Text.text = Ability1.ToString();
        //Ability2Text.text = Ability2.ToString();
        //Ability3Text.text = Ability3.ToString();
        //Item1Text.text = Item1.ToString();
        //Item2Text.text = Item2.ToString();
        //Item3Text.text = Item3.ToString();

        //SAVE
        Apply();
    }

    public void UsernameInput(string UsernameInp)
    {
        Username = UsernameInp;
    }

    public void ResetOptions()
    {
        CurrentResIndex = FindHdResolution(1920, 1080);
        fullscreen = true;
        CurrentQuality = 1;
        Vsync = true;
        FrameLimit = 60;
        ResDisplay();
        Apply();
        QualitySlider.value = 1f;
        FullscreenToggle.isOn = true;
        MainMix(0);
        MusicMix(0);
        SoundsMix(0);
        VoicesMix(0);
        MainSlider.value = 0f;
        MusicSlider.value = 0f;
        SoundsSlider.value = 0f;
        VoicesSlider.value = 0f;
        MouseSensivity = 3;
        MouseSensivitySlider.value = MouseSensivity;
        Forward = KeyCode.W;
        Back = KeyCode.S;
        Left = KeyCode.A;
        Right = KeyCode.D;
        Sprint = KeyCode.LeftShift;
        Sneak = KeyCode.LeftControl;
        Action1 = KeyCode.Mouse0;
        Action2 = KeyCode.Mouse1;
        Interact = KeyCode.F;
        Inventory = KeyCode.I;
        Map = KeyCode.M;
        Climb = KeyCode.Space;
        Ability1 = KeyCode.Q;
        Ability2 = KeyCode.E;
        Ability3 = KeyCode.R;
        Item1 = KeyCode.Alpha1;
        Item2 = KeyCode.Alpha2;
        Item3 = KeyCode.Alpha3;
    }

    public int FindHdResolution(int ResWidth, int ResHeight)
    {
        int ResIndex = 0;
        for (int i = 0; i < resolutions.Length - 1; i++)
        {
            if (resolutions[i].width == ResWidth && resolutions[i].height == ResHeight) { break; }
            ResIndex++;
        }
        Debug.Log(ResIndex);
        return ResIndex;
    }

    public void Fullscreen_change(bool fscr)
    {
        fullscreen = fscr;
    }

    public void VsyncChange(bool Vsync)
    {
        if (Vsync == false) { QualitySettings.vSyncCount = 0; }
        else { QualitySettings.vSyncCount = 1; }
    }

    public void ResDisplay()
    {
        ResolutionText.text = resolutions[CurrentResIndex].width.ToString() + "x" + resolutions[CurrentResIndex].height.ToString();
    }

    public void LimitFramesInput(string limitFrames)
    {
        int LimitFrames = int.Parse(limitFrames);
        if (LimitFrames < 1) { LimitFrames = 1; }
        FrameLimit = LimitFrames;
        Application.targetFrameRate = LimitFrames;
    }

    public void ResFWD()
    {
        CurrentResIndex = CurrentResIndex + 1;
        if (CurrentResIndex > (resolutions.Length - 1)) { CurrentResIndex = (resolutions.Length - 1); }
        ResDisplay();
    }

    public void ResBACK()
    {
        CurrentResIndex = CurrentResIndex - 1;
        if (CurrentResIndex < 0) { CurrentResIndex = 0; }
        ResDisplay();

    }

    public void SetQuality(float quality)
    {
        CurrentQuality = (int)quality;
    }

    public void Apply()
    {
        if (fullscreen == true)
        {
            Screen.SetResolution(resolutions[CurrentResIndex].width, resolutions[CurrentResIndex].height, true);
        }
        else
        {
            Screen.SetResolution(resolutions[CurrentResIndex].width, resolutions[CurrentResIndex].height, false);
        }
        QualitySettings.SetQualityLevel(CurrentQuality, true);
        VsyncChange(Vsync);
        LimitFramesInput(LimitFramesInputBox.text);

    }

    public void MainMix(float volume)
    {
        MainMixer.SetFloat("Volume", volume);
        MainVolume = volume;
    }

    public void MusicMix(float volume)
    {
        MusicMixer.SetFloat("Volume", volume);
        MusicVolume = volume;
    }

    public void SoundsMix(float volume)
    {
        SoundsMixer.SetFloat("Volume", volume);
        SoundsVolume = volume;
    }

    public void VoicesMix(float volume)
    {
        VoicesMixer.SetFloat("Volume", volume);
        VoicesVolume = volume;
    }

    public void MouseSensivityChange(float sensivity)
    {
        MouseSensivity = sensivity;
    }

}