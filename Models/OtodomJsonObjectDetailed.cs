//Klasa, która zawiera wszystkie dane w postaci obiektu typu JSON, jakie przechowuje w sobie skrypt
//pobierany z otodom. Dzięki możliwości VS, nie trzeba pisać tej klasy ręcznie. Można skopiować obiekt typu JSON
//a VS sam stworzy dla nas klasę
class OtodomJsonObjectDetailed
{ 
    public Initialprops initialProps { get; set; }
    public Appinitialprops appInitialProps { get; set; }
}

public class Initialprops
{
    public Meta meta { get; set; }
    public Data data { get; set; }
}

public class Meta
{
    public Target target { get; set; }
    public Seo seo { get; set; }
    public Extra_Params extra_params { get; set; }
    public string referer { get; set; }
    public int statusCode { get; set; }
}

public class Target
{
    public string Area { get; set; }
    public object[] AreaRange { get; set; }
    public string Build_year { get; set; }
    public string Building_floors_num { get; set; }
    public string[] Building_material { get; set; }
    public string[] Building_ownership { get; set; }
    public string[] Building_type { get; set; }
    public string City { get; set; }
    public string City_id { get; set; }
    public string[] Construction_status { get; set; }
    public string Country { get; set; }
    public string[] Extras_types { get; set; }
    public string[] Floor_no { get; set; }
    public string[] Heating { get; set; }
    public string Id { get; set; }
    public string MarketType { get; set; }
    public string OfferType { get; set; }
    public string Photo { get; set; }
    public int Price { get; set; }
    public string[] PriceRange { get; set; }
    public int Price_per_m { get; set; }
    public string ProperType { get; set; }
    public string Province { get; set; }
    public string RegularUser { get; set; }
    public string[] Rooms_num { get; set; }
    public string[] Security_types { get; set; }
    public string Subregion { get; set; }
    public string Title { get; set; }
    public string[] Windows_type { get; set; }
    public string env { get; set; }
    public string seller_id { get; set; }
    public string user_type { get; set; }
}

public class Seo
{
    public string title { get; set; }
    public string description { get; set; }
    public string robots { get; set; }
}

public class Extra_Params
{
    public Seo1 seo { get; set; }
    public Target1 target { get; set; }
}

public class Seo1
{
    public string title { get; set; }
    public string description { get; set; }
    public string robots { get; set; }
}

public class Target1
{
    public string Area { get; set; }
    public object[] AreaRange { get; set; }
    public string Build_year { get; set; }
    public string Building_floors_num { get; set; }
    public string[] Building_material { get; set; }
    public string[] Building_ownership { get; set; }
    public string[] Building_type { get; set; }
    public string City { get; set; }
    public string City_id { get; set; }
    public string[] Construction_status { get; set; }
    public string Country { get; set; }
    public string[] Extras_types { get; set; }
    public string[] Floor_no { get; set; }
    public string[] Heating { get; set; }
    public string Id { get; set; }
    public string MarketType { get; set; }
    public string OfferType { get; set; }
    public string Photo { get; set; }
    public int Price { get; set; }
    public string[] PriceRange { get; set; }
    public int Price_per_m { get; set; }
    public string ProperType { get; set; }
    public string Province { get; set; }
    public string RegularUser { get; set; }
    public string[] Rooms_num { get; set; }
    public string[] Security_types { get; set; }
    public string Subregion { get; set; }
    public string Title { get; set; }
    public string[] Windows_type { get; set; }
    public string env { get; set; }
    public string seller_id { get; set; }
    public string user_type { get; set; }
}

public class Data
{
    public Advert advert { get; set; }
    public Statistics statistics { get; set; }
    public object[] parentAds { get; set; }
}

public class Advert
{
    public int advertId { get; set; }
    public Advertowner advertOwner { get; set; }
    public string advertiser_type { get; set; }
    public string advertType { get; set; }
    public Agency agency { get; set; }
    public Areaprice areaPrice { get; set; }
    public Breadcrumb[] breadcrumb { get; set; }
    public Characteristic[] characteristics { get; set; }
    public Category category { get; set; }
    public string dateCreated { get; set; }
    public string dateModified { get; set; }
    public string description { get; set; }
    public string encryptedId { get; set; }
    public bool exclusiveOffer { get; set; }
    public string[] features { get; set; }
    public Links links { get; set; }
    public Location location { get; set; }
    public Cenatorium cenatorium { get; set; }
    public int parentId { get; set; }
    public string parentLink { get; set; }
    public string parentTitle { get; set; }
    public Photo1[] photos { get; set; }
    public Price price { get; set; }
    public string referenceId { get; set; }
    public string slug { get; set; }
    public string status { get; set; }
    public string title { get; set; }
    public string url { get; set; }
    public Userad[] userAds { get; set; }
}

public class Advertowner
{
    public string name { get; set; }
    public string type { get; set; }
    public string[] phones { get; set; }
    public Avatar avatar { get; set; }
}

public class Avatar
{
    public string baseUrl { get; set; }
}

public class Agency
{
    public string address { get; set; }
    public string code { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string[] phones { get; set; }
    public Photo photo { get; set; }
    public string type { get; set; }
    public string url { get; set; }
}

public class Photo
{
    public string baseUrl { get; set; }
}

public class Areaprice
{
    public string human_value { get; set; }
    public string label { get; set; }
    public string unit { get; set; }
    public string value { get; set; }
}

public class Category
{
    public int id { get; set; }
    public Name[] name { get; set; }
    public Parent_Name[] parent_name { get; set; }
}

public class Name
{
    public string locale { get; set; }
    public string value { get; set; }
}

public class Parent_Name
{
    public string locale { get; set; }
    public string value { get; set; }
}

public class Links
{
    public string local_plan { get; set; }
    public string video { get; set; }
    public string view3d { get; set; }
    public string walkaround { get; set; }
}

public class Location
{
    public string address { get; set; }
    public Coordinates coordinates { get; set; }
    public Geolevel[] geoLevel { get; set; }
}

public class Coordinates
{
    public float latitude { get; set; }
    public float longitude { get; set; }
    public int radius { get; set; }
    public int zoom_level { get; set; }
}

public class Geolevel
{
    public string id { get; set; }
    public string label { get; set; }
    public string type { get; set; }
}

public class Cenatorium
{
    public string price_from { get; set; }
    public string price_to { get; set; }
}

public class Price
{
    public string human_value { get; set; }
    public string label { get; set; }
    public string unit { get; set; }
    public string value { get; set; }
}

public class Breadcrumb
{
    public string label { get; set; }
    public string locative { get; set; }
    public string url { get; set; }
}

public class Characteristic
{
    public string key { get; set; }
    public string currency { get; set; }
    public string label { get; set; }
    public string value { get; set; }
    public string value_translated { get; set; }
}

public class Photo1
{
    public string thumbnail { get; set; }
    public string small { get; set; }
    public string medium { get; set; }
    public string large { get; set; }
}

public class Userad
{
    public string title { get; set; }
    public string url { get; set; }
    public string image { get; set; }
    public string rooms_num { get; set; }
    public string net_area { get; set; }
    public Price1 price { get; set; }
    public Pricepermeter pricepermeter { get; set; }
    public string area_unit { get; set; }
}

public class Price1
{
    public string value { get; set; }
    public string unit { get; set; }
}

public class Pricepermeter
{
    public string value { get; set; }
    public string unit { get; set; }
}

public class Statistics
{
    public Areaprice1[] areaPrice { get; set; }
    public Areapricetrend areaPriceTrend { get; set; }
    public Price2[] price { get; set; }
    public Pricetrend priceTrend { get; set; }
    public Reference reference { get; set; }
}

public class Areapricetrend
{
    public Series[] series { get; set; }
}

public class Series
{
    public Constraints constraints { get; set; }
    public Value value { get; set; }
}

public class Constraints
{
    public string category { get; set; }
    public string date { get; set; }
    public Geolevel1 geoLevel { get; set; }
    public string typology { get; set; }
}

public class Geolevel1
{
    public string name { get; set; }
}

public class Value
{
    public string minValue { get; set; }
    public string meanValue { get; set; }
    public string maxValue { get; set; }
    public string unit { get; set; }
}

public class Pricetrend
{
    public Series1[] series { get; set; }
}

public class Series1
{
    public Constraints1 constraints { get; set; }
    public Value1 value { get; set; }
}

public class Constraints1
{
    public string category { get; set; }
    public string date { get; set; }
    public Geolevel2 geoLevel { get; set; }
    public string typology { get; set; }
}

public class Geolevel2
{
    public string name { get; set; }
}

public class Value1
{
    public string minValue { get; set; }
    public string meanValue { get; set; }
    public string maxValue { get; set; }
    public string unit { get; set; }
}

public class Reference
{
    public Geolevel3 geoLevel { get; set; }
    public string areaPrice { get; set; }
    public string areaPriceUnit { get; set; }
    public string price { get; set; }
    public string priceUnit { get; set; }
    public string category { get; set; }
}

public class Geolevel3
{
    public string name { get; set; }
}

public class Areaprice1
{
    public Constraints2 constraints { get; set; }
    public Value2 value { get; set; }
}

public class Constraints2
{
    public string category { get; set; }
    public string date { get; set; }
    public Geolevel4 geoLevel { get; set; }
    public string typology { get; set; }
}

public class Geolevel4
{
    public string name { get; set; }
}

public class Value2
{
    public string minValue { get; set; }
    public string meanValue { get; set; }
    public string maxValue { get; set; }
    public string unit { get; set; }
}

public class Price2
{
    public Constraints3 constraints { get; set; }
    public Value3 value { get; set; }
}

public class Constraints3
{
    public string category { get; set; }
    public string date { get; set; }
    public Geolevel5 geoLevel { get; set; }
    public string typology { get; set; }
}

public class Geolevel5
{
    public string name { get; set; }
}

public class Value3
{
    public string minValue { get; set; }
    public string meanValue { get; set; }
    public string maxValue { get; set; }
    public string unit { get; set; }
}

public class Appinitialprops
{
    public Data1 data { get; set; }
}

public class Data1
{
    public Topdeveloper[] topDevelopers { get; set; }
    public Toplocation[] topLocations { get; set; }
}

public class Topdeveloper
{
    public string id { get; set; }
    public string name { get; set; }
    public string link { get; set; }
}

public class Toplocation
{
    public string id { get; set; }
    public string name { get; set; }
    public string link { get; set; }
    public string image { get; set; }
    public int numberOfProjects { get; set; }
}
