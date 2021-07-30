using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using EVERFI.Foundry.Classes;

namespace EVERFI.Foundry
{
    public partial class API
    {
        public Category AddCategory(Category MyCategory)
        {
            Console.WriteLine("Adding category " + MyCategory.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/categories", Method.POST); //TODO
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.CategoryJson(MyCategory), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 201)
            {
                throw new FoundryException(numericCode, response.Content);
            }

            CategoryData categoryData = JsonConvert.DeserializeObject<CategoryData>(response.Content);
            Console.WriteLine("Category successfully added.");

            return categoryData.Data;
        }

        public Category GetCategoryById(string CategoryId, bool WithLabels) // Should we always return with List<Label>?
        {
            Console.WriteLine("Getting category " + CategoryId + "...");

            RestRequest request = new RestRequest("/{version}/admin/categories/{id}", Method.GET); //TODO
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", CategoryId, ParameterType.UrlSegment);
            if (WithLabels)
            {
                request.AddParameter("include", "category_labels", ParameterType.QueryString);
            }
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(numericCode, response.Content);
            }

            CategoryData categoryData = JsonConvert.DeserializeObject<CategoryData>(response.Content);

            Category category = categoryData.Data;

            category.ConfigureCategory();
            if (WithLabels)
            {
                for (int i = 0; i < category.Labels.Count; i++)
                {
                    category.Labels[i] = GetLabelById(category.Labels[i].Id);
                }
            }
            /*else
            {
                category.Labels.Clear();
            }*/

            return category;
        }

        public List<Category> GetCategories(bool WithLabels)
        {
            Console.WriteLine("Getting categories...");

            RestRequest request = new RestRequest("/{version}/admin/categories", Method.GET); //TODO
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            if (WithLabels)
            {
                request.AddParameter("include", "category_labels", ParameterType.QueryString);
            }
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(numericCode, response.Content);
            }

            CategoryListData categoryData = JsonConvert.DeserializeObject<CategoryListData>(response.Content);
            List<Category> categories = new List<Category>();

            foreach (Category category in categoryData.Data)
            {
                category.ConfigureCategory();
                if (WithLabels)
                {
                    for (int i = 0; i < category.Labels.Count; i++)
                    {
                        category.Labels[i] = GetLabelById(category.Labels[i].Id);
                    }
                }
                /*else
                {
                    category.Labels.Clear();
                }*/
                categories.Add(category);
            }
            return categories;
        }

        public Category UpdateCategory(Category MyCategory)
        {
            Console.WriteLine("Updating category...");

            RestRequest request = new RestRequest("/{version}/admin/categories/{id}", Method.PATCH);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", MyCategory.Id, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.CategoryJson(MyCategory), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(numericCode, response.Content);
            }

            CategoryData categoryData = JsonConvert.DeserializeObject<CategoryData>(response.Content);
            Console.WriteLine("Category successfully updated.");
            Category category = categoryData.Data;

            category.ConfigureCategory();

            return category;
        }

        public string DeleteCategory(Category MyCategory)
        {
            Console.WriteLine("Deleting category " + MyCategory.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/categories/{id}", Method.DELETE);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", MyCategory.Id, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 204)
            {
                throw new FoundryException(numericCode, response.Content);
            }

            Console.WriteLine("Category successfully deleted.");

            return response.Content;
        }
    }
}
