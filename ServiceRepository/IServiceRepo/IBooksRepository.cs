using Models;
using Models.Model;
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

        Task<BookDetails> AddNewBook(BookDetails bookDetails, string image);

        Task<bool> DeleteBookDetails(ISBNNumber isbnDetails);
        bool ReturnBooks(IssueBooks issueBooks);
        Task<bool> IssueBooks(IssueBooks issueBooks);
        Task<bool> BlockBooks(BlockBooks blockedbookdetails);
        Task<bool> UnBlockBooks(BlockBooks blockedbookdetails);

        Task<List<ISBNNumber>> GetAllIsbnDetails();

        Task<List<LatestBooks>> GetAllLatestBookDetails();

        Task<bool> EditIsbnDetails(ISBNNumber iSBNNumber);
    }
}
