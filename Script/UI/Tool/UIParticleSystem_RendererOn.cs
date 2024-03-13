using UnityEngine;
/// <summary>
/// UIParticleSystem로 인해 ParticleSystemRenderer이 자동으로 enable false 됨
/// 특정 Popup위에 출력이 안되는 현상 있음
/// 강제로 ParticleSystemRenderer를 true 해줘 출력
/// </summary>
public class UIParticleSystem_RendererOn : MonoBehaviour { private void Start() => GetComponent<ParticleSystemRenderer>().enabled = true; }
