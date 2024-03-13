using UnityEngine;
using UnityEngine.UI;

public class FADE : MonoBehaviour
{
    private Animator _animator = null;
    public Image img;
    public bool isPlaying;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        img = GetComponent<Image>();
    }

    public void FadeInInstant(float _speed = 1f)
    {
        _animator.speed = _speed;
        _animator.Play("FadeIn_Instant");
    }

    public void FadeOutInstant(float _speed = 1f)
    {
        _animator.speed = _speed;
        _animator.Play("FadeOut_Instant");
    }

    public void FadeIn(float _speed = 1f)
    {
        _animator.speed = _speed;
        
        _animator.Play("FadeIn");
    }

    public void FadeOut(float _speed = 1f)
    {
        _animator.speed = _speed;

        _animator.Play("FadeOut");
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }
}
