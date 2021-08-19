using GovernanceProjectService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovernanceProjectService.Data
{
    public class RhafileData : IRhafile
    {
        private GesitDbContext _db;
        public RhafileData(GesitDbContext db)
        {
            _db = db;
        }
        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Rhafile> GetById(string id)
        {
            var result = await _db.Rhafiles.Where(s => s.Id == Convert.ToInt32(id)).FirstOrDefaultAsync();
            return result;
        }

        public async Task Insert(Rhafile obj)
        {
            try
            {
                _db.Rhafiles.Add(obj);
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

        public Task Update(string id, Rhafile obj)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Rhafile>> GetAll()
        {
            var result = await _db.Rhafiles.Include(e => e.RhafilesEvidences).Include(e => e.InputTlfiles).ThenInclude(c=> c.InputTlfilesEvidences).OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Rhafile>> GetByNPP(string npp)
        {
            var result = await _db.Rhafiles.Where(s => s.Assign == npp).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Rhafile>> CountRha()
        {
            var result = await _db.Rhafiles.AsNoTracking().ToListAsync();
            return result;
        }

        //public Task UploadFile(IFormFile file)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
