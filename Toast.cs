using System;
using UnityEngine;
using UnityEngine.UI;
public class Toast : MonoBehaviour{
    public enum Position{top, bottom};
    public enum Theme{light, dark, dynamic};

    public static void Show(string message, float timeout = 2f, Position position = Position.bottom, Theme theme = Theme.dark){
        DateTime time = DateTime.Now, morning = DateTime.Today.AddHours(7d), evening = DateTime.Today.AddHours(19d);
        if (theme == Theme.dynamic)
            theme = time > morning && time < evening ? Theme.light : Theme.dark;  //sets theme based on system time
        GameObject toastPrefab = Resources.Load<GameObject>("Toast");   //toast prefab
        Sprite lightBackground = Resources.Load<Sprite>("LightToast");  //light background
        Sprite darkBackground = Resources.Load<Sprite>("DarkToast");    //dark background
        GameObject containerObject = toastPrefab.gameObject.transform.GetChild(0).gameObject;   //container object
        GameObject textBackground = containerObject.gameObject.transform.GetChild(0).gameObject;    //text background
        GameObject textObject = containerObject.gameObject.transform.GetChild(0).GetChild(0).gameObject;    //text
        textBackground.GetComponent<Image>().sprite = theme == Theme.light ? lightBackground : darkBackground;   //sets sprite of text background depending on the theme
        textObject.GetComponent<Text>().text = message; //sets text to the message provided
        textObject.GetComponent<Text>().color = theme == Theme.light ? Color.black : Color.white; //sets text color depending on the theme
        containerObject.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, position == Position.top ? 1f : 0f);                    //positions toast
        containerObject.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, position == Position.top ? 1f : 0f);                    //"""""""""""""""
        containerObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.5f, position == Position.top ? -100f : 100f, 0f);    //"""""""""""""""
        GameObject clone = Instantiate(toastPrefab);    //spawns toast
        Destroy(clone.gameObject, timeout); //removes toast after time specified
    }
}
