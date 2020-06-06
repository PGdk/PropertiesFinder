﻿// <auto-generated />
using System;
using DatabaseConnection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DatabaseConnection.Migrations
{
    [DbContext(typeof(TrovitDbContext))]
    [Migration("20200606123527_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DatabaseConnection.Logs", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("XRequestId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Models.Entry", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("OfferDetailsID")
                        .HasColumnType("int");

                    b.Property<int?>("PropertyAddressID")
                        .HasColumnType("int");

                    b.Property<int?>("PropertyDetailsID")
                        .HasColumnType("int");

                    b.Property<int?>("PropertyFeaturesID")
                        .HasColumnType("int");

                    b.Property<int?>("PropertyPriceID")
                        .HasColumnType("int");

                    b.Property<string>("RawDescription")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("OfferDetailsID");

                    b.HasIndex("PropertyAddressID");

                    b.HasIndex("PropertyDetailsID");

                    b.HasIndex("PropertyFeaturesID");

                    b.HasIndex("PropertyPriceID");

                    b.ToTable("Entries");
                });

            modelBuilder.Entity("Models.OfferDetails", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsStillValid")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("OfferKind")
                        .HasColumnType("int");

                    b.Property<int?>("SellerContactID")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("SellerContactID");

                    b.ToTable("OfferDetails");
                });

            modelBuilder.Entity("Models.PropertyAddress", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("City")
                        .HasColumnType("int");

                    b.Property<string>("DetailedAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("District")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StreetName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("PropertyAddress");
                });

            modelBuilder.Entity("Models.PropertyDetails", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Area")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("FloorNumber")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfRooms")
                        .HasColumnType("int");

                    b.Property<int?>("YearOfConstruction")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("PropertyDetails");
                });

            modelBuilder.Entity("Models.PropertyFeatures", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("Balconies")
                        .HasColumnType("int");

                    b.Property<decimal?>("BasementArea")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("GardenArea")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("IndoorParkingPlaces")
                        .HasColumnType("int");

                    b.Property<int?>("OutdoorParkingPlaces")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("PropertyFeatures");
                });

            modelBuilder.Entity("Models.PropertyPrice", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("PricePerMeter")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("ResidentalRent")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalGrossPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ID");

                    b.ToTable("PropertyPrice");
                });

            modelBuilder.Entity("Models.SellerContact", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telephone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("SellerContact");
                });

            modelBuilder.Entity("Models.Entry", b =>
                {
                    b.HasOne("Models.OfferDetails", "OfferDetails")
                        .WithMany()
                        .HasForeignKey("OfferDetailsID");

                    b.HasOne("Models.PropertyAddress", "PropertyAddress")
                        .WithMany()
                        .HasForeignKey("PropertyAddressID");

                    b.HasOne("Models.PropertyDetails", "PropertyDetails")
                        .WithMany()
                        .HasForeignKey("PropertyDetailsID");

                    b.HasOne("Models.PropertyFeatures", "PropertyFeatures")
                        .WithMany()
                        .HasForeignKey("PropertyFeaturesID");

                    b.HasOne("Models.PropertyPrice", "PropertyPrice")
                        .WithMany()
                        .HasForeignKey("PropertyPriceID");
                });

            modelBuilder.Entity("Models.OfferDetails", b =>
                {
                    b.HasOne("Models.SellerContact", "SellerContact")
                        .WithMany()
                        .HasForeignKey("SellerContactID");
                });
#pragma warning restore 612, 618
        }
    }
}
