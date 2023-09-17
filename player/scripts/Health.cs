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

    public void TakeDamage(int damage)
    {
        if (IsDead() || _invincibilityTimer > 0)
            return;

        _lastTakenDamageTimer = 2f;
        _invincibilityTimer = .2f;
        CurrentHealth -= damage;
        GD.Print(CurrentHealth);
        AdjustBloodAlpha(1 - (CurrentHealth / (float)MaxHealth));
        if (CurrentHealth <= 0)
            Die();
    }

    public void Heal(int heal)
    {
        if (IsDead())
            return;

        CurrentHealth += heal;
        GD.Print(CurrentHealth);
        AdjustBloodAlpha(1 - (CurrentHealth / (float)MaxHealth));
    }

    public bool IsDead() => CurrentHealth <= 0;

    public void Die()
    {
        GD.Print("YOU HAVE DIED");
    }

    private void AdjustBloodAlpha(float alpha)
    {
        var color = _bloodTexture.Modulate;
        color.a = alpha;
        _bloodTexture.Modulate = color;
    }
}