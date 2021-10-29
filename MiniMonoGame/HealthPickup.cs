namespace MiniMonoGame
{
    public class HealthPickup : Pickup, IHealthPickup
    {
        public override void ApplyUsage(IPlayer player)
        {
            if (player.CurrentHealth < player.BaseHealth)
            {
                player.IncreaseHealth(Value);
                Used = true;
            }
        }
    }
}
