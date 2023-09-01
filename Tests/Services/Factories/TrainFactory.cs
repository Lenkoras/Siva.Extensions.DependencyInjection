using Tests.Models;

namespace Tests.Services.Factories
{
    public class TrainFactory : Factory<Train>
    {
        public override Train Create()
        {
            return new Train();
        }
    }
}