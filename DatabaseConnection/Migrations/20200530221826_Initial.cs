using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseConnection.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferDetails_Url = table.Column<string>(nullable: true),
                    OfferDetails_CreationDateTime = table.Column<DateTime>(nullable: true),
                    OfferDetails_LastUpdateDateTime = table.Column<DateTime>(nullable: true),
                    OfferDetails_OfferKind = table.Column<int>(nullable: true),
                    OfferDetails_SellerContact_Email = table.Column<string>(nullable: true),
                    OfferDetails_SellerContact_Telephone = table.Column<string>(nullable: true),
                    OfferDetails_SellerContact_Name = table.Column<string>(nullable: true),
                    OfferDetails_IsStillValid = table.Column<bool>(nullable: true),
                    PropertyPrice_TotalGrossPrice = table.Column<decimal>(nullable: true),
                    PropertyPrice_PricePerMeter = table.Column<decimal>(nullable: true),
                    PropertyPrice_ResidentalRent = table.Column<decimal>(nullable: true),
                    PropertyPrice_NegotiablePrice = table.Column<bool>(nullable: true),
                    PropertyDetails_Area = table.Column<decimal>(nullable: true),
                    PropertyDetails_NumberOfRooms = table.Column<int>(nullable: true),
                    PropertyDetails_FloorNumber = table.Column<int>(nullable: true),
                    PropertyDetails_YearOfConstruction = table.Column<int>(nullable: true),
                    PropertyDetails_BuldingType = table.Column<string>(nullable: true),
                    PropertyDetails_NumberOfFloors = table.Column<int>(nullable: true),
                    PropertyAddress_City = table.Column<int>(nullable: true),
                    PropertyAddress_District = table.Column<string>(nullable: true),
                    PropertyAddress_StreetName = table.Column<string>(nullable: true),
                    PropertyAddress_DetailedAddress = table.Column<string>(nullable: true),
                    PropertyFeatures_GardenArea = table.Column<decimal>(nullable: true),
                    PropertyFeatures_Balconies = table.Column<int>(nullable: true),
                    PropertyFeatures_BasementArea = table.Column<decimal>(nullable: true),
                    PropertyFeatures_OutdoorParkingPlaces = table.Column<int>(nullable: true),
                    PropertyFeatures_IndoorParkingPlaces = table.Column<int>(nullable: true),
                    PropertyFeatures_HasElevator = table.Column<bool>(nullable: true),
                    PropertyFeatures_HasBalcony = table.Column<bool>(nullable: true),
                    PropertyFeatures_IsPrimaryMarket = table.Column<bool>(nullable: true),
                    PropertyFeatures_HasBasementArea = table.Column<bool>(nullable: true),
                    PropertyFeatures_ParkingPlaces = table.Column<int>(nullable: true),
                    PropertyFeatures_BalconyArea = table.Column<int>(nullable: true),
                    RawDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HeaderValue = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
