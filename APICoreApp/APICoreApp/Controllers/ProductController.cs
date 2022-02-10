﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APICoreApp.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace APICoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly string _connectionString;
        public ProductController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbConnectionString");
        }
        // GET: api/Product
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var result = await conn.QueryAsync<Product>("Get_Product_All", null, null, null, CommandType.StoredProcedure);
                return result;
            }
        }

        // GET: api/Product/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<Product> Get(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", id);
                var result = await conn.QueryAsync<Product>("Get_Product_By_Id", paramaters, null, null, CommandType.StoredProcedure);
                return result.Single();
            }
        }

        // POST: api/Product
        [HttpPost]
        public async Task<int> Post([FromBody] Product product)
        {
            int newId = 0;
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var paramaters = new DynamicParameters();
                paramaters.Add("@sku", product.Sku);
                paramaters.Add("@price", product.Price);
                paramaters.Add("@isActive", product.IsActive);
                paramaters.Add("@imageUrl", product.ImageUrl);
                paramaters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var result = await conn.ExecuteAsync("Create_Product", paramaters, null, null, CommandType.StoredProcedure);
                newId = paramaters.Get<int>("@id");
            }
            return newId;
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] Product product)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", id);
                paramaters.Add("@sku", product.Sku);
                paramaters.Add("@price", product.Price);
                paramaters.Add("@isActive", product.IsActive);
                paramaters.Add("@imageUrl", product.ImageUrl);
                await conn.ExecuteAsync("Update_Product", paramaters, null, null, CommandType.StoredProcedure);
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", id);
                await conn.ExecuteAsync("Delete_Product", paramaters, null, null, CommandType.StoredProcedure);
            }
        }
    }
}
