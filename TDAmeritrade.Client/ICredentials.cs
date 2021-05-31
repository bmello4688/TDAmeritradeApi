namespace TDAmeritradeApi.Client
{
    public interface ICredentials
    {
        string GetUserName();

        string GetPassword();

        string GetSmsCode();
    }
}
