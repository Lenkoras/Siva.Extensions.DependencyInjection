using Tests.Models;

namespace Tests.Services.Factories
{
    public class ShoeFactory : Factory<Shoe>
    {
        public override Shoe Create()
        {
            return new Shoe();
        }
    }
}