﻿// <auto-generated />
using System;
using CRET.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CRET.DataAccessLayer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240507135020_certificate-validity-dates")]
    partial class certificatevaliditydates
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.25");

            modelBuilder.Entity("CRET.Domain.Models.Entities.AllowedUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserIdList")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AllowedUser");
                });

            modelBuilder.Entity("CRET.Domain.Models.Entities.Certificate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ActivationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("CertificateContent")
                        .HasColumnType("TEXT");

                    b.Property<string>("CertificateName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("CertificateStatus")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CsrContent")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImportCSRError")
                        .HasColumnType("TEXT");

                    b.Property<string>("IncidentNo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Info")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("OrganizartionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Tag")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Certificate");
                });

            modelBuilder.Entity("CRET.Domain.Models.Entities.Organization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Contact")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OrganizationName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Organization");
                });

            modelBuilder.Entity("CRET.Domain.Models.Entities.Setting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("BatchValidity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ConsumerValidity")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("InsValidity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LabValidity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProductionValidity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PulseValidity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ServiceValidity")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Setting");

                    b.HasData(
                        new
                        {
                            Id = new Guid("6c628d2c-9322-469f-861d-436472fe2627"),
                            BatchValidity = 90,
                            ConsumerValidity = 730,
                            CreatedAt = new DateTime(2024, 3, 28, 15, 40, 41, 0, DateTimeKind.Unspecified),
                            CreatedBy = "Admin",
                            InsValidity = 90,
                            LabValidity = 90,
                            ProductionValidity = 90,
                            PulseValidity = 90,
                            ServiceValidity = 90
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
