using GovernanceProjectService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovernanceProjectService.Data
{
    public interface IRhafilesEvidence : ICrud<RhafilesEvidence>
    {
        Task<IEnumerable<RhafilesEvidence>> GetByRhaID(string idRha);
        Task<IEnumerable<RhafilesEvidence>> CountExistingFileNameRhaEvidence(string filename);
    }
}
