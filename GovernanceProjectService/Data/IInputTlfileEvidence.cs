using GovernanceProjectService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovernanceProjectService.Data
{
    public interface IInputTlfileEvidence : ICrud<InputTlfilesEvidence>
    {
        Task<IEnumerable<InputTlfilesEvidence>> CountExistingFileNameInputTLEvidence(string filename);
    }
}
