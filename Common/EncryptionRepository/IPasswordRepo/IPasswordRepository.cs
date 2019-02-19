using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.EncryptionRepository
{
    public interface IPasswordRepository
    {

        Task<string> GetEncryptedPassword( string password);
    }
}
