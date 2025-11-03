using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    private Image reticleImage;

    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color hitColor = Color.red;

    void Awake()
    {
        // Get the Image component attached to the same GameObject
        reticleImage = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetReticleColor(bool isRaycastHitting)
    {
        if (isRaycastHitting)
        {
            // Raycast is hitting something
            reticleImage.color = hitColor;
        }
        else
        {
            // Raycast is not hitting anything
            reticleImage.color = defaultColor;
        }
    }
}
