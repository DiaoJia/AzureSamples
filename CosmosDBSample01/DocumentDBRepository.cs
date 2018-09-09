using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace CosmosDBSample01
{
   public static class DocumentDBRepository<T> where T: class
    {
        public static readonly string DatabaseId = string.Empty;
    }
}
