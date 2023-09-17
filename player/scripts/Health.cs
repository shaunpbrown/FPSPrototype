using System.Runtime.Remoting.Messaging;
using Godot;

public class Health
{
    public const int MaxHealth = 10;
    public int CurrentHealth = MaxHealth;

    private Player _player;
    private TextureRect _bloodTexture;
    private float _lastTakenDamageTimer;
    private float _healTimer;
    private float _invincibilityTimer;
    private Vector3 _headDeathHeight;
    private float _deadTimer;

    public Health(Player player)
    {
        _player = player;
        _bloodTexture = _player.GetNode<TextureRect>("CanvasLayer/Panel/Blood");
        AdjustBloodAlpha(0);
    }

    public void HealthProcess(float delta)
    {
        if (_invincibilityTimer > 0)
            _invincibilityTimer -= delta;

        if (_lastTakenDamageTimer > 0)
        {
            _lastTakenDamageTimer -= delta;
        }
        else
        {
            if (_healTimer > 0)
            {
                _healTimer -= delta;
            }
            else if (CurrentHealth < MaxHealth)
            {
                _healTimer = .2f;
                Heal(1);
            }
        }

    }

    public void DeathProcess(float delta)
    {
        if (IsDead())
        {
            var head = _player.GetNode<Spatial>("Head");
            if (head.GlobalTranslation.y > .5f)
                head.GlobalTranslation = head.GlobalTranslation.LinearInterpolate(_headDeathHeight, delta * 2);

            _deadTimer += delta;
            if (_deadTimer > 10)
            {
                string currentScenePath = _player.GetTree().CurrentScene.Filename;
                _player.GetTree().ChangeScene(currentScenePath);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsDead() || _invincibilityTimer > 0)
            return;

        _lastTakenDamageTimer = 2f;
        _invincibilityTimer = .2f;
        CurrentHealth -= damage;
        AdjustBloodAlpha(1 - (CurrentHealth / (float)MaxHealth));
        if (CurrentHealth <= 0)
            Die();
    }

    public void Heal(int heal)
    {
        if (IsDead())
            return;

        CurrentHealth += heal;
        AdjustBloodAlpha(1 - (CurrentHealth / (float)MaxHealth));
    }

    public bool IsDead() => CurrentHealth <= 0;

    public void Die()
    {
        _headDeathHeight = _player.GetNode<Spatial>("Head").GlobalTranslation + Vector3.Down;
    }

    private void AdjustBloodAlpha(float alpha)
    {
        var color = _bloodTexture.Modulate;
        color.a = alpha;
        _bloodTexture.Modulate = color;
    }
}