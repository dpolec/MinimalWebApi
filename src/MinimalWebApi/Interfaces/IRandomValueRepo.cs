using MinimalWebApi.Models;

namespace MinimalWebApi.Interfaces
{
    public interface IRandomValueRepo
    {
        void Add(RandomValue model);

        void Delete(int id);

        RandomValue Get(int id);

        IReadOnlyCollection<RandomValue> Get();

        void Update(RandomValue model);
    }
}