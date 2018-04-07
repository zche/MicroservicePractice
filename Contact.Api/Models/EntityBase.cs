using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Contact.Api.Models
{
    /// <summary>
    ///可持久到数据库的领域模型的基类。
    /// </summary>
    //[Serializable]
    public abstract class EntityBase<TKey>
    {
        public virtual TKey ID { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string _id { get; set; }
    }
}