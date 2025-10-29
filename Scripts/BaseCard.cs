using Godot;
using System;

public abstract partial class BaseCard : TextureRect
{
    public string Name { get; protected set; }
    public Texture2D TextureIcon { get; protected set; }

    public bool IsSelected { get; private set; }
    private bool _isDragging;
    public bool IsDragging => _isDragging;
    public bool IsDraggable { get; set; } = true;

    public delegate void CardClicked(BaseCard clickedCard);
    public event CardClicked OnCardClicked;

    public delegate void CardDrag(BaseCard card, Vector2 delta);
    public event CardDrag OnDragging;

    public delegate void CardDragEnd(BaseCard card);
    public event CardDragEnd OnDragEnded;

    private Vector2 _dragOffset;
    private float _currentYRot = 0.0f;
    private float _currentXRot = 0.0f;
    private Tween _scaleTween; // Tween for scaling animation
    private const float HOVER_SCALE = 1.05f;
    private const float SELECTED_SCALE = 1.15f;
    private const float NORMAL_SCALE = 1.0f;
    private const float TWEEN_DURATION = 0.5f;
    [Export] private TextureRect _shadow;
    

    public virtual void Initialize(string name, Texture2D texture)
    {
        Name = name;
        TextureIcon = texture;
        Texture = texture;

        // Apply ShaderMaterial
        var shaderMaterial = new ShaderMaterial();
        shaderMaterial.Shader = GD.Load<Shader>("res://Shaders/CardPerspectiveShader.gdshader");
        shaderMaterial.SetShaderParameter("fov", 90.0f);
        shaderMaterial.SetShaderParameter("y_rot", 0.0f);
        shaderMaterial.SetShaderParameter("x_rot", 0.0f);
        shaderMaterial.SetShaderParameter("cull_back", true);
        shaderMaterial.SetShaderParameter("inset", 0.0f);
        Material = shaderMaterial;

        Connect("gui_input", new Callable(this, nameof(OnCardInput)));
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        if (_shadow != null)
    {
        _shadow.PivotOffset = _shadow.Size / 2;
    }
    }

    private void OnCardInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (mouseEvent.Pressed)
                {
                    OnCardClicked?.Invoke(this);

                    if (!IsDraggable)
                    {
                        return;
                    }

                    _dragOffset = GetGlobalMousePosition() - GlobalPosition;
                    _isDragging = true;
                    HideTooltip();
                }
                else
                {
                    _isDragging = false;
                    OnDragEnded?.Invoke(this);
                    // Reset rotation when dragging ends
                    if (Material is ShaderMaterial shaderMaterial)
                    {
                        _currentYRot = 0.0f;
                        _currentXRot = 0.0f;
                        shaderMaterial.SetShaderParameter("y_rot", _currentYRot);
                        shaderMaterial.SetShaderParameter("x_rot", _currentXRot);
                    }
                }
            }
        }
        else if (@event is InputEventMouseMotion motion)
        {
            if (_isDragging)
            {
                GlobalPosition = GetGlobalMousePosition() - _dragOffset;
                OnDragging?.Invoke(this, motion.Relative);
            }
            UpdateCardRotation();
        }
    }

    private void OnMouseEntered()
    {
    if (IsDragging) return;
    if (IsSelected) return;
    UpdateCardRotation();

    if (_scaleTween != null && _scaleTween.IsValid())
        _scaleTween.Kill();

    _scaleTween = CreateTween();
    _scaleTween.SetParallel(true);

    bool hasAnimation = false;

    // Anima a carta (sempre)
    _scaleTween.TweenProperty(this, "scale", new Vector2(HOVER_SCALE, HOVER_SCALE), TWEEN_DURATION)
        .SetTrans(Tween.TransitionType.Elastic)
        .SetEase(Tween.EaseType.Out);
    hasAnimation = true;

    // Anima a sombra (se existir)
    if (_shadow != null)
    {
        _scaleTween.TweenProperty(_shadow, "scale", new Vector2(HOVER_SCALE, HOVER_SCALE), TWEEN_DURATION)
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);
        hasAnimation = true;
    }

    // Só roda se tiver pelo menos uma animação
    if (!hasAnimation)
    {
        _scaleTween.Kill();
        _scaleTween = null;
    }
}

    private void OnMouseExited()
{
    if (_isDragging) return;

    // Reset rotação
    if (Material is ShaderMaterial shaderMaterial)
    {
        _currentYRot = 0.0f;
        _currentXRot = 0.0f;
        shaderMaterial.SetShaderParameter("y_rot", _currentYRot);
        shaderMaterial.SetShaderParameter("x_rot", _currentXRot);
    }

    if (_scaleTween != null && _scaleTween.IsValid())
        _scaleTween.Kill();

    _scaleTween = CreateTween();
    _scaleTween.SetParallel(true);
    float targetScale = IsSelected ? SELECTED_SCALE : NORMAL_SCALE;

    bool hasAnimation = false;

    // Anima a carta (sempre)
    _scaleTween.TweenProperty(this, "scale", new Vector2(targetScale, targetScale), TWEEN_DURATION)
        .SetTrans(Tween.TransitionType.Elastic)
        .SetEase(Tween.EaseType.Out);
    hasAnimation = true;

    // Anima a sombra (se existir)
    if (_shadow != null)
    {
        _scaleTween.TweenProperty(_shadow, "scale", new Vector2(targetScale, targetScale), TWEEN_DURATION)
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);
        hasAnimation = true;
    }

    if (!hasAnimation)
    {
        _scaleTween.Kill();
        _scaleTween = null;
    }
}

    private void UpdateCardRotation()
    {
        if (Material is ShaderMaterial shaderMaterial)
        {
            Vector2 mousePos = GetLocalMousePosition();
            Vector2 cardSize = Size;

            float normalizedX = (mousePos.X / cardSize.X) * 2.0f - 1.0f;
            float normalizedY = (mousePos.Y / cardSize.Y) * 2.0f - 1.0f;

            float maxYRot = 30.0f;
            float maxXRot = 20.0f;

            float targetYRot = normalizedX * maxYRot;
            float targetXRot = -normalizedY * maxXRot;

            float lerpSpeed = 0.1f;
            _currentYRot = Mathf.Lerp(_currentYRot, targetYRot, lerpSpeed);
            _currentXRot = Mathf.Lerp(_currentXRot, targetXRot, lerpSpeed);

            shaderMaterial.SetShaderParameter("y_rot", _currentYRot);
            shaderMaterial.SetShaderParameter("x_rot", _currentXRot);
        }
    }

    public void ToggleSelection()
{
    IsSelected = !IsSelected;

    // Interrompe qualquer animação (hover ou anterior)
    if (_scaleTween != null && _scaleTween.IsValid())
        _scaleTween.Kill();

    _scaleTween = CreateTween();
    _scaleTween.SetParallel(true);

    float targetScale = IsSelected ? SELECTED_SCALE : NORMAL_SCALE;

    // Anima a carta
    _scaleTween.TweenProperty(this, "scale", new Vector2(targetScale, targetScale), TWEEN_DURATION)
        .SetTrans(Tween.TransitionType.Elastic)
        .SetEase(Tween.EaseType.Out);

    // Anima a sombra
    if (_shadow != null)
    {
        _scaleTween.TweenProperty(_shadow, "scale", new Vector2(targetScale, targetScale), TWEEN_DURATION)
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);
    }

    // Modulação de cor
    Modulate = IsSelected ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
}
    protected virtual void HideTooltip()
{
    // será sobrescrita nas subclasses
}
}