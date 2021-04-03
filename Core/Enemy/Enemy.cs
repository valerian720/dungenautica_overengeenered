using Godot;
using SibGameJam2021.Core;
using SibGameJam2021.Core.Managers;

public class Enemy : KinematicBody2D
{
    public SpawnManager SpawnManager { get; set; }
    
    private int _сurrentHealth;
    [Export]
    private int MAX_HEALTH = 100;

    [Export]
    private int GET_DMG_PER_HIT = 30;

    // AI
    [Export]
    private float ActivationRadius = 300;
    [Export]
    private float SightActivationRadius = 350;
    //

    public Enemy()
    {
        CurrentHealth = MAX_HEALTH;
    }

    public int CurrentHealth
    {
        get { return _сurrentHealth; }

        private set { _сurrentHealth = value > 0 ? (value < MAX_HEALTH ? value : MAX_HEALTH) : 0; }
    }

    public override void _PhysicsProcess(float delta)
    {
        var player = GameManager.Instance.Player;

        UpdatePosition(player);

        direction = (player.Position - Position).Normalized();
        
    }

    virtual public void UpdatePosition(Player player)
    {
        if (player.Position.DistanceSquaredTo(Position) < ActivationRadius* ActivationRadius)
        {
            // базовое перемещение в сторону игрока если он находится в некотром радиусе от моба
            Position += (player.Position - Position) / 50;
            SetAnimationRun();
        }
        else 
        {
            SetAnimationIdle();
        }
    }
    virtual public void UpdateAnimation(Player player)
    {
        if (player.Position.DistanceSquaredTo(Position) < SightActivationRadius * SightActivationRadius)
        {
            // обновление анимаций моба если игрок входит в определенный радиус
            UpdateAnimationTreeState();
        }
    }

    private void _on_Area2D_body_entered(Node body)
    {
        
        DealDmg();
    }

    private void DealDmg()
    {
        CurrentHealth -= GET_DMG_PER_HIT;
        SetAnimationHurt();

        if (CurrentHealth == 0)
        {
            Kill();
        }
    }
    private void Kill()
    {
        QueueFree();
        SpawnManager.EnemiesAlive--;
    }

    // я хз будет ли это норм работать на всех противников или только на рыбу
    private AnimationNodeStateMachinePlayback animationState = null;
    private AnimationTree animationTree = null;

    private Vector2 direction;

    public override void _Ready()
    {
         animationTree = GetNode("Animations/AnimationTree") as AnimationTree; // da
         animationState = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback; // da

        animationTree.Active = true;

        SetAnimationRun(); // TODO
    }

    private void UpdateAnimationTreeState()
    {
        animationTree.Set("parameters/Attack/blend_position", direction);
        animationTree.Set("parameters/Hurt/blend_position", direction);
        animationTree.Set("parameters/Idle/blend_position", direction);
        animationTree.Set("parameters/Run/blend_position", direction);
    }

    private void SetAnimationIdle() {
        animationState.Travel("Idle");
    }

    private void SetAnimationRun() {
        animationState.Travel("Run");
    }

    private void SetAnimationAttack() {
        animationState.Travel("Attack");
    }

    private void SetAnimationHurt() {
        animationState.Travel("Hurt");
    }
}