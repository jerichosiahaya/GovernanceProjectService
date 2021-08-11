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
    }
}
