using MinimalWebApi.Interfaces;
using MinimalWebApi.Models;

namespace MinimalWebApi.Repositories
{
    public class RandomValueRepo : IRandomValueRepo
    {
        private List<RandomValue> _list = new();

        public RandomValueRepo()
        {
            for (int i = 0; i < 20; i++)
            {
                Add(new RandomValue(i, $"Value-{i}"));
            }
        }

        public void Add(RandomValue mdeol) => _list.Add(mdeol);

        public void Delete(int id) => _list = _list.Where(_ => _.Id != id).ToList();

        public RandomValue Get(int id) => _list.Single(_ => _.Id == id);

        public IReadOnlyCollection<RandomValue> Get() => _list;

        public void Update(RandomValue mdeol)
        {
            Delete(mdeol.Id);
            Add(mdeol);
        }
    }
}