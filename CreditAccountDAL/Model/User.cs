
namespace CreditAccountDAL
{
    public class User
    {
        public long Id { get; }
        public string Name { get; }

        public User(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
