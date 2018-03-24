using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using BotTest.SitecoreAPI.Models;
using BotTest.SitecoreAPI.Utils;
using Newtonsoft.Json;

namespace BotTest.SitecoreAPI
{
    public class SitecoreApiClient : ISitecoreClient
    {
        HttpClient _client;
        SitecoreConfiguration _configuration;
        string _database;
        string _apiKey;

        public SitecoreApiClient(string database = null)
        {
            _configuration = (SitecoreConfiguration)System.Configuration.ConfigurationManager.GetSection("sitecoreApi");
            if (_configuration != null)
            {
                _database = !String.IsNullOrEmpty(database) ? database : _configuration.Host.Database;
                _apiKey = _configuration.Host.APIKey.KeyId;
                _client = new HttpClient();
                _client.BaseAddress = new Uri(_configuration.Host.Endpoint);
                _client.DefaultRequestHeaders.Add("X-Scitemwebapi-Username", _configuration.Host.Credentials.UserName);
                _client.DefaultRequestHeaders.Add("X-Scitemwebapi-Password", _configuration.Host.Credentials.Password);
                _client.DefaultRequestHeaders.Add("X-Scitemwebapi-Encrypted", _configuration.Host.Credentials.Encrypt ? "1" : "0");
            }

        }
        /// <summary>
        /// Gets Sitecore Items by ID
        /// </summary>
        /// <param name="id">ID of the item</param>
        /// <param name="language">Optional Language of the Item</param>
        /// <returns>Sitecore Item</returns>
        public Item GetById(string id, string language = null)
        {
            string queryFormat = "/sitecore/api/ssc/aggregate/content/Items/({0})?sc_apikey={1}";

            if (!String.IsNullOrEmpty(language))
                queryFormat += "&language={2}";

            string url = String.IsNullOrEmpty(language) ? String.Format(queryFormat, id, _apiKey) : String.Format(queryFormat, id, _apiKey, language);

            var response = Get(url);

            var result = ProcessResponseMessage<ApiResult>(response);

            if (result != null && result.Result != null && result.Result.ResultCount > 0)
                return result.Result.Items.FirstOrDefault();

            return null;
        }

        /// <summary>
        /// Gets Sitecore Items by a Query
        /// </summary>
        /// <param name="query">Sitecore Fast Query</param>
        /// <param name="language">Optional Language Parameter</param>
        /// <returns>Collection of Sitecore Items</returns>
        public IEnumerable<Item> GetByQuery(string query, string language = null)
        {
            try
            {
                query = WebUtility.UrlEncode(query);
                string queryFormat = "/sitecore/api/ssc/aggregate/content/Items/?$filter={0}&sc_database={1}&sc_apikey={2}";

                if (!String.IsNullOrEmpty(language))
                    queryFormat += "&language={3}";

                string url = String.IsNullOrEmpty(language) ? String.Format(queryFormat, query, _database, _apiKey) : String.Format(queryFormat, query, _database, _apiKey, language);

                var response = Get(url);

                var result = ProcessResponseMessage<ApiResult>(response);

                if (result != null && result.Result != null && result.Result.ResultCount > 0)
                    return result.Result.Items;

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        /// <summary>
        /// Wrapper around the _client GetAsync method with additional logging
        /// </summary>
        /// <param name="url">URL for the request</param>
        /// <returns>Response from the GET method</returns>
        private HttpResponseMessage Get(string url)
        {
            var response = _client.GetAsync(url).Result;
            return response;
        }

        /// <summary>
        /// Helper Response Processing & Deserlization of HttpResponseMessage
        /// </summary>
        /// <typeparam name="T">Type of Expected Response</typeparam>
        /// <param name="response">HttpResponseMessage received from HttpClient</param>
        /// <returns>Deserialized object of Expected Response Type T</returns>
        private T ProcessResponseMessage<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                T content = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
                return content;
            }
            return default(T);
        }
    }
}