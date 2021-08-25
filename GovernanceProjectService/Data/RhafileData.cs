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

        public async Task Update(string id, Rhafile obj)
        {
            try
            {
                var result = await GetById(id);
                if (result != null)
                {
                    result.SubKondisi = obj.SubKondisi;
                    result.Kondisi = obj.Kondisi;
                    result.Rekomendasi = obj.Rekomendasi;
                    result.TindakLanjut = obj.TindakLanjut;
                    result.TargetDate = obj.TargetDate;
                    result.Assign = obj.Assign;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Data {id} not found");
                }
            }
            catch (DbUpdateException DbEx)
            {

                throw new Exception(DbEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

        public async Task<IEnumerable<Rhafile>> CountRhaDone()
        {
            
            var result = await _db.Rhafiles.Where(s => s.StatusCompleted == 1).Include(e => e.RhafilesEvidences).Include(e => e.InputTlfiles).ThenInclude(c => c.InputTlfilesEvidences).OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Rhafile>> CountRhaPending()
        {
            var result = await _db.Rhafiles.Where(s => s.StatusCompleted == 0).Include(e => e.RhafilesEvidences).Include(e => e.InputTlfiles).ThenInclude(c => c.InputTlfilesEvidences).OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Rhafile>> CountExistingFileNameRha(string filename)
        {
            var result = await _db.Rhafiles.Where(s => s.FileName.Contains(filename)).AsNoTracking().ToListAsync();
            return result;
            // TO DO
            // select * from dbo.RHAFiles where CHARINDEX('t', file_name) > 0
            //throw new NotImplementedException();
        }

        public async Task UpdateStatus(string id, int status)
        {
            try
            {
                var result = await GetById(id);
                if (result != null)
                {
                    result.StatusCompleted = status;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Data {id} not found");
                }
            }
            catch (DbUpdateException DbEx)
            {

                throw new Exception(DbEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //public Task UploadFile(IFormFile file)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
