using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum estadoPlayer { parado, andandoIzq, andandoDer, corriendoIzq,  corriendoDer, saltando, volando, inmune }

public class Player : MonoBehaviour
{
    float zPos;
    float ySpeedActual;
    estadoPlayer estado;
    Rigidbody rb;
    bool corriendo = false;
    Animator playerAnimator;

    [Header ("Características")]
    [SerializeField] int velocidad;
    [SerializeField] int fuerzaSalto = 10;
    [SerializeField] GameObject posicionPies;
    [Header("Objetos")]
    [SerializeField] int cantidadCombustibleDeVuelo = 100;
    private int maxcantidadCombustibleDeVuelo = 100;
    bool ignicion = false;
    float tiempoActivarTrigger = 1f;
    [Header("Salud")]
    [SerializeField] int salud = 100;
    private int saludMaxima = 100;
    [SerializeField] int vidas = 1;
    private int vidasMaximas = 3;
    [Header("FX")]
    [SerializeField] ParticleSystem explosionIgnicion;
    [SerializeField] AudioSource sonidoIgnicion;
    [SerializeField] ParticleSystem lanzallamas;
    [SerializeField] ParticleSystem laser;
    [Header("Armas")]
    [SerializeField] Transform disparoVolando;
    [SerializeField] Transform disparoTierra;
    [SerializeField] int energiaArmas = 100;
    private int energiaMaximaArmas = 100;



    private void Start()
    {
        estado = estadoPlayer.parado;
        rb = this.GetComponent<Rigidbody>();
        playerAnimator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        CorrerEstaApretado();
        SaltarEstaApretado();
        Animar();
        Disparar();
    }


    private void FixedUpdate()
    {
        zPos = Input.GetAxis("Horizontal");
        ySpeedActual = rb.velocity.y;
        VolarSaltarCaminar();
        CambiarDireccion();
    }


    private void CorrerEstaApretado()
    {
        if(estado != estadoPlayer.saltando)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                velocidad *= 2;
                corriendo = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                velocidad /= 2;
                corriendo = false;
            }
        }
        
    }

    private void SaltarEstaApretado()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            estado = estadoPlayer.saltando;
            Invoke("ActivarTrigger", tiempoActivarTrigger);
            ignicion = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ignicion = false;
        }

    }



    private void CambiarDireccion()
    {
        if (zPos > 0.01f)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
            if (estado != estadoPlayer.saltando)
            {
                estado = (corriendo) ? estadoPlayer.corriendoDer : estadoPlayer.andandoDer;
            }

        }
        if (zPos < -0.01f)
        {
            this.transform.localScale = new Vector3(1, 1, -1);
            if (estado != estadoPlayer.saltando)
            {
                estado = (corriendo) ? estadoPlayer.corriendoIzq : estadoPlayer.andandoIzq;
            }
        }
        if (zPos < 0.01f && zPos > -0.01f)
        {
            if (estado != estadoPlayer.saltando)
            {
                estado = estadoPlayer.parado;
            }
        }
    }

    private void Animar()
    {
        playerAnimator.SetInteger("estadoPlayer", (int)estado);
    }


    private void VolarSaltarCaminar()
    {
        float velocidadY = ySpeedActual;
         
        if (estado == estadoPlayer.saltando && EstaEnSuelo())
        {
            velocidadY = fuerzaSalto;
        }
        else if (ignicion && cantidadCombustibleDeVuelo > 0)
        {
            velocidadY = fuerzaSalto;
            explosionIgnicion.Play();
            sonidoIgnicion.Play();  
            cantidadCombustibleDeVuelo -= 1;
        }
        rb.velocity = new Vector3(0, velocidadY, zPos * velocidad);


    }


    private bool EstaEnSuelo()
    {
        bool enSuelo = false;
        float radio = 0.1f;
        Collider[] col = Physics.OverlapSphere(posicionPies.transform.position, radio);
        foreach (Collider c in col)
        {
            print(c.tag);
            if (c != null && !c.gameObject.tag.Equals("Player"))
            {
                    enSuelo = true;
            }
        }
        return enSuelo;

    }


    private void ActivarTrigger()
    {
        posicionPies.GetComponent<BoxCollider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        estado = estadoPlayer.parado;
        posicionPies.GetComponent<BoxCollider>().enabled = false;
    }

    private void Disparar()
    {
        Transform posicionDisparo = null;
        ParticleSystem ps = null;

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (estado == estadoPlayer.parado)
            {
                posicionDisparo = disparoVolando;
                ps = Instantiate(lanzallamas, posicionDisparo.transform);
            }
            else
            {
                posicionDisparo = disparoTierra;
                ps = Instantiate(laser, posicionDisparo.transform);
            }
            ps.Play();
            Destroy(ps.gameObject, 2);
        }


    }




}
