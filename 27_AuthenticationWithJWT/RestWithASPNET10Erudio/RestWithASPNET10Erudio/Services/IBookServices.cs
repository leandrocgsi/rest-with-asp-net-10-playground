using RestWithASPNET10Erudio.Data.DTO.V1;

namespace RestWithASPNET10Erudio.Services
{
    public interface IBookServices
    {

        BookDTO Create(BookDTO book);

        BookDTO FindById(long id);

        List<BookDTO> FindAll();

        BookDTO Update(BookDTO book);

        void Delete(long id);
    }
}
