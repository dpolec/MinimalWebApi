using MinimalWebApi.Models;

namespace MinimalWebApi.Interfaces
{
    public interface IIdNameRepo
    {
        void Add(IdName randomValue);

        void Delete(int id);

        IdName Get(int id);

        IReadOnlyCollection<IdName> Get();

        void Update(IdName randomValue);
    }
}