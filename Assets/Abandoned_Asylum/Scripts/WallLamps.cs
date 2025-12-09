using UnityEngine;

public class WallLamps : MonoBehaviour, IInteractable
{
    [ColorUsage(true, true)] // This attribute adds the HDR intensity slider to the Inspector
    public Color emissionColor;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    [SerializeField] private LightPuzzle lightPuzzle;
    [SerializeField] private char value;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        // 1. Get the current properties
        _renderer.GetPropertyBlock(_propBlock);

        // 2. Set the Emission Color
        // Note: "_EmissionColor" is standard, but verify your shader property name!
        _propBlock.SetColor("_EmissionColor", emissionColor);

        // 3. Apply the changes
        _renderer.SetPropertyBlock(_propBlock);
    }

    // Optional: Call this if you need to update lights in real-time
    public void SetColor(Color emissionColor)
    {
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_EmissionColor", emissionColor);
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void Interact()
    {
        DefaultColor();
        lightPuzzle.AddValue(value);
    }

    public void DefaultColor()
    {
        SetColor(Color.white);
    }

    public void ResetColor()
    {
        SetColor(emissionColor);
    }
}