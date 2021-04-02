using Godot;

public class Enemy : KinematicBody2D
{
    private const int MAX_HEALTH = 100;
    private const int DMG_PER_HIT = 30;

    public int CurrentHealth
    {
        get { return _сurrentHealth; }

        private set { _сurrentHealth = value > 0 ? (value < MAX_HEALTH ? value : MAX_HEALTH) : 0; }
    }
    private int _сurrentHealth;

    public Enemy()
    {
        CurrentHealth = MAX_HEALTH;
    }
    public override void _PhysicsProcess(float delta)
    {
        Node2D player = GetParent().GetNode("Player") as Node2D;

        Position += (player.Position - Position) / 50;
    }

    public override void _Ready()
    {
    }

    private void kill()
    {
        QueueFree();
    }
    private void DealDmg()
    {
        CurrentHealth -= DMG_PER_HIT;

        if (CurrentHealth == 0)
        {
            kill();
        }
    }

    private void _on_Area2D_body_entered(Area2D body)
    {
        if (body.Name.IndexOf("Bullet") > 0)
        {
            DealDmg();
        }
        GD.Print("enemy");
        GD.Print(body.Name);
    }
}