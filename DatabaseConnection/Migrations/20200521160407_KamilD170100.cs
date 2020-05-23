using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseConnection.Migrations
{
    public partial class KamilD170100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertyAddressDb",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<int>(nullable: false),
                    District = table.Column<string>(nullable: true),
                    StreetName = table.Column<string>(nullable: true),
                    DetailedAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyAddressDb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyDetailsDb",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Area = table.Column<decimal>(nullable: false),
                    NumberOfRooms = table.Column<int>(nullable: false),
                    FloorNumber = table.Column<int>(nullable: true),
                    YearOfConstruction = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyDetailsDb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyFeaturesDb",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GardenArea = table.Column<decimal>(nullable: true),
                    Balconies = table.Column<int>(nullable: true),
                    BasementArea = table.Column<decimal>(nullable: true),
                    OutdoorParkingPlaces = table.Column<int>(nullable: true),
                    IndoorParkingPlaces = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyFeaturesDb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyPriceDb",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalGrossPrice = table.Column<decimal>(nullable: false),
                    PricePerMeter = table.Column<decimal>(nullable: false),
                    ResidentalRent = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyPriceDb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SellerContactDb",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: true),
                    Telephone = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerContactDb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfferDetailsDb",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(nullable: true),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateDateTime = table.Column<DateTime>(nullable: true),
                    OfferKind = table.Column<int>(nullable: false),
                    SellerContactId = table.Column<long>(nullable: true),
                    IsStillValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferDetailsDb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferDetailsDb_SellerContactDb_SellerContactId",
                        column: x => x.SellerContactId,
                        principalTable: "SellerContactDb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferDetailsId = table.Column<long>(nullable: true),
                    PropertyPriceId = table.Column<long>(nullable: true),
                    PropertyDetailsId = table.Column<long>(nullable: true),
                    PropertyAddressId = table.Column<long>(nullable: true),
                    PropertyFeaturesId = table.Column<long>(nullable: true),
                    RawDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entries_OfferDetailsDb_OfferDetailsId",
                        column: x => x.OfferDetailsId,
                        principalTable: "OfferDetailsDb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_PropertyAddressDb_PropertyAddressId",
                        column: x => x.PropertyAddressId,
                        principalTable: "PropertyAddressDb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_PropertyDetailsDb_PropertyDetailsId",
                        column: x => x.PropertyDetailsId,
                        principalTable: "PropertyDetailsDb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_PropertyFeaturesDb_PropertyFeaturesId",
                        column: x => x.PropertyFeaturesId,
                        principalTable: "PropertyFeaturesDb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_PropertyPriceDb_PropertyPriceId",
                        column: x => x.PropertyPriceId,
                        principalTable: "PropertyPriceDb",
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
                name: "IX_OfferDetailsDb_SellerContactId",
                table: "OfferDetailsDb",
                column: "SellerContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "OfferDetailsDb");

            migrationBuilder.DropTable(
                name: "PropertyAddressDb");

            migrationBuilder.DropTable(
                name: "PropertyDetailsDb");

            migrationBuilder.DropTable(
                name: "PropertyFeaturesDb");

            migrationBuilder.DropTable(
                name: "PropertyPriceDb");

            migrationBuilder.DropTable(
                name: "SellerContactDb");
        }
    }
}
