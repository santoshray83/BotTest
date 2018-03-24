using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotTest.SitecoreAPI.Models
{
    public class Item
    {
        public Guid Id { get; set; }

        public string Category { get; set; }

        public string Database { get; set; }

        public string DisplayName { get; set; }

        public string Icon { get; set; }

        public string Language { get; set; }
        public string LongID { get; set; }
        public string MediaUrl { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public string Template { get; set; }

        public Guid TemplateId { get; set; }

        public string TemplateName { get; set; }

        public string Url { get; set; }

        public int Version { get; set; }

        public Dictionary<string, Field> Fields { get; set; }
    }

    public class Field
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }
    }


    internal class ApiResult
    {
        public int StatusCode { get; set; }

        public Result Result { get; set; }
    }

    internal class Result
    {
        public int TotalCount { get; set; }

        public int ResultCount { get; set; }

        public IEnumerable<Item> Items { get; set; }
    }
}