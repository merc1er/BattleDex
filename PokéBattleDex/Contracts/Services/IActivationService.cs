namespace PokéBattleDex.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
