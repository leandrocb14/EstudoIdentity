using ByteBank.Forum.MongoContext.Model;
using MongoDB.Driver;

namespace ByteBank.Forum.MongoContext.DAO
{
    public class ContaDAO
    {
        private readonly IMongoCollection<Conta> _collectionConta;
        public ContaDAO()
        {
            DbContext dbContext = new DbContext();
            _collectionConta = dbContext.GetCollectionConta();
        }

        public void Adicionar(Conta pConta)
        {
            _collectionConta.InsertOne(pConta);
        }
    }
}