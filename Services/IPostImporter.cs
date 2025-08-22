using System.Threading.Tasks;
using System.Collections.Generic;
using JsonPlaceholderApi.Models;

namespace JsonPlaceholderApi.Services
{
  public interface IPostImporter
  {
    Task<IEnumerable<Post>> FetchAndImportAsync();
  }
}
