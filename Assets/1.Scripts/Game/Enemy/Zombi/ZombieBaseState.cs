public abstract class ZombieBaseState : UnitBaseState
{
    protected ZombieController Controller;

    public ZombieBaseState(ZombieController zombieController)
    {
        Controller = zombieController;
    }
}