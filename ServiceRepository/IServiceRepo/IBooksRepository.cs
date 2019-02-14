using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceRepository
{
    public interface IBooksRepository
    {
        Task<List<BookDetails>> GetAllBooks();

        Task<List<BookDetails>> GetAllAvailableBooks();

        Task<Response<string>> AddSubCategoryToExistingBook(ISBNNumber isbnDetails);

        Task<bool> UpdateBookDetails(BookDetails bookDetails);

        Task<BookDetails> AddNewBook(BookDetails bookDetails);

        Task<bool> DeleteBookDetails(ISBNNumber isbnDetails);

    }
}
