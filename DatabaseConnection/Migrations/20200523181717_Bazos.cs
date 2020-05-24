using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseConnection.Migrations
{
    public partial class Bazos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertyAddressId",
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
                    table.PrimaryKey("PK_PropertyAddressId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyDetailsId",
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
                    table.PrimaryKey("PK_PropertyDetailsId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyFeaturesId",
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
                    table.PrimaryKey("PK_PropertyFeaturesId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyPriceId",
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
                    table.PrimaryKey("PK_PropertyPriceId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SellerContactId",
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
                    table.PrimaryKey("PK_SellerContactId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfferDetailsId",
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
                    table.PrimaryKey("PK_OfferDetailsId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferDetailsId_SellerContactId_SellerContactId",
                        column: x => x.SellerContactId,
                        principalTable: "SellerContactId",
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
                        name: "FK_Entries_OfferDetailsId_OfferDetailsId",
                        column: x => x.OfferDetailsId,
                        principalTable: "OfferDetailsId",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_PropertyAddressId_PropertyAddressId",
                        column: x => x.PropertyAddressId,
                        principalTable: "PropertyAddressId",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_PropertyDetailsId_PropertyDetailsId",
                        column: x => x.PropertyDetailsId,
                        principalTable: "PropertyDetailsId",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_PropertyFeaturesId_PropertyFeaturesId",
                        column: x => x.PropertyFeaturesId,
                        principalTable: "PropertyFeaturesId",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_PropertyPriceId_PropertyPriceId",
                        column: x => x.PropertyPriceId,
                        principalTable: "PropertyPriceId",
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
                name: "IX_OfferDetailsId_SellerContactId",
                table: "OfferDetailsId",
                column: "SellerContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries");

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
        }
    }
}
