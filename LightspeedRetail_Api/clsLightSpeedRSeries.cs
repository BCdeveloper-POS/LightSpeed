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
        private readonly int StoreId;
        private readonly decimal tax;
        private readonly string BaseUrl;
        private readonly string ClientId;
        private readonly string ClientSecret;
        private readonly string RefreshToken;
        private readonly int AccountID;
        public clsLightSpeedRSeries(int _StoreId, decimal _tax, string _BaseUrl, string _ClientId, string _ClientSecret, int _AccountID, string _RefreshToken)
        {
            StoreId = _StoreId;
            tax = _tax;
            BaseUrl = _BaseUrl;
            ClientId = _ClientId;
            ClientSecret = _ClientSecret;
            RefreshToken = _RefreshToken;
            AccountID = _AccountID;
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


                Parsing(item, StoreId, tax);
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
                // refresh_token = "def502003587cc6928deb79fabe69f1efd6f4cf20b50139fd0ad96a116b8e179b8eff997b5b987cf1157a18cbf432e8421ff5c4ebbc5e3554b6b78f17e00a6982582765d2d0152d8525d8cd9c70aca8109bceab222b3bdc108ba32f01218f74b41f73db182579e90a43f21d2388c8f81d7f2b97cedfeefff0a7489337a4b6d623661f7fee44f2192016a1abb4580d750e4030c71d4c1f8b1165aea6ca2b0593e84c0ccfcae58ab1a18168d7b027e9c6812a4299484202ba2931a1e64d4e5dd2cb80daf008c02b3050194e0db802231e1ad5d677a754a59f0790d42ca3f5c349d709732f1a212e36dd8944dc29c906e4e9df6b638d0a8a3537f94d7c2216d11649d2251a3d0851faa7e8f6e0080c1a45253c1ef31878f18bbf087748578e050af1399fb8b38e5dedd240e4a1b451f12be943c8c4cb3e6c4c31cae04c7086127d7f831ada5f519961692fa317be58a5437a574cbde26463ea4dfba1930b5e5521c61e7851c2f3ea2fd2ca538bc4a1e3f99e43bd245fc4114fcfaf1e076037fd6e0f01fa34e758bdc894812442ca20241072453e7ebae84c5681162e224f280b6b4eaa195e90e117a0b3b15d98293631f41d291653db8922dcc"

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

                        //comment later 
                        // File.WriteAllText($"{StoreId}_Product_Page_{pageNo + 1}.json", response.Content); // comment Later 

                        //string parentDirectory = Directory.GetParent(BaseDirectory).FullName;
                        /*string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                          string filePath = Path.Combine(currentDirectory, StoreId + "_product.json");
                          File.WriteAllText(filePath, response.Content);*/

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

                  //  File.WriteAllText($"{StoreId}_Product_Full.json", JsonConvert.SerializeObject(allItems, Formatting.Indented)); // comment Later 

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
        public void Parsing(List<ClsLightProductList.Item> ItemResult, int storeid, decimal tax)
        {
            List<ClsLightProductList.LightProductModel> prodList = new List<ClsLightProductList.LightProductModel>();
            List<ClsLightProductList.LightFullnameModel> fullNameList = new List<ClsLightProductList.LightFullnameModel>();
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
                    prod.Qty = data.ItemShops?.ItemShop?.FirstOrDefault() != null ? Convert.ToInt32(data.ItemShops.ItemShop.First().qoh) : 0;
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
