using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverSimple : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{

    public AudioClip audioHover;
    public AudioClip audioSeleted;
    AudioSource audioSource;
    public void OnPointerEnter(PointerEventData eventData)
    {
            audioSource.clip = audioHover;
            this.audioSource.Play();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        audioSource.clip = audioSeleted;
        this.audioSource.Play();
    }
    
}
