using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using EVERFI.Foundry.Classes;
using System.Linq;

namespace EVERFI.Foundry
{
    public partial class API
    {
       
        public List<Label> CreateCategoryLabelList(IRestResponse response)
        {

            CategoryLabels categorylabelslist = JsonConvert.DeserializeObject<CategoryLabels>(response.Content);
            List<LabelInformation> labelsIncluded = categorylabelslist.AllLabels;
            List<Label> catLabels = new List<Label>();
            if (labelsIncluded != null && labelsIncluded.Any())
            {
                foreach (var categoryLabel in labelsIncluded)
                {
                    Label l = new Label();
                    l.Name = categoryLabel.Attributes.CategoryLabelName;
                    l.CategoryName = categoryLabel.Attributes.CategoryName;
                    l.CategoryId = categoryLabel.Attributes.CategoryId;
                    l.Id = categoryLabel.CategoryLabelId;
                    l.UserCount = categoryLabel.Attributes.UserCount;
                    catLabels.Add(l);

                }
            }

            return catLabels;

        }
       
        public Category AddCategory(Category MyCategory)
        {
            Console.WriteLine("Adding category " + MyCategory.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/categories", Method.POST); //TODO
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.CategoryJson(MyCategory), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response);

            CategoryData categoryData = JsonConvert.DeserializeObject<CategoryData>(response.Content);
            Console.WriteLine("Category successfully added.");

            return categoryData.Data;
        }

        public Category GetCategoryById(string CategoryId) // Should we always return with List<Label>?
        {
            Console.WriteLine("Getting category " + CategoryId + "...");

            RestRequest request = new RestRequest("/{version}/admin/categories/{id}", Method.GET); //TODO
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", CategoryId, ParameterType.UrlSegment);
            
            request.AddParameter("include", "category_labels", ParameterType.QueryString);
            
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response);
         
            CategoryData categoryData = JsonConvert.DeserializeObject<CategoryData>(response.Content);

            Category category = categoryData.Data;
            var catLabels = CreateCategoryLabelList(response);
            category.Labels = catLabels;
            category.ConfigureCategory();
            
            return category;
        }

        public List<Category> GetCategories()
        {
            Console.WriteLine("Getting categories...");

            RestRequest request = new RestRequest("/{version}/admin/categories", Method.GET); //TODO
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
          
           
            request.AddParameter("include", "category_labels", ParameterType.QueryString);
            
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response);
            //CategoryLabelsIncludedList categorylabelslist = JsonConvert.DeserializeObject<CategoryLabelsIncludedList>(response.Content);
            CategoryListData categoryData = JsonConvert.DeserializeObject<CategoryListData>(response.Content);
           
            List<Category> categories = new List<Category>();

            foreach (Category category in categoryData.Data)
            {
                var catLabels = CreateCategoryLabelList(response);
                category.Labels = new List<Label>();
                if (catLabels != null && catLabels.Any())
                {
                    foreach (Label lab in catLabels)
                    {

                        List<CategoryLabelsIncluded> labels = category.Relationships.CategoryLabels.Label;
                            foreach (CategoryLabelsIncluded labelInfo in labels)
                            {
                                if (labelInfo.CategoryLabelId == lab.Id)
                                {
                                    category.Labels.Add(lab);

                                }
                            }

                        }
                    }

                category.ConfigureCategory();

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
            checkResponseSuccess(response);
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
            checkResponseSuccess(response);
            Console.WriteLine("Category successfully deleted.");

            return response.Content;
        }
    }
}
