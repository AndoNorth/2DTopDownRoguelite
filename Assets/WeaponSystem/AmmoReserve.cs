public class AmmoReserve
{
    private int lightReserveAmmo;
    private int heavyReserveAmmo;
    private int energyReserveAmmo;
    private int explosiveReserveAmmo;
    public AmmoReserve(int light, int heavy, int energy, int explosive)
    {
        this.lightReserveAmmo = light;
        this.heavyReserveAmmo = heavy;
        this.energyReserveAmmo = energy;
        this.explosiveReserveAmmo = explosive;
    }
    public int ReserveAmmo(AmmoType ammoType)
    {
        switch (ammoType)
        {
            case (AmmoType.Light):
                return lightReserveAmmo;
            case (AmmoType.Heavy):
                return heavyReserveAmmo;
            case (AmmoType.Energy):
                return energyReserveAmmo;
            case (AmmoType.Explosive):
                return explosiveReserveAmmo;
            default:
                return 0;
        }
    }
    public void AddReserveAmmo(AmmoType ammoType, int amount)
    {
        switch (ammoType)
        {
            case (AmmoType.Light):
                lightReserveAmmo += amount;
                break;
            case (AmmoType.Heavy):
                heavyReserveAmmo += amount;
                break;
            case (AmmoType.Energy):
                energyReserveAmmo += amount;
                break;
            case (AmmoType.Explosive):
                explosiveReserveAmmo += amount;
                break;
            default:
                break;
        }
    }
}
