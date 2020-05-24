using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseConnection.Migrations
{
    public partial class stanislawk171978_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_OfferDetailsId_OfferDetailsId",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entries_PropertyAddressId_PropertyAddressId",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entries_PropertyDetailsId_PropertyDetailsId",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entries_PropertyFeaturesId_PropertyFeaturesId",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entries_PropertyPriceId_PropertyPriceId",
                table: "Entries");

            migrationBuilder.DropTable(
                name: "OfferDetailsId");

            migrationBuilder.DropTable(
                name: "PropertyAddressId");

            migrationBuilder.DropTable(
                name: "PropertyDetailsId");

            migrationBuilder.DropTable(
                name: "PropertyFeaturesId");

            migrationBuilder.DropTable(
                name: "PropertyPriceId");

            migrationBuilder.DropTable(
                name: "SellerContactId");

            migrationBuilder.CreateTable(
                name: "PropertyAddress",
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
                    table.PrimaryKey("PK_PropertyAddress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyDetails",
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
                    table.PrimaryKey("PK_PropertyDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyFeatures",
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
                    table.PrimaryKey("PK_PropertyFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyPrice",
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
                    table.PrimaryKey("PK_PropertyPrice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SellerContact",
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
                    table.PrimaryKey("PK_SellerContact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfferDetails",
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
                    table.PrimaryKey("PK_OfferDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferDetails_SellerContact_SellerContactId",
                        column: x => x.SellerContactId,
                        principalTable: "SellerContact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfferDetails_SellerContactId",
                table: "OfferDetails",
                column: "SellerContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_OfferDetails_OfferDetailsId",
                table: "Entries",
                column: "OfferDetailsId",
                principalTable: "OfferDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_PropertyAddress_PropertyAddressId",
                table: "Entries",
                column: "PropertyAddressId",
                principalTable: "PropertyAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_PropertyDetails_PropertyDetailsId",
                table: "Entries",
                column: "PropertyDetailsId",
                principalTable: "PropertyDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_PropertyFeatures_PropertyFeaturesId",
                table: "Entries",
                column: "PropertyFeaturesId",
                principalTable: "PropertyFeatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_PropertyPrice_PropertyPriceId",
                table: "Entries",
                column: "PropertyPriceId",
                principalTable: "PropertyPrice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_OfferDetails_OfferDetailsId",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entries_PropertyAddress_PropertyAddressId",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entries_PropertyDetails_PropertyDetailsId",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entries_PropertyFeatures_PropertyFeaturesId",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entries_PropertyPrice_PropertyPriceId",
                table: "Entries");

            migrationBuilder.DropTable(
                name: "OfferDetails");

            migrationBuilder.DropTable(
                name: "PropertyAddress");

            migrationBuilder.DropTable(
                name: "PropertyDetails");

            migrationBuilder.DropTable(
                name: "PropertyFeatures");

            migrationBuilder.DropTable(
                name: "PropertyPrice");

            migrationBuilder.DropTable(
                name: "SellerContact");

            migrationBuilder.CreateTable(
                name: "PropertyAddressId",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<int>(type: "int", nullable: false),
                    DetailedAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyAddressId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyDetailsId",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Area = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FloorNumber = table.Column<int>(type: "int", nullable: true),
                    NumberOfRooms = table.Column<int>(type: "int", nullable: false),
                    YearOfConstruction = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyDetailsId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyFeaturesId",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Balconies = table.Column<int>(type: "int", nullable: true),
                    BasementArea = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GardenArea = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IndoorParkingPlaces = table.Column<int>(type: "int", nullable: true),
                    OutdoorParkingPlaces = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyFeaturesId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyPriceId",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PricePerMeter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ResidentalRent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalGrossPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyPriceId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SellerContactId",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerContactId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfferDetailsId",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsStillValid = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OfferKind = table.Column<int>(type: "int", nullable: false),
                    SellerContactId = table.Column<int>(type: "int", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferDetailsId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferDetailsId_SellerContactId_SellerContactId",
                        column: x => x.SellerContactId,
                        principalTable: "SellerContactId",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfferDetailsId_SellerContactId",
                table: "OfferDetailsId",
                column: "SellerContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_OfferDetailsId_OfferDetailsId",
                table: "Entries",
                column: "OfferDetailsId",
                principalTable: "OfferDetailsId",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_PropertyAddressId_PropertyAddressId",
                table: "Entries",
                column: "PropertyAddressId",
                principalTable: "PropertyAddressId",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_PropertyDetailsId_PropertyDetailsId",
                table: "Entries",
                column: "PropertyDetailsId",
                principalTable: "PropertyDetailsId",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_PropertyFeaturesId_PropertyFeaturesId",
                table: "Entries",
                column: "PropertyFeaturesId",
                principalTable: "PropertyFeaturesId",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_PropertyPriceId_PropertyPriceId",
                table: "Entries",
                column: "PropertyPriceId",
                principalTable: "PropertyPriceId",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
