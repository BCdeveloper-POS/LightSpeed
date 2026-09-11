using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using static LightspeedRetail_Api.clsLighspeedRetailV3;
using static LightspeedRetail_Api.clsLightSppedV3.clsLightProductList;

namespace LightspeedRetail_Api
{
    class clsLightSpeedRSeries
    {
        string BaseDirectory = ConfigurationManager.AppSettings["BaseDirectory"];
        string DeveloperId = ConfigurationManager.AppSettings["DeveloperId"];
        string showonline = ConfigurationManager.AppSettings["showonline"];
        string OnlinePricing = ConfigurationManager.AppSettings["Onlineprice"];
        private readonly int StoreId;
        private readonly decimal tax;
        private readonly string BaseUrl;
        private readonly string ClientId;
        private readonly string ClientSecret;
        private readonly string RefreshToken;
        private readonly int AccountID;
        private readonly string shopID;
        public clsLightSpeedRSeries(int _StoreId, decimal _tax, string _BaseUrl, string _ClientId, string _ClientSecret, int _AccountID, string _RefreshToken , string  _shopID )
        {
            StoreId = _StoreId;
            tax = _tax;
            BaseUrl = _BaseUrl;
            ClientId = _ClientId;
            ClientSecret = _ClientSecret;
            RefreshToken = _RefreshToken;
            AccountID = _AccountID;
            shopID = _shopID;
            Console.WriteLine("Generating Lightspeed " + StoreId + " Product File....");
            Console.WriteLine("Generating Lightspeed " + StoreId + " Fullname File....");
        }
        public async Task RunAsync()
        {
            try
            {
                string[] array = LG_RefreshToken(BaseUrl, ClientId, ClientSecret, RefreshToken);
                List<ClsLightProductList.Item> item = await LightspeedSetting(BaseUrl, ClientId, ClientSecret, array[0], AccountID);

                //For only Testing, Comment this
              //  string token = "def5020019d8a45912418f56ee8257fd50ac465da81bc4beb2e9e449da11c79dd6ec2ffbf43886ffc8b61b0d19c65196a80e1643d0d4833da3968ea6504227177fd847f95f55986f31d6426a9b0fe3f32d8f6efb4d36f0bc2938bd3ba48be6d166a43d041f578b6601b51a290f763fdb7438e442a47ef7af63343fceee1bae7d2a8fac88958da75fb7a726b6acd3b21633ec9d790b41a92c61f46af65501beb4d5b885f2134252c0411112e239a9495ce954027a7d6e1b166aee02b12ee19a386f3de3abad7487ed6729d530bb11ae5d889a03844a32b10d8ab4fecce31af2e870889c30b8e933d29017b1c62b91d837e14ed77dfa31ba255cf66ad0d188f2d193996530dff2d0a8b49050b6c05b73b7234e8795df5f3711ce6889a8e148b926e6479e242fc1179e43410194d8fdd1ce73bbc8907a3875a7fbd2ad5b062091207805b0af00161341fb074adfe9a54b25bed2cdef0f4b0b1b25e0ec02cf2b11612c07a9eb9640865711cd92ff7c623f8f6bb399b80d1dad4a838bc9591aea7812b54e718451c7c7393a4195d20bc4419559dcdc9eca015afcf61671a5f01947dbf9aaef8a5640dc06b10ea4fc1c30ecc8a6d50c6a90b87b4a";
               // List<ClsLightProductList.Item> item = await LightspeedSetting(BaseUrl, ClientId, ClientSecret, token, AccountID);


              //  Parsing(item, StoreId, tax);
                Parsing(item, StoreId, tax, shopID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public string[] LG_RefreshToken(string BaseUrl, string ClientId, string ClientSecret, string refreshtoken)
        {
            string[] token_info = new string[2];
            string accessToken = "";
            BaseUrl = "https://cloud.lightspeedapp.com/auth/oauth/token";
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("");
            request.AddHeader("Content-Type", "application/json");
            var body = new
            {
                client_id = ClientId,
                client_secret = ClientSecret,
                grant_type = "refresh_token",
                refresh_token = refreshtoken
                // refresh_token = "def50200f3e6f8b1ca7f9d5a11de632a2df453accda1177d1d92b9d2b2a716cff99db14faacb06291cb1f2bb0070773b9709cffe0d5afb01dc589b31abb0baeb82407c0ae85f256b7f588d27d170dbb0f8cd1bf0e8aa3fb8f645f306bccb44650ac9de8528434fcdad48e6fcea9d1f2b614738b069c9a17d2649b8709d36014b4578b0ae85c496a08cf5f54d64cdc70b31a369b5b5ef83e56370ea8aaba02c8c959329d855c8a939a4e8a47778546fe9b6dd5fdbeceb52c3635f94340aa37bdbf22da2fc5832d5304a5e18a7415157043e771a4cba132724a3175f1ed73ce245b259c9d7f4eeb5217c640d804b0cdb9afd414e1425b005ea20c15c8c6c17478502c8c9a97c162fb7a21a06b74aa36ad6b5c86713d8ee4821836d0bc61ad9b551247d7bea3a9f353704635819d544645808cf9f8c2d5f2dfd1d48daa16bdcc06b4122bf93cc4d925c2bbb3e1ffc2cc908700eea87660eeef2b5e49d08c60dd35ebbf3858dcdc01a37d43100c83535af4ca5584f60704c21c14576360ea2db848db7593cebbd128ab7f68a3c20c6049316e64971acfaeb6eaa2eb3f29a4494778edeb60f51ac636342e0bca59a28a64b35e3097c9d7f23"
            };
            string jsonBody = JsonConvert.SerializeObject(body);
            request.AddStringBody(jsonBody, DataFormat.Json);
            string requestJson = JsonConvert.SerializeObject(body);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var response = client.Execute(request, Method.Post);
            string responseContent = response.Content;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dynamic data = JsonConvert.DeserializeObject(responseContent);
                accessToken = data.access_token;
                refreshtoken = data.refresh_token;

                token_info[0] = accessToken;
                token_info[1] = refreshtoken;

                try
                {
                    List<SqlParameter> parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@StoreId", StoreId),
                        new SqlParameter("@AccessToken", accessToken),
                        new SqlParameter("@refresh_token", refreshtoken)
                    };

                    DatabaseObject db = new DatabaseObject();
                    db.GetDataTable("usp_bc_LightSpeedAccessTokenInsert", parameters);

                }
                catch { }
            }
            return token_info;
        }

        public async Task<List<ClsLightProductList.Item>> LightspeedSetting(string BaseUrl, string ClientId, string ClientSecret, string accesstoken, int AccountID)
        {
            string Url = "";
            int recordsTotal = 100;
            List<ClsLightProductList.Item> allItems = new List<ClsLightProductList.Item>();

            if (!string.IsNullOrEmpty(accesstoken))
            {
                BaseUrl = "https://api.lightspeedapp.com/API/Account/" + AccountID + "/";

                try
                {
                    for (int pageNo = 0; pageNo <= recordsTotal - 100; pageNo++)
                    {
                        string shops = "load_relations=[\"ItemShops\",\"Category\"]";
                        string ApiUrl = BaseUrl + "Item.json" + "?" + shops;
                        ApiUrl = string.IsNullOrEmpty(Url) ? ApiUrl : Url;

                        var client = new RestClient(ApiUrl);
                        var request = new RestRequest("");
                        request.AddHeader("Authorization", "Bearer " + accesstoken);
                        request.AddHeader("cache-control", "no-cache");
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        var response = await client.ExecuteAsync(request, Method.Get);

                      

                        // File.WriteAllText($"{StoreId}_Product_Page_{pageNo + 1}.json", response.Content); // comment Later 



                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var itemList = JsonConvert.DeserializeObject<ClsLightProductList.ItemList>(response.Content);
                            if (itemList != null && itemList.items != null)
                            {
                                allItems.AddRange(itemList.items);

                                string lastItemId = itemList.items.LastOrDefault()?.itemID.ToString();
                                if (!string.IsNullOrEmpty(lastItemId))
                                {
                                    Url = BaseUrl + $"Item.json?orderby=itemID&itemID=%3E%2C{lastItemId}&" + shops;
                                }
                                recordsTotal = Convert.ToInt32(itemList.attributes.count);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error fetching data: " + response.StatusCode);
                        }
                    }

                  // File.WriteAllText($"{StoreId}_Product_Full.json", JsonConvert.SerializeObject(allItems, Formatting.Indented)); // comment Later 

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + " LightspeedRetailAPI ");
                }

                return allItems;
            }
            else
            {
                Console.WriteLine("Refresh Token Expired", StoreId);
            }


            return new List<ClsLightProductList.Item>(); 
        }
        public void Parsing(List<ClsLightProductList.Item> ItemResult, int storeid, decimal tax, string shopID)
        {
            List<ClsLightProductList.LightProductModel> prodList = new List<ClsLightProductList.LightProductModel>();
            List<ClsLightProductList.LightFullnameModel> fullNameList = new List<ClsLightProductList.LightFullnameModel>();
            
            // Added: shopID comes from POSSettings and identifies which shop's inventory row belongs
            // to this store. Parsed once here rather than on every item.
            int storeShopId = -1;
            bool hasShopId = !string.IsNullOrEmpty(shopID) && int.TryParse(shopID, out storeShopId);
            if (ItemResult.Count > 0)
            {
                foreach (var data in ItemResult)
                {
                    ClsLightProductList.LightProductModel prod = new ClsLightProductList.LightProductModel();
                    ClsLightProductList.LightFullnameModel fullName = new ClsLightProductList.LightFullnameModel();
                    if (showonline.Contains(storeid.ToString()) && !data.publishToEcom)
                    {
                        continue;
                    }
                    prod.StoreID = storeid;
                    if (string.IsNullOrEmpty(data.upc))
                        prod.upc = "#" + data.systemSku;
                    else
                        prod.upc = "#" + data.upc;
                    fullName.upc = prod.upc;

                    prod.sku = "#" + data.systemSku;
                    fullName.sku = prod.sku;

                    //  prod.Qty = data.ItemShops?.ItemShop?.FirstOrDefault() != null ? Convert.ToInt32(data.ItemShops.ItemShop.First().qoh) : 0;

                    if (hasShopId)
                    {
                        var shopStock = data.ItemShops?.ItemShop?.FirstOrDefault(s => s.shopID == storeShopId);
                        prod.Qty = shopStock != null ? Convert.ToInt32(shopStock.qoh) : 0;
                    }
                    else
                    {
                        prod.Qty = data.ItemShops?.ItemShop?.FirstOrDefault() != null ? Convert.ToInt32(data.ItemShops.ItemShop.First().qoh) : 0;
                    }
                    prod.pack = 1;
                    fullName.pack = prod.pack;
                    prod.uom = "";
                    fullName.uom = prod.uom;
                    prod.StoreProductName = data.description;
                    prod.StoreDescription = prod.StoreProductName;
                    fullName.pname = prod.StoreProductName;
                    fullName.pdesc = prod.StoreProductName;
                    prod.Price = Convert.ToDecimal(data.Prices.ItemPrice[0].amount);
                    prod.sprice = 0;
                    fullName.Price = prod.Price;
                    if (OnlinePricing.Contains(storeid.ToString()))
                    {
                        var onlinePrice = data.Prices.ItemPrice
                            .FirstOrDefault(p => p.useType == "Online");

                        if (onlinePrice != null)
                        {
                            prod.Price = onlinePrice.amount;
                        }
                    }

                    prod.tax = tax;
                    if (!string.IsNullOrEmpty(data.category?.fullPathName))
                    {
                        var categories = data.category.fullPathName.Split('/');

                        fullName.pcat = categories.Length > 0 ? categories[0] : "";
                        fullName.pcat1 = categories.Length > 1 ? categories[1] : "";
                        fullName.pcat2 = categories.Length > 2 ? categories[2] : "";
                    }
                    else
                    {
                        fullName.pcat = fullName.pcat1 = fullName.pcat2 = "";
                    }
                    
                    if (prod.Price > 0 && prod.upc.Length > 2)
                    {
                        prodList.Add(prod);
                        fullNameList.Add(fullName);
                    }

                }
                if (prodList.Count > 0 && fullNameList.Count > 0)
                {
                    DataTable dtproduct = ToDataTable(prodList);
                    DataTable dtfullname = ToDataTable(fullNameList);
                    Console.WriteLine("Generating CSV Files");
                    string product = GenerateCSV.GenerateCSVFile(dtproduct, "PRODUCT", storeid, BaseDirectory);
                    string fullname = GenerateCSV.GenerateCSVFile(dtfullname, "FULLNAME", storeid, BaseDirectory);
                    Console.WriteLine("Product FIle Generated For LightspeedRetailPos " + storeid);
                    Console.WriteLine("Fullname FIle Generated For LightspeedRetailPos " + storeid);
                }
                else
                {
                    Console.WriteLine("Files not generated, No products in the ProductList " + storeid);
                }
            }
            else
            {
                Console.WriteLine("No Products Found");
            }
        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable table = new DataTable(typeof(T).Name);
            var propList = typeof(T).GetProperties();

            foreach (var prop in propList)
            {
                Type colType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                table.Columns.Add(prop.Name, colType);
            }

            foreach (var item in items)
            {
                var values = new object[propList.Length];
                for (int i = 0; i < propList.Length; i++)
                {
                    values[i] = propList[i].GetValue(item, null);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
    public class ClsLightProductList
    {
        internal class Root
        {
            public object @attributes { get; set; }
            public List<Item> Items { get; set; }
            public string access_token { get; set; }
        }
        public class Item
        {
            public int itemID { get; set; }
            public string systemSku { get; set; }
            public float defaultCost { get; set; }
            public float avgCost { get; set; }
            public bool discountable { get; set; }
            public bool archived { get; set; }
            public string itemType { get; set; }
            public bool serialized { get; set; }
            public string description { get; set; }
            public int modelYear { get; set; }
            public string upc { get; set; }
            public string ean { get; set; }
            public string customSku { get; set; }
            public string manufacturerSku { get; set; }
            public DateTime createTime { get; set; }
            public DateTime timeStamp { get; set; }
            public bool publishToEcom { get; set; }
            public int categoryID { get; set; }
            public int taxClassID { get; set; }
            public int departmentID { get; set; }
            public int itemMatrixID { get; set; }
            public int manufacturerID { get; set; }
            public int seasonID { get; set; }
            public int defaultVendorID { get; set; }
            public ItemShops ItemShops { get; set; }
            public Prices Prices { get; set; }
            public Category category { get; set; }
            public int catvID { get; set; }
            public string catname { get; set; }
        }
        public class ItemShops
        {
            public List<ItemShop> ItemShop { get; set; }
            public int itemShopID { get; set; }
            public string qoh { get; set; }
            public int sellable { get; set; }
            public int backorder { get; set; }
            public int componentQoh { get; set; }
            public int componentBackorder { get; set; }
            public int reorderPoint { get; set; }
            public int reorderLevel { get; set; }
            public DateTime timeStamp { get; set; }
            public int itemID { get; set; }
            public int shopID { get; set; }
        }
        public class ItemShop
        {
            //public object ItemShop { get; set; }
            public int itemShopID { get; set; }
            public int qoh { get; set; }
            public int sellable { get; set; }
            public int backorder { get; set; }
            public int componentQoh { get; set; }
            public int componentBackorder { get; set; }
            public int reorderPoint { get; set; }
            public int reorderLevel { get; set; }
            public DateTime timeStamp { get; set; }
            public int itemID { get; set; }
            public int shopID { get; set; }
        }
        public class Prices
        {
            public List<ItemPrice> ItemPrice { get; set; }
            // public decimal amount { get; set; }
            public string useTypeID { get; set; }
            public string useType { get; set; }
        }
        public class ItemPrice
        {
            // public object ItemPrice { get; set; }
            public decimal amount { get; set; }
            public string useTypeID { get; set; }
            public string useType { get; set; }
        }
        public class Result
        {
            public string Response { get; set; }
            public string Url { get; set; }
        }
        public class attributes
        {
            //public string @attributes { get; set; }
            public int count { get; set; }

            public string offset { get; set; }
            public string limit { get; set; }
        }
        public class Category
        {
            public int categoryID { get; set; }
            public string name { get; set; }
            public int nodeDepth { get; set; }
            public string fullPathName { get; set; }
            public int leftNode { get; set; }
            public int rightNode { get; set; }
            public int parentID { get; set; }
            public DateTime createTime { get; set; }
            public DateTime timeStamp { get; set; }
        }

        public class LightFullnameModel
        {
            public string pname { get; set; }
            public string pdesc { get; set; }
            public string upc { get; set; }
            public string sku { get; set; }
            public decimal Price { get; set; }
            public string uom { get; set; }
            public int pack { get; set; }
            public string pcat { get; set; }
            public string pcat1 { get; set; }
            public string pcat2 { get; set; }
            public string country { get; set; }
            public string region { get; set; }
        }
        public class LightProductModel
        {
            public int StoreID { get; set; }
            public string upc { get; set; }
            public int Qty { get; set; }
            public string sku { get; set; }
            public int pack { get; set; }
            public string uom { get; set; }
            public string StoreProductName { get; set; }
            public string StoreDescription { get; set; }
            public decimal Price { get; set; }
            public decimal sprice { get; set; }
            public string Start { get; set; }
            public string End { get; set; }
            public decimal tax { get; set; }
            public string altupc1 { get; set; }
            public string altupc2 { get; set; }
            public string altupc3 { get; set; }
            public string altupc4 { get; set; }
            public string altupc5 { get; set; }
            // public int catID { get; set; }
        }

        public class Refresh
        {
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string access_token { get; set; }
            public string refresh_token { get; set; }
        }
        public class ItemList
        {
            [JsonProperty("@attributes")]
            public attributes attributes { get; set; }
            [JsonProperty("Item")]
            public List<Item> items { get; set; }
        }
        public class Item1
        {
            public string systemSku { get; set; }
            public string description { get; set; }
            public string upc { get; set; }
            public string discountable { get; set; }

        }
    }
}
