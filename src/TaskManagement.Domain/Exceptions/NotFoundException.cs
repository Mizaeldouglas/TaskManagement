namespace TaskManagement.Domain.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string name, object key)
            : base($"Entidade \"{name}\" ({key}) não foi encontrada.")
        {
        }
    }
}
