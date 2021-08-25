using GovernanceProjectService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovernanceProjectService.Data
{
    public interface IInputTlfile : ICrud<InputTlfile>
    {
        Task UpdateNotes(string id, string notes);
        Task<IEnumerable<InputTlfile>> CountExistingFileNameInputTL(string filename);
    }
}
