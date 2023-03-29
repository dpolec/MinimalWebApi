using MinimalWebApi.Interfaces;
using MinimalWebApi.Models;

namespace MinimalWebApi.Repositories
{
    public class IdNameRepo : IIdNameRepo
    {
        private List<IdName> _list = new();

        public IdNameRepo()
        {
            for (int i = 0; i < 20; i++)
            {
                Add(new IdName(i, $"Name-{i}"));
            }
        }

        public void Add(IdName mdeol) => _list.Add(mdeol);

        public void Delete(int id) => _list = _list.Where(_ => _.Id != id).ToList();

        public IdName Get(int id) => _list.Single(_ => _.Id == id);

        public IReadOnlyCollection<IdName> Get() => _list;

        public void Update(IdName mdeol)
        {
            Delete(mdeol.Id);
            Add(mdeol);
        }
    }
}