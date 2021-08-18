using GovernanceProjectService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovernanceProjectService.Data
{
    public class InputTlfileEvidenceData : IInputTlfileEvidence
    {
        private GesitDbContext _db;
        public InputTlfileEvidenceData(GesitDbContext db)
        {
            _db = db;
        }
        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InputTlfilesEvidence>> GetAll()
        {
            var result = await _db.InputTlfilesEvidences.OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<InputTlfilesEvidence> GetById(string id)
        {
            var result = await _db.InputTlfilesEvidences.Where(s => s.Id == Convert.ToInt32(id)).FirstOrDefaultAsync();
            return result;
        }

        public async Task Insert(InputTlfilesEvidence obj)
        {
            try
            {
                _db.InputTlfilesEvidences.Add(obj);
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

        public Task Update(string id, InputTlfilesEvidence obj)
        {
            throw new NotImplementedException();
        }
    }
}
