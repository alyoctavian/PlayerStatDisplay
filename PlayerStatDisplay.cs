using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerStatDisplay : MonoBehaviour
{
    protected const float DefaultTweenInterval = 0.5f;

    [Header("--- UI Variables ---")]
    [SerializeField]
    private RectMask2D m_rectMask = null;
    [SerializeField]
    private Image m_outerImage = null;
    [SerializeField]
    private Image m_innerImage = null;
    [SerializeField]
    private TextMeshProUGUI m_valueText = null;

    [SerializeField]
    private Color m_defaultColor = default;
    [SerializeField]
    private Color m_pulsateColor = default;

    [Header("--- Display Values ---")]
    private int m_currentValue = 0;
    private int m_maxValue = 0;
    private float m_maskPadding = 0;

    [Header("--- Tween Variables ---")]
    private float m_tweenDuration = 0f;
    private Tween m_maskTween = null;
    private Tween m_stringTween = null;
    private Sequence m_pulsateSequence = null;

    [Header("--- Sequence Variables ---")]
    [SerializeField]
    private float m_pulsateScale = 1.25f;
    [SerializeField]
    private float m_pulsateTime = 0.75f;
    [SerializeField]
    private int m_pulsateLoops = 3;

    public float MaskPadding 
    { 
        get => m_maskPadding; 
        
        set
        {
            m_maskPadding = value;

            Vector4 padding = Vector4.zero;
            padding.w = m_maskPadding;

            m_rectMask.padding = padding;
        } 
    }

    private void Start()
    {
        // Minimum padding value is the sprite top border
        MaskPadding = TopPaddingDefault(m_innerImage);

        // Assign the default string color
        m_defaultColor = m_valueText.color;
    }

    /// <summary>
    /// Calculates and returns the default top padding of an Image component based on the sprite's border and the image's height.
    /// </summary>
    /// <param name="image">The Image component to calculate the top padding for.</param>
    /// <returns>The calculated top padding value.</returns>
    private float TopPaddingDefault(Image image)
    {
        // Rule of three calculation between sprite size and image size
        return (image.sprite.border.w * image.rectTransform.rect.height) / image.sprite.rect.height;
    }

    /// <summary>
    /// Calculates and returns the default bottom padding of an Image component based on the sprite's border and the image's height.
    /// </summary>
    /// <param name="image">The Image component to calculate the bottom padding for.</param>
    /// <returns>The calculated bottom padding value.</returns>
    private float BottomPaddingDefault(Image image)
    {
        // Rule of three calculation between sprite size and image size
        return (image.sprite.border.y * image.rectTransform.rect.height) / image.sprite.rect.height;
    }

    /// <summary>
    /// Calculates and returns the default size of the Image component, excluding the top and bottom padding.
    /// </summary>
    /// <param name="image">The Image component to calculate the default size for.</param>
    /// <returns>The calculated default size value.</returns>
    private float ImageSizeDefault(Image image)
    {
        return image.rectTransform.rect.height - TopPaddingDefault(image) - BottomPaddingDefault(image);
    }

    /// <summary>
    /// Initializes the start values without animating 
    /// </summary>
    public void InitValues(int newValue, int maxValue)
    {
        m_currentValue = newValue;

        m_maxValue = maxValue;

        MaskPadding = GetTargetPadding(m_currentValue);
    }

    /// <summary>
    /// Updates the displayed values and manages smooth transitions.
    /// </summary>
    /// <param name="newVal">The new value to display.</param>
    /// <param name="newMax">The new maximum value.</param>
    public void SetValues(int newVal, int newMax)
    {
        m_maxValue = newMax;

        m_tweenDuration = (Mathf.Abs(newVal - m_currentValue) + 1) * DefaultTweenInterval;

        if (newVal != m_currentValue)
        {
            SetDisplayText(newVal);
        }

        SetImageMask(newVal);
    }

    /// <summary>
    /// Manages the update of the display text during value transitions.
    /// </summary>
    /// <param name="newVal">The new value to display.</param>
    private void SetDisplayText(int newVal)
    {
        if (m_stringTween != null)
        {
            m_stringTween.Kill();

            m_stringTween = null;
        }

        m_stringTween = SmoothStringTween(newVal);
    }

    /// <summary>
    /// Creates a smooth tween animation for updating the displayed text.
    /// </summary>
    /// <param name="newVal">The new value to display.</param>
    /// <returns>The created Tween animation.</returns>
    private Tween SmoothStringTween(int newVal)
    {
        Tween stringTween = DOTween.To(() => m_currentValue, x => m_currentValue = x, newVal, m_tweenDuration)
                                    .SetEase(Ease.OutQuad)
                                    .OnUpdate(() =>
                                    {
                                        m_valueText.text = m_currentValue.ToString();
                                    });

        return stringTween;
    }

    /// <summary>
    /// Manages the update of the image mask during value transitions.
    /// </summary>
    /// <param name="newVal">The new value to display.</param>
    private void SetImageMask(int newVal)
    {
        if (m_maskTween != null)
        {
            m_maskTween.Kill();

            m_maskTween = null;
        }

        m_maskTween = SmoothMaskTween(newVal);
    }

    /// <summary>
    /// Creates a smooth tween animation for updating the image mask.
    /// </summary>
    /// <param name="newVal">The new value to display.</param>
    /// <returns>The created Tween animation.</returns>
    private Tween SmoothMaskTween(int newVal)
    {
        float target = GetTargetPadding(newVal);

        Tween maskTween = DOTween.To(() => MaskPadding, x => MaskPadding = x, target, m_tweenDuration)
                                    .SetEase(Ease.OutQuad)
                                    .OnComplete(() => PulsateText(newVal));

        return maskTween;
    }

    /// <summary>
    /// Calculates the target padding for the image mask based on the new value and maximum value.
    /// </summary>
    /// <param name="newVal">The new value.</param>
    /// <returns>The target padding value.</returns>
    private float GetTargetPadding(int newVal)
    {
        float percentage = 1 - (float)newVal / m_maxValue;

        float target = TopPaddingDefault(m_innerImage) + percentage * ImageSizeDefault(m_innerImage);

        return target;
    }

    /// <summary>
    /// Handles pulsating the text when the value percentage is low.
    /// </summary>
    /// <param name="newVal">The current value.</param>
    private void PulsateText(int newVal)
    {
        // If value percentage higer than 25% do not pulsate
        if ((float)newVal / m_maxValue > 0.25f)
        {
            if (m_pulsateSequence.IsActive())
            {
                m_pulsateSequence.Kill(false);
            }

            return;
        }

        if (m_pulsateSequence.IsActive())
        {
            return;
        }

        if (m_pulsateSequence != null)
        {
            m_pulsateSequence.Kill();

            m_pulsateSequence = null;
        }

        m_pulsateSequence = PulsateSequence();
    }

    /// <summary>
    /// Defines a pulsating animation sequence for the text.
    /// </summary>
    /// <returns>The created pulsating animation sequence.</returns>
    private Sequence PulsateSequence()
    {
        Sequence heartSequence = DOTween.Sequence();

        heartSequence.Append(m_valueText.transform.DOScale(new Vector3(m_pulsateScale, m_pulsateScale, 1), m_pulsateTime).SetEase(Ease.OutQuad))
        .Append(m_valueText.transform.DOScale(Vector3.one, m_pulsateTime).SetEase(Ease.OutQuad))
        .SetLoops(m_pulsateLoops, LoopType.Restart)
        .OnKill(() =>
        {
            m_valueText.transform.DOScale(Vector3.one, m_pulsateTime * 2).SetEase(Ease.OutQuad);
        });

        return heartSequence;
    }
}
