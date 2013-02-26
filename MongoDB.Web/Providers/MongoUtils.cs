using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;

namespace MongoDB.Web.Providers
{
    public static class MongoUtils
    {
        public static Boolean TryCreateBsonValue(object o, out BsonValue result)
        {
            var success = false;

            try 
            {
                result = BsonValue.Create(o);
                success = true;
            }
            catch (ArgumentException e)
            {
                result = null;
            }

            return success;
        }
    }
}
