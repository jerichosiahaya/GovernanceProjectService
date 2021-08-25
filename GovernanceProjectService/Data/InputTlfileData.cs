using GovernanceProjectService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovernanceProjectService.Data
{
    public class InputTlfileData : IInputTlfile
    {
        private GesitDbContext _db;
        public InputTlfileData(GesitDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<InputTlfile>> CountExistingFileNameInputTL(string filename)
        {
            var result = await _db.InputTlfiles.Where(s => s.FileName.Contains(filename)).AsNoTracking().ToListAsync();
            return result;
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InputTlfile>> GetAll()
        {
            var result = await _db.InputTlfiles.Include(e => e.InputTlfilesEvidences).OrderByDescending(s => s.CreatedAt).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<InputTlfile> GetById(string id)
        {
            var result = await _db.InputTlfiles.Where(s => s.Id == Convert.ToInt32(id)).FirstOrDefaultAsync();
            return result;
        }

        public async Task Insert(InputTlfile obj)
        {
            try
            {
                _db.InputTlfiles.Add(obj);
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

        public async Task Update(string id, InputTlfile obj)
        {
            try
            {
                var result = await GetById(id);
                if (result != null)
                {
                    result.RhafilesId = obj.RhafilesId;
                    result.FileName = obj.FileName;
                    result.FilePath = obj.FilePath;
                    result.FileSize = obj.FileSize;
                    result.FileType = obj.FileType;
                    result.Notes = obj.Notes;
                    result.CreatedAt = obj.CreatedAt;
                    result.UpdatedAt = obj.UpdatedAt;
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

        public async Task UpdateNotes(string id, string notes)
        {
            try
            {
                var result = await GetById(id);
                if (result != null)
                {
                    result.Notes = notes;
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

    }
}
