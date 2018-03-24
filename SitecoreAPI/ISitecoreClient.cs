using BotTest.SitecoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotTest.SitecoreAPI
{
    public interface ISitecoreClient
    {
        Item GetById(string id, string language = null);

        IEnumerable<Item> GetByQuery(string query, string language = null);
    }
}