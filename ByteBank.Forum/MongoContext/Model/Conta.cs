using AspNet.Identity.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ByteBank.Forum.MongoContext.Model
{
    public class Conta : IdentityUser
    {
        public string NomeCompleto { get; set; }
    }
}