﻿using MINISOOQ.DataAccess.Repository.IRepository;
using MINISOOQ.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINISOOQ.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
			Favorite = new FavoriteRepository(_db);
            Images = new ImageRepository(_db);
        }
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company {  get; private set; }
        public IShoppingCartRepository ShoppingCart {  get; private set; }
        public IFavoriteRepository Favorite { get; private set; }
        public IApplicationUserRepository ApplicationUser {  get; private set; }
        public IOrderHeaderRepository OrderHeader {  get; private set; }
        public IOrderDetailRepository OrderDetail {  get; private set; }

        public IImageRepository Images { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }

		public List<Product> GetAllProducts()
		{
			return _db.Products.Include(p => p.Category).ToList();
		}
       
    }
}
