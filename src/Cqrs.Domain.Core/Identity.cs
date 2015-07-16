namespace Cqrs.Domain.Core
{
    public abstract class Identity
    {
        public abstract string GetId(); //TODO: Introduce tag and identity separately

        public abstract string GetTag();
    }
}
