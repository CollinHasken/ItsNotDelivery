using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerCrashedEvent : UnityEvent<GameObject> { }


public class Player : MonoBehaviour
{    
    public PlayerCrashedEvent playerCrashedEvent
    {
        get
        {
            if (m_PlayerCrashedEvent == null)
                m_PlayerCrashedEvent = new PlayerCrashedEvent();
            return m_PlayerCrashedEvent;
        }
    }


    public GameObject playerExplosion;
    public SpriteRenderer playerIcon;
    public double minKineticEnergyToCrash = 100;
    public int playerIndex = Constants.InvalidPlayerIndex;
    public List<AudioClip> playerDeliverySFX = new List<AudioClip>();
    public List<AudioClip> playerHitSFX = new List<AudioClip>();
    public List<AudioClip> playerScreamSFX = new List<AudioClip>();
    public List<AudioClip> playerSpawnSFX = new List<AudioClip>();

    public SkinnedMeshRenderer playerBody;

    private Color playerColor;
    private PlayerCrashedEvent m_PlayerCrashedEvent;
    private AudioSource playerAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        playerAudioSource = GetComponent<AudioSource>();
        playerAudioSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(Color color, Material bodyColor)
    {
        playerColor = color;
        playerIcon.color = playerColor;

        if (playerBody != null)
        {
            playerBody.materials[0] = bodyColor;
        }
    }

    public Color GetColor()
    {
        return playerColor;
    }

    public void SetIndex(int index)
    {
        playerIndex = index;
    }

    public void PlaySpawnSFX()
    {
        if(playerAudioSource == null)
        {
            playerAudioSource = GetComponent<AudioSource>();
            playerAudioSource.playOnAwake = false;
        }
        playerAudioSource.Stop();
        playerAudioSource.clip = playerSpawnSFX[playerIndex];
        playerAudioSource.Play();
    }

    void OnStart()
    {
        if (!GameManager.inGame)
        {
            GameManager.Instance.LoadGame();
        }
        else
        {
            MenuManager.Instance.SetPause(true);
        }
    }

    public void Respawn(Transform respawnPoint)
    {
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        RenderChildren(true);
        GetComponent<BikeControllerEasy>().enabled = true;

        playerAudioSource.Stop();
        playerAudioSource.clip = playerSpawnSFX[playerIndex];
        playerAudioSource.Play();
    }

    public void RenderChildren(bool shouldRender)
    {
        RenderChildrenInternal(transform, shouldRender);
    }

    public void OnPizzaDelivered()
    {
        playerAudioSource.Stop();
        playerAudioSource.clip = playerDeliverySFX[playerIndex];
        playerAudioSource.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Powerup")
        {
            return;
        }

        if(collision.relativeVelocity.magnitude < 5)
        {
            return;
        }

        if (collision.transform.tag == "NonPlayerVehicle")
        {
            AudioSource carAudio = collision.transform.GetComponent<AudioSource>();
            carAudio.Stop();
            carAudio.clip = GameManager.Instance.vehicleHitSFX[Random.Range(0, GameManager.Instance.vehicleHitSFX.Count)];
            carAudio.Play();
        }

        double kineticEnergy = 0.5 * collision.rigidbody.mass * collision.relativeVelocity.magnitude * collision.relativeVelocity.magnitude;
        if(kineticEnergy < minKineticEnergyToCrash)
        {
            playerAudioSource.Stop();
            playerAudioSource.clip = playerHitSFX[playerIndex];
            playerAudioSource.Play();

            return;
        }
        Debug.Log("Player " + playerColor.ToString() + " crashed after hitting object " + collision.collider.gameObject.transform.root.name + " with mass " + collision.rigidbody.mass + ", relative velocity " + collision.relativeVelocity.magnitude + ", and KE " + kineticEnergy);
        Crashed();
    }

    private void Crashed()
    {
        // Play animation?
        playerAudioSource.Stop();
        playerAudioSource.clip = playerScreamSFX[playerIndex];
        playerAudioSource.Play();

        RenderChildren(false);
        GetComponent<BikeControllerEasy>().enabled = false;

        GameObject particleEmitter = Instantiate(playerExplosion, transform.position, transform.rotation);
        Explosion explosion = particleEmitter.GetComponent<Explosion>();
        if (explosion == null)
        {
            Debug.LogError("Explosion particle emitter doesn't have explosion script");
            playerCrashedEvent.Invoke(gameObject);
            return;
        }

        explosion.particlesStopped.AddListener(OnExplosionDone);
    }

    private void OnExplosionDone()
    {
        playerCrashedEvent.Invoke(gameObject);
    }

    private void RenderChildrenInternal(Transform parent, bool shouldRender)
    { 
        foreach (Transform child in parent)
        {
            MeshRenderer renderer = child.gameObject.GetComponent<MeshRenderer>();
            if(renderer != null)
            {
                renderer.enabled = shouldRender;
            }

            SkinnedMeshRenderer skinnedMeshRenderer = child.gameObject.GetComponent<SkinnedMeshRenderer>();
            if(skinnedMeshRenderer != null)
            {
                skinnedMeshRenderer.enabled = shouldRender;
            }

            if (child.childCount > 0)
            {
                RenderChildrenInternal(child, shouldRender);
            }
        }
    }
}
