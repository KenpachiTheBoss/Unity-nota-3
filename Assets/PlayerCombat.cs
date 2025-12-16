using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;          // Animator del jugador
    public GameObject arma;            // Mazo en la mano
    public Collider armaCollider;      // Collider del mazo (IsTrigger)

    [Header("Ataque ligero (clic izquierdo)")]
    public float tiempoColliderLigero = 0.3f;
    public float cooldownLigero = 0.4f;

    [Header("Ataque fuerte (tecla F)")]
    public float tiempoColliderFuerte = 0.5f;
    public float cooldownFuerte = 0.8f;
    public KeyCode teclaAtaqueFuerte = KeyCode.F;

    [Header("Equipar / guardar arma")]
    public KeyCode teclaEquipar = KeyCode.R;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip sonidoSwingLigero;
    public AudioClip sonidoSwingFuerte;
    public AudioClip sonidoEquipar;

    // Estados internos
    bool armaEquipada = false;
    bool puedeAtacar = true;
    Coroutine ataqueActual;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Comenzar con el arma guardada
        EquiparArma(false);

        if (armaCollider != null)
            armaCollider.enabled = false;
    }

    void Update()
    {
        // Sacar / guardar arma
        if (Input.GetKeyDown(teclaEquipar))
        {
            armaEquipada = !armaEquipada;
            EquiparArma(armaEquipada);
        }

        // No se puede atacar sin arma equipada o en cooldown
        if (!armaEquipada || !puedeAtacar)
            return;

        // Ataque ligero (clic izquierdo)
        if (Input.GetMouseButtonDown(0))
        {
            if (ataqueActual != null) StopCoroutine(ataqueActual);
            ataqueActual = StartCoroutine(RealizarAtaque(
                triggerAnimator: "Attack",              // nombre del Trigger en el Animator
                tiempoCollider: tiempoColliderLigero,
                cooldown: cooldownLigero,
                clip: sonidoSwingLigero
            ));
        }

        // Ataque fuerte (tecla F)
        if (Input.GetKeyDown(teclaAtaqueFuerte))
        {
            if (ataqueActual != null) StopCoroutine(ataqueActual);
            ataqueActual = StartCoroutine(RealizarAtaque(
                triggerAnimator: "AttackFuerte",        // nombre del Trigger en el Animator
                tiempoCollider: tiempoColliderFuerte,
                cooldown: cooldownFuerte,
                clip: sonidoSwingFuerte
            ));
        }
    }

    // Mostrar / ocultar arma y avisar al Animator
    void EquiparArma(bool equipada)
    {
        if (arma != null)
            arma.SetActive(equipada);

        if (animator != null)
            animator.SetBool("IsArmed", equipada);

        if (equipada && audioSource != null && sonidoEquipar != null)
            audioSource.PlayOneShot(sonidoEquipar);
    }

    IEnumerator RealizarAtaque(string triggerAnimator, float tiempoCollider, float cooldown, AudioClip clip)
    {
        puedeAtacar = false;

        // Disparar animación
        if (animator != null)
            animator.SetTrigger(triggerAnimator);

        // Sonido del swing
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);

        // Activar collider del arma durante el golpe
        if (armaCollider != null)
        {
            armaCollider.enabled = true;
            yield return new WaitForSeconds(tiempoCollider);
            armaCollider.enabled = false;
        }

        // Esperar cooldown antes de volver a permitir atacar
        yield return new WaitForSeconds(cooldown);
        puedeAtacar = true;
    }
}



