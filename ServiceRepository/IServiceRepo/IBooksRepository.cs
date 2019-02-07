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
    }
}
