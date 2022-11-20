using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class mouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Text btnText;
    public Image underLine;
    public AudioClip audioHover;
    public AudioClip audioSeleted;
    AudioSource audioSource;
    bool mouseEnter;
    float m;
    public void OnPointerEnter(PointerEventData eventData)
    {
            audioSource.clip = audioHover;
            this.audioSource.Play();
            mouseEnter = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        audioSource.clip = audioSeleted;
        this.audioSource.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
            mouseEnter = false;
    }

    void Awake() {
        this.audioSource = GetComponent<AudioSource>();
    }
    
    void Update() {
        if(mouseEnter)
        {
            if(btnText.color.a > 0.9f)
                m = -0.7f;
            else if(btnText.color.a < 0.1f)
                m = 0.7f;
            btnText.color = new Color(btnText.color.r, btnText.color.g, btnText.color.b, btnText.color.a + Time.deltaTime / m);
            underLine.color = new Color(btnText.color.r, btnText.color.g, btnText.color.b, btnText.color.a + Time.deltaTime / m);
        }
        else
        {
            btnText.color = new Color(btnText.color.r, btnText.color.g, btnText.color.b, 1.0f);
            underLine.color = new Color(btnText.color.r, btnText.color.g, btnText.color.b, 1.0f);
        }
    }
}
