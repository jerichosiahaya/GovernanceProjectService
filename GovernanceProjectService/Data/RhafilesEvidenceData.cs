using GovernanceProjectService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovernanceProjectService.Data
{
    public class RhafilesEvidenceData : IRhafilesEvidence
    {
        private GesitDbContext _db;
        public RhafilesEvidenceData(GesitDbContext db)
        {
            _db = db;
        }
        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task Insert(RhafilesEvidence obj)
        {
            try
            {
                _db.RhafilesEvidences.Add(obj);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception(dbEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task Update(string id, RhafilesEvidence obj)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RhafilesEvidence>> GetAll()
        {
            var result = await _db.RhafilesEvidences.OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<RhafilesEvidence> GetById(string id)
        {
            var result = await _db.RhafilesEvidences.Where(s => s.Id == Convert.ToInt32(id)).FirstOrDefaultAsync();
            return result;
        }

        public Task<IEnumerable<RhafilesEvidence>> GetByNPP(string idRha)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RhafilesEvidence>> GetByRhaID(string idRha)
        {
            var result = await _db.RhafilesEvidences.OrderByDescending(s => s.CreatedAt).Where(s => s.RhafilesId == Convert.ToInt32(idRha)).AsNoTracking().ToListAsync();
            return result;
        }
    }
}
