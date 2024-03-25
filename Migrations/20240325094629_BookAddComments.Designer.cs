﻿// <auto-generated />
using System;
using System.Collections.Generic;
using FpConsole;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FPConsole.Migrations
{
    [DbContext(typeof(DemoContext))]
    [Migration("20240325094629_BookAddComments")]
    partial class BookAddComments
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FpConsole.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string[]>("Comments")
                        .HasColumnType("text[]");

                    b.Property<string>("Descriptions")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.ComplexProperty<Dictionary<string, object>>("Aurthor", "FpConsole.Book.Aurthor#Aurthor", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Email")
                                .HasColumnType("text");

                            b1.Property<string>("Name")
                                .HasColumnType("text");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Price", "FpConsole.Book.Price#BookPrice", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<DateTime?>("ChangedDate")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<DateOnly?>("EndDate")
                                .HasColumnType("date");

                            b1.Property<decimal>("Price")
                                .HasColumnType("numeric");

                            b1.Property<DateOnly>("StartDate")
                                .HasColumnType("date");
                        });

                    b.HasKey("Id");

                    b.ToTable("Books");
                });
#pragma warning restore 612, 618
        }
    }
}
