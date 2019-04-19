using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoseHUD : MonoBehaviour
{


    public Sprite[] images;
    Image m_Image;

    void Start()
    {
        //Fetch the Image from the GameObject
        m_Image = GetComponent<Image>();
    }

    public void changeSprite(int meters, int total)
    {
        float percentage = ((float)meters / total) * 100.0f;
        //Debug.Log("Percentage: " + percentage);
        if(percentage > 99)
        {
            m_Image.sprite = images[0];
            return;
        }
        if (percentage >= 96)
        {
            m_Image.sprite = images[1];
            return;
        }
        if (percentage >= 91)
        {
            m_Image.sprite = images[2];
            return;
        }
        if (percentage >= 86)
        {
            m_Image.sprite = images[3];
            return;
        }
        if (percentage >= 81)
        {
            m_Image.sprite = images[4];
            return;
        }
        if (percentage >= 76)
        {
            m_Image.sprite = images[5];
            return;
        }
        if (percentage >= 71)
        {
            m_Image.sprite = images[6];
            return;
        }
        if (percentage >= 66)
        {
            m_Image.sprite = images[7];
            return;
        }
        if (percentage >= 61)
        {
            m_Image.sprite = images[8];
            return;
        }
        if (percentage >= 56)
        {
            m_Image.sprite = images[9];
            return;
        }
        if (percentage >= 51)
        {
            m_Image.sprite = images[10];
            return;
        }
        if (percentage >= 46)
        {
            m_Image.sprite = images[11];
            return;
        }
        if (percentage >= 41)
        {
            m_Image.sprite = images[12];
            return;
        }
        if (percentage >= 36)
        {
            m_Image.sprite = images[13];
            return;
        }
        if (percentage >= 31)
        {
            m_Image.sprite = images[14];
            return;
        }
        if (percentage >= 26)
        {
            m_Image.sprite = images[15];
            return;
        }
        if (percentage >= 21)
        {
            m_Image.sprite = images[16];
            return;
        }
        if (percentage >= 16)
        {
            m_Image.sprite = images[17];
            return;
        }
        if (percentage >= 11)
        {
            m_Image.sprite = images[18];
            return;
        }
        if (percentage >= 6)
        {
            m_Image.sprite = images[19];
            return;
        }
        if (percentage >= 1)
        {
            m_Image.sprite = images[20];
            return;
        }
        if (percentage >= 0)
        {
            m_Image.sprite = images[21];
            return;
        }
    }
}
