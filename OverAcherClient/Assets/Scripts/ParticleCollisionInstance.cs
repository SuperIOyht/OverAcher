/*This script created by using docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;
public class ParticleCollisionInstance : NetworkBehaviour
{
    
    public GameObject[] EffectsOnCollision;
    public float Offset = 0;
    public string teamFrom;
    public float damage;
    public float destroyAfter = 5;//自身删除
    public int ArrowEffectType = 0;//1 表示冰冻，2表示中毒，0无法球
    //public bool UseWorldSpacePosition;
    //public bool UseFirePointRotation;
    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void Start()
    {
        part = GetComponent<ParticleSystem>();
    }
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
    [ServerCallback]
    void OnParticleCollision(GameObject other)
    {      
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);     
        for (int i = 0; i < numCollisionEvents; i++)
        {
            foreach (var effect in EffectsOnCollision)
            {
                if (other.tag == "BlueArrow" || other.tag == "RedArrow" || other.tag == "Arrow")
                {
                    return;
                }
                if (teamFrom == "TeamRed")
                {
                    if (other.tag == "TeamBlue")
                    {
                        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                        if(ArrowEffectType == 1)
                        {
                            playerController.beFrozen = true;
                        }
                        if (ArrowEffectType == 2)
                        {
                            playerController.bePoisoned = true;
                        }
                        playerController.BeAttacked(this.damage);
                    }
                }
                else if (teamFrom == "TeamBlue")
                {
                    if (other.tag == "TeamRed")
                    {
                        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                        if (ArrowEffectType == 1)
                        {
                            playerController.PlayerFrozend();
                        }
                        if (ArrowEffectType == 2)
                        {
                            playerController.playerPoisoned();
                        }
                        playerController.BeAttacked(this.damage);
                    }
                }
                var instance = Instantiate(effect, collisionEvents[i].intersection + collisionEvents[i].normal * Offset, new Quaternion()) as GameObject;
                NetworkServer.Spawn(instance);
            }
        }
        NetworkServer.Destroy(gameObject);
    }
}
