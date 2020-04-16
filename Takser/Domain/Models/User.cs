namespace Takser.Domain.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string HashedPassword { get; set; }
    }
}