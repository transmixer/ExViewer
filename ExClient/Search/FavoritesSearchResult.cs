﻿using ExClient.Galleries;
using HtmlAgilityPack;
using Opportunity.MvvmUniverse.AsyncHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;

namespace ExClient.Search
{
    public class FavoritesSearchResult : SearchResultBase
    {
        public static Uri SearchBaseUri => new Uri(Client.Current.Uris.RootUri, "favorites.php");

        public override Uri SearchUri { get; }

        internal static FavoritesSearchResult Search(Client client, string keyword, FavoriteCategory category)
        {
            if (category == null || category.Index < 0)
                category = FavoriteCategory.All;
            var result = new FavoritesSearchResult(client, keyword, category);
            return result;
        }

        private FavoritesSearchResult(Client client, string keyword, FavoriteCategory category)
            : base(client)
        {
            this.Keyword = keyword ?? "";
            this.Category = category;
            this.SearchUri = new Uri(SearchBaseUri, $"?{new HttpFormUrlEncodedContent(getUriQuery())}");
        }

        private IEnumerable<KeyValuePair<string, string>> getUriQuery()
        {
            //?favcat=all&f_search=&f_apply=Search+Favorites
            string cat;
            if (this.Category == null || this.Category.Index < 0)
                cat = "all";
            else
                cat = this.Category.Index.ToString();
            yield return new KeyValuePair<string, string>("favcat", cat);
            yield return new KeyValuePair<string, string>("f_search", this.Keyword);
            yield return new KeyValuePair<string, string>("f_apply", "Search Favorites");
        }

        protected override void HandleAdditionalInfo(HtmlNode trNode, Gallery gallery)
        {
            base.HandleAdditionalInfo(trNode, gallery);
            var favNode = trNode.ChildNodes[2].LastChild;
            var favNote = HtmlEntity.DeEntitize(favNode.InnerText);
            if (favNote.StartsWith("Note: "))
                gallery.FavoriteNote = favNote.Substring(6);
            else
                gallery.FavoriteNote = "";
        }

        protected override void LoadPageOverride(HtmlDocument doc)
        {
            var noselNode = doc.DocumentNode
                .Element("html")
                .Element("body")
                .Element("div")
                .Elements("div").First();
            var fpNodes = noselNode.Elements("div").Take(10);
            fpNodes.Select(n =>
            {
                var fav = n.Elements("div").First(nn => nn.GetAttributeValue("class", null) == "i");
                return this.Owner.Favorites.GetCategory(fav);
            }).ToList();
        }

        public IAsyncAction AddToCategoryAsync(IReadOnlyList<ItemIndexRange> items, FavoriteCategory categoty)
        {
            if (categoty == null)
                throw new ArgumentNullException(nameof(categoty));
            if (items == null || items.Count == 0)
                return AsyncWrapper.CreateCompleted();
            return AsyncInfo.Run(async token =>
            {
                var ddact = default(string);
                if (categoty.Index < 0)
                    ddact = "delete";
                else
                    ddact = $"fav{categoty.Index}";
                var post = Client.Current.HttpClient.PostAsync(this.SearchUri, new HttpFormUrlEncodedContent(getParameters()));
                token.Register(post.Cancel);
                var r = await post;
                if(categoty.Index<0)
                {
                    this.Reset();
                }
                else
                {
                    foreach (var range in items)
                    {
                        for (var i = range.FirstIndex; i <= range.LastIndex; i++)
                        {
                            this[i].FavoriteCategory = categoty;
                        }
                    }
                }
                IEnumerable<KeyValuePair<string,string>> getParameters()
                {
                    yield return new KeyValuePair<string, string>("apply", "Apply");
                    yield return new KeyValuePair<string, string>("ddact", ddact);
                    foreach (var range in items)
                    {
                        for (var i = range.FirstIndex; i <= range.LastIndex; i++)
                        {
                            yield return new KeyValuePair<string, string>("modifygids[]", this[i].Id.ToString());
                        }
                    }
                }
            });

        }

        public string Keyword
        {
            get;
        }

        public FavoriteCategory Category
        {
            get;
        }
    }
}