using SerkoTest.BusinessRepository.Objects;

namespace SerkoTest.BusinessRepository.Interface
{
    public interface IParseTextRepository
    {
        ExpenseClaim ParseMessage(string message);
    }
}
