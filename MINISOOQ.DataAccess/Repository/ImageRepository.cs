using MINISOOQ.DataAccess.Repository.IRepository;
using MINISOOQ.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINISOOQ.DataAccess.Repository
{
    public class ImageRepository : Repository<Images>, IImageRepository
    {
        private readonly ApplicationDbContext _db;
        public ImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
