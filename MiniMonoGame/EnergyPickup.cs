namespace MiniMonoGame
{
    public class EnergyPickup : Pickup, IEnergyPickup
    {
        public override void ApplyUsage(IPlayer player)
        {
            if (player.CurrentEnergy < player.BaseEnergy)
            {
                player.IncreaseEnergy(Value);
                Used = true;
            }
        }
    }
}
