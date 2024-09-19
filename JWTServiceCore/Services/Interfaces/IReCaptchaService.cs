namespace JWTServiceCore.Services.Interfaces
{
    public interface IReCaptchaService
    {
        Task<bool> IsCaptchaVerifiedAsync();
    }
}