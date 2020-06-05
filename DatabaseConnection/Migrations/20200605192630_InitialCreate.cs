using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseConnection.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertiesAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<int>(nullable: false),
                    District = table.Column<string>(nullable: true),
                    StreetName = table.Column<string>(nullable: true),
                    DetailedAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertiesAddresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertiesDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Area = table.Column<decimal>(nullable: false),
                    NumberOfRooms = table.Column<int>(nullable: false),
                    FloorNumber = table.Column<int>(nullable: true),
                    YearOfConstruction = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertiesDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertiesFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GardenArea = table.Column<decimal>(nullable: true),
                    Balconies = table.Column<int>(nullable: true),
                    BasementArea = table.Column<decimal>(nullable: true),
                    OutdoorParkingPlaces = table.Column<int>(nullable: true),
                    IndoorParkingPlaces = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertiesFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertiesPrices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalGrossPrice = table.Column<decimal>(nullable: false),
                    PricePerMeter = table.Column<decimal>(nullable: false),
                    ResidentalRent = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertiesPrices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SellersContacts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: true),
                    Telephone = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellersContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OffersDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(nullable: true),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateDateTime = table.Column<DateTime>(nullable: true),
                    OfferKind = table.Column<int>(nullable: false),
                    SellerContactId = table.Column<int>(nullable: true),
                    IsStillValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffersDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OffersDetails_SellersContacts_SellerContactId",
                        column: x => x.SellerContactId,
                        principalTable: "SellersContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferDetailsId = table.Column<int>(nullable: true),
                    PropertyPriceId = table.Column<int>(nullable: true),
                    PropertyDetailsId = table.Column<int>(nullable: true),
                    PropertyAddressId = table.Column<int>(nullable: true),
                    PropertyFeaturesId = table.Column<int>(nullable: true),
                    RawDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entries_OffersDetails_OfferDetailsId",
                        column: x => x.OfferDetailsId,
                        principalTable: "OffersDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_PropertiesAddresses_PropertyAddressId",
                        column: x => x.PropertyAddressId,
                        principalTable: "PropertiesAddresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_PropertiesDetails_PropertyDetailsId",
                        column: x => x.PropertyDetailsId,
                        principalTable: "PropertiesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_PropertiesFeatures_PropertyFeaturesId",
                        column: x => x.PropertyFeaturesId,
                        principalTable: "PropertiesFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_PropertiesPrices_PropertyPriceId",
                        column: x => x.PropertyPriceId,
                        principalTable: "PropertiesPrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entries_OfferDetailsId",
                table: "Entries",
                column: "OfferDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_PropertyAddressId",
                table: "Entries",
                column: "PropertyAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_PropertyDetailsId",
                table: "Entries",
                column: "PropertyDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_PropertyFeaturesId",
                table: "Entries",
                column: "PropertyFeaturesId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_PropertyPriceId",
                table: "Entries",
                column: "PropertyPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_OffersDetails_SellerContactId",
                table: "OffersDetails",
                column: "SellerContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "OffersDetails");

            migrationBuilder.DropTable(
                name: "PropertiesAddresses");

            migrationBuilder.DropTable(
                name: "PropertiesDetails");

            migrationBuilder.DropTable(
                name: "PropertiesFeatures");

            migrationBuilder.DropTable(
                name: "PropertiesPrices");

            migrationBuilder.DropTable(
                name: "SellersContacts");
        }
    }
}
