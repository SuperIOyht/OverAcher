using System.Collections;
using UnityEngine;
using Mirror;

namespace WarriorAnimsFREE
{
    public enum Warrior
    {
        Archer,
        Brute,
        Crossbow,
        Hammer,
        Karate,
        Knight,
        Mage,
        Ninja,
        Sorceress,
        Spearman,
        Swordsman,
        TwoHanded
    }

    public class WarriorControllerFREE : NetworkBehaviour
    {
        //Components.
        [HideInInspector] public WarriorMovementControllerFREE warriorMovementController;
        [HideInInspector] public WarriorInputControllerFREE warriorInputController;
        [HideInInspector] public WarriorTimingFREE warriorTiming;
        [HideInInspector] public Animator animator;
        [HideInInspector] public IKHands ikHands;
        public Warrior warrior;

        //Strafing/action.
        [HideInInspector] public bool canAction = true;
        [HideInInspector] int attack;

        //Animation speed control. (doesn't affect lock timing)
        public float animationSpeed = 1;

        #region Initialization

        private void Awake()
        {
            //Get Movement Controller.
            warriorMovementController = GetComponent<WarriorMovementControllerFREE>();

            //Add Input and Timing Controllers.
            warriorInputController = gameObject.AddComponent<WarriorInputControllerFREE>();
            warriorTiming = gameObject.AddComponent<WarriorTimingFREE>();
            warriorTiming.warriorController = this;

            //Add IKHands.
            ikHands = GetComponent<IKHands>();

            //Setup Animator, add AnimationEvents script.
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("ERROR: There is no Animator component for character.");
                Destroy(this);
            }
            else
            {
                animator.gameObject.AddComponent<WarriorCharacterAnimatorEvents>();
                animator.GetComponent<WarriorCharacterAnimatorEvents>().warriorController = this;
                animator.gameObject.AddComponent<AnimatorParentMove>();
                animator.GetComponent<AnimatorParentMove>().anim = animator;
                animator.GetComponent<AnimatorParentMove>().warriorMovementController = warriorMovementController;
                animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
                animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            }

            leftHandTrans = transform.Find("ArrowSpawn");
        }

        #endregion

        #region Updates

        public virtual void Update()
        {
            UpdateAnimationSpeed();
            //Character is on ground.
            if (warriorMovementController.MaintainingGround() && canAction)
            {
                Attacking();
            }
            Toggles();
        }

        private void UpdateAnimationSpeed()
        {
            animator.SetFloat("AnimationSpeed", animationSpeed);
        }

        #endregion

        #region Combat

        private void Attacking()
        {
            if (warriorInputController.inputAttack0)
            {
                Attack(1);
            }
        }

        public bool Jump()
        {
            return warriorInputController.inputJump;
        }

        public void Attack(int attackNumber)
        {
            if (canAction)
            {
                //Ground attacks.
                if (warriorMovementController.MaintainingGround())
                {
                    Lock(true, true, true, 0, warriorTiming.TimingLock(warrior, ("attack" + attackNumber.ToString())));
                    CmdAttack(transform, attackNumber);
                    //Stationary attack.
                    if (!warriorMovementController.isMoving)
                    {

                    }
                }
                //Trigger the animation.
            }
        }
        [SyncVar]
        public float health = 100;
        [SyncVar]
        public float sheild = 0;
        [SyncVar]
        public float damage = 10;

        [SyncVar]
        public bool isSpeedUp = false;
        [SyncVar]
        public bool canSplite = false;
        [SyncVar]
        public bool canHitFrozenArrow = false;
        [SyncVar]
        public bool canHitPoisonArrow = false;
        [SyncVar]
        public bool canHitBallArrow = false;
        [SyncVar]
        public bool haveShield = false;
        [SyncVar]
        public bool bePoisoned = false;
        [SyncVar]
        public bool beFrozen = false;
        [SyncVar]
        public bool canHitFireArrow = false;

        public void resetEffect()
        {
            canHitFrozenArrow = false;
            canHitPoisonArrow = false;
            canHitBallArrow = false;
            canHitFireArrow = false;
            damage = 10;
        }

        public GameObject ArrowPrefab;
        public GameObject FireArrowPrefab;
        public GameObject BallArrowPrefab;
        public GameObject FrozenArrowPrefab;
        public GameObject PoisonArrowPrefab;
        private Transform leftHandTrans; // 箭的位置是左手的位置
        IEnumerator attackSync(Transform trans)
        {
            yield return new WaitForSeconds(0.3f);
            GameObject nowArrow = ArrowPrefab;
            bool canHitEffectArrow = false;
            if (canHitFrozenArrow)
            {
                nowArrow = FrozenArrowPrefab;
                canHitEffectArrow = true;
            }
            else if (canHitFireArrow)
            {
                nowArrow = FireArrowPrefab;
                canHitEffectArrow = true;
            }
            else if (canHitPoisonArrow)
            {
                nowArrow = PoisonArrowPrefab;
                canHitEffectArrow = true;
            }
            else if (canHitBallArrow)
            {
                nowArrow = BallArrowPrefab;
                canHitEffectArrow = true;
            }
            if (canSplite)
            {
                if (canHitEffectArrow)
                {
                    trans.Rotate(new Vector3(0, -100, 0), Space.Self);
                    GameObject arrowleft = Instantiate(nowArrow, leftHandTrans.position, leftHandTrans.rotation);
                    ParticleCollisionInstance arrowController1 = arrowleft.GetComponent<ParticleCollisionInstance>();
                    arrowController1.teamFrom = this.tag;
                    arrowController1.damage = damage;
                    trans.Rotate(new Vector3(0, 20, 0), Space.Self);
                    GameObject arrowright = Instantiate(nowArrow, leftHandTrans.position, leftHandTrans.rotation);
                    ParticleCollisionInstance arrowController2 = arrowright.GetComponent<ParticleCollisionInstance>();
                    arrowController2.teamFrom = this.tag;
                    arrowController2.damage = damage;
                    NetworkServer.Spawn(arrowleft);
                    NetworkServer.Spawn(arrowright);
                    trans.Rotate(new Vector3(0, 80, 0), Space.Self);
                    if (canHitFrozenArrow)
                    {
                        arrowController1.ArrowEffectType = 1;
                        arrowController2.ArrowEffectType = 1;
                    }
                    if (canHitPoisonArrow)
                    {
                        arrowController2.ArrowEffectType = 2;
                        arrowController1.ArrowEffectType = 2;
                    }
                }
                else
                {
                    trans.Rotate(new Vector3(0, -10, 0), Space.Self);
                    GameObject arrowleft = Instantiate(nowArrow, leftHandTrans.position, leftHandTrans.rotation);
                    ArrowController arrowController1 = arrowleft.GetComponent<ArrowController>();
                    arrowController1.teamFrom = this.tag;
                    arrowController1.damage = damage;
                    trans.Rotate(new Vector3(0, 20, 0), Space.Self);
                    GameObject arrowright = Instantiate(nowArrow, leftHandTrans.position, leftHandTrans.rotation);
                    ArrowController arrowController2 = arrowright.GetComponent<ArrowController>();
                    arrowController2.teamFrom = this.tag;
                    arrowController2.damage = damage;
                    NetworkServer.Spawn(arrowleft);
                    NetworkServer.Spawn(arrowright);
                    trans.Rotate(new Vector3(0, -10, 0), Space.Self);
                }
              
            }
            if(canHitEffectArrow)
            {
                trans.Rotate(new Vector3(0, -90, 0), Space.Self);
                GameObject arrow = Instantiate(nowArrow, leftHandTrans.position, leftHandTrans.rotation);
                ParticleCollisionInstance arrowController = arrow.GetComponent<ParticleCollisionInstance>();
                arrowController.teamFrom = this.tag;
                arrowController.damage = damage;
                if (canHitFrozenArrow)
                {
                    arrowController.ArrowEffectType = 1;
                }
                if (canHitPoisonArrow)
                {
                    arrowController.ArrowEffectType = 2;
                }
                NetworkServer.Spawn(arrow);
                trans.Rotate(new Vector3(0, 90, 0), Space.Self);
            }
            else
            {
                GameObject arrow = Instantiate(nowArrow, leftHandTrans.position, leftHandTrans.rotation);
                ArrowController arrowController = arrow.GetComponent<ArrowController>();
                arrowController.teamFrom = this.tag;
                arrowController.damage = damage;
                NetworkServer.Spawn(arrow);
            }
            yield break;
        }

        [Command]
        void CmdAttack(Transform trans, int attackNumber)
        {
            RpcAttack(attackNumber);
            StartCoroutine(attackSync(trans));
            //GameObject arraw = Instantiate(arrowPrefab, leftHandTrans.position, trans.rotation);
            //NetworkServer.Spawn(arraw);
            //Invoke("CmdShoot", 0.1f);// 延迟调用，时转向和箭实例化同步
        }

        [ClientRpc]
        void RpcAttack(int attackNumber)
        {
            animator.SetInteger("Action", attackNumber);
            animator.SetTrigger("AttackTrigger");
        }

        IEnumerator _Attack1()
        {
            StopAllCoroutines();
            animator.SetInteger("Action", 1);
            animator.SetTrigger("AttackTrigger");
            Lock(true, true, true, 0, warriorTiming.TimingLock(warrior, "attack1"));
            attack = 1;
            yield return null;
        }

        #endregion

        #region Misc

        private void Toggles()
        {
            //Slow time toggle.
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (Time.timeScale != 1)
                {
                    Time.timeScale = 1;
                }
                else
                {
                    Time.timeScale = 0.25f;
                }
            }
            //Pause toggle.
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (Time.timeScale != 1)
                {
                    Time.timeScale = 1;
                }
                else
                {
                    Time.timeScale = 0f;
                }
            }
            //Debug toggle.
            if (Input.GetKeyDown(KeyCode.L))
            {
                VariablesDebug();
            }
        }

        /// <summary>
        /// Keep character from doing actions.
        /// </summary>
        private void LockAction()
        {
            canAction = false;
        }

        /// <summary>
        /// Let character move and act again.
        /// </summary>
        private void UnLock(bool movement, bool actions)
        {
            if (movement)
            {
                warriorMovementController.UnlockMovement();
            }
            if (actions)
            {
                canAction = true;
            }
        }

        private IEnumerator _GetCurrentAnimationLength()
        {
            yield return new WaitForEndOfFrame();
            float length = animator.GetCurrentAnimatorClipInfo(0).Length;
            Debug.Log(length);
        }

        /// <summary>
        /// Lock character movement and/or action, on a delay for a set time.
        /// </summary>
        /// <param name="lockMovement">If set to <c>true</c> lock movement.</param>
        /// <param name="lockAction">If set to <c>true</c> lock action.</param>
        /// <param name="timed">If set to <c>true</c> timed.</param>
        /// <param name="delayTime">Delay time.</param>
        /// <param name="lockTime">Lock time.</param>
        public void Lock(bool lockMovement, bool lockAction, bool timed, float delayTime, float lockTime)
        {
            StopCoroutine("_Lock");
            StartCoroutine(_Lock(lockMovement, lockAction, timed, delayTime, lockTime));
        }

        //Timed -1 = infinite, 0 = no, 1 = yes.
        public IEnumerator _Lock(bool lockMovement, bool lockAction, bool timed, float delayTime, float lockTime)
        {
            if (delayTime > 0)
            {
                yield return new WaitForSeconds(delayTime);
            }
            if (lockMovement)
            {
                warriorMovementController.LockMovement();
            }
            if (lockAction)
            {
                LockAction();
            }
            if (timed)
            {
                if (lockTime > 0)
                {
                    yield return new WaitForSeconds(lockTime);
                    UnLock(lockMovement, lockAction);
                }
            }
        }

        public void AnimatorDebug()
        {
            Debug.Log("ANIMATOR SETTINGS---------------------------");
            Debug.Log("Moving: " + animator.GetBool("Moving"));
            Debug.Log("Strafing: " + animator.GetBool("Strafing"));
            Debug.Log("Aiming: " + animator.GetBool("Aiming"));
            Debug.Log("Stunned: " + animator.GetBool("Stunned"));
            Debug.Log("Blocking: " + animator.GetBool("Blocking"));
            Debug.Log("Jumping: " + animator.GetInteger("Jumping"));
            Debug.Log("Action: " + animator.GetInteger("Action"));
            Debug.Log("Velocity X: " + animator.GetFloat("Velocity X"));
            Debug.Log("Velocity Z: " + animator.GetFloat("Velocity Z"));
        }

        public void VariablesDebug()
        {
            Debug.Log("VARIABLE SETTINGS---------------------------");
            Debug.Log("canAction: " + canAction);
            Debug.Log("attack: " + attack);
        }

        #endregion

    }
}