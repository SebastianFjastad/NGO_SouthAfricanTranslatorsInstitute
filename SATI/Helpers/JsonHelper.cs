using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SATI.Helpers
{
    public static class JsonHelper
    {
        public static MvcHtmlString JsonFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var value = expression.Compile()(html.ViewData.Model);
            return MvcHtmlString.Create(JsonConvert.SerializeObject(value, Formatting.None, SerializerSettings));
        }

        private static JsonSerializerSettings SerializerSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };
            }
        }
    }
}