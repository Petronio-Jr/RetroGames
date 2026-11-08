namespace RetroGames.Repository;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException()
        : base("Já existe um registro com esses dados.")
    {
    }

    public RegistroDuplicadoException(string mensagem)
        : base(mensagem)
    {
    }

    public RegistroDuplicadoException(string mensagem, Exception excecaoInterna)
        : base(mensagem, excecaoInterna)
    {
    }
}