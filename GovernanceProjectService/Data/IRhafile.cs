using GovernanceProjectService.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovernanceProjectService.Data
{
    public interface IRhafile : ICrud<Rhafile>
    {
        //Task UploadFile(IFormFile file);
        Task<IEnumerable<Rhafile>> GetByNPP(string npp);
        Task<IEnumerable<Rhafile>> CountRha();
        Task<IEnumerable<Rhafile>> CountRhaDone();
        Task<IEnumerable<Rhafile>> CountRhaPending();
        Task<IEnumerable<Rhafile>> CountExistingFileNameRha(string filename);

    }
}
