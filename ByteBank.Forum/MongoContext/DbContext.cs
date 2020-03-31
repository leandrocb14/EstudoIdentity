using AspNet.Identity.MongoDB;
using ByteBank.Forum.MongoContext.Model;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AspNet.Identity.MongoDB;

namespace ByteBank.Forum.MongoContext
{
    public class DbContext
    {
        private const string _connectionString = "mongodb://localhost:27017";
        private const string _dataBaseName = "Identity";
        private const string _contaCollection = "Conta";
        private const string _identityRoleCollection = "IdentityRole";

        private readonly IMongoDatabase _mongoDatabase;
        public DbContext()
        {
            IMongoClient _mongoCliente = new MongoClient(_connectionString);
            _mongoDatabase = _mongoCliente.GetDatabase(_dataBaseName);
        }

        public IMongoCollection<Conta> GetCollectionConta()
        {
            return _mongoDatabase.GetCollection<Conta>(_contaCollection);
        }

        public IMongoCollection<IdentityRole> GetCollectionIdentityRole()
        {
            return _mongoDatabase.GetCollection<IdentityRole>(_identityRoleCollection);
        }
    }
}